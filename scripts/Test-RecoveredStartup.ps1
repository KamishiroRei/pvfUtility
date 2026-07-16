param(
    [string]$Configuration = "Debug",
    [string]$RuntimeIdentifier = "win-x64",

    [ValidateSet("Legacy", "Hybrid")]
    [string]$RecoveredWpfResourceMode = "Hybrid",

    [string]$OutputDirectory,
    [int]$ObservationSeconds = 15,
    [switch]$SingleFile
)

$ErrorActionPreference = "Stop"
. (Join-Path $PSScriptRoot "RecoveredSourceManifest.ps1")
. (Join-Path $PSScriptRoot "SingleFileSmokeCopy.ps1")

Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes

function Get-WindowDiagnosticText {
    param([System.Windows.Automation.AutomationElement]$Window)

    $values = [System.Collections.Generic.HashSet[string]]::new()
    $elements = $Window.FindAll(
        [System.Windows.Automation.TreeScope]::Descendants,
        [System.Windows.Automation.Condition]::TrueCondition
    )

    for ($index = 0; $index -lt $elements.Count; $index++) {
        $element = $elements.Item($index)
        $name = $element.Current.Name
        if (-not [string]::IsNullOrWhiteSpace($name)) {
            [void]$values.Add($name)
        }

        foreach ($pattern in @(
            [System.Windows.Automation.ValuePattern]::Pattern,
            [System.Windows.Automation.TextPattern]::Pattern
        )) {
            $patternObject = $null
            if (-not $element.TryGetCurrentPattern($pattern, [ref]$patternObject)) {
                continue
            }

            $value = switch ($pattern.ProgrammaticName) {
                "ValuePatternIdentifiers.Pattern" { $patternObject.Current.Value }
                "TextPatternIdentifiers.Pattern" { $patternObject.DocumentRange.GetText(-1) }
            }
            if (-not [string]::IsNullOrWhiteSpace($value)) {
                [void]$values.Add($value.Trim())
            }
        }
    }

    return ($values -join [Environment]::NewLine)
}

$projectRoot = Split-Path -Parent $PSScriptRoot
$usesDefaultOutputDirectory = [string]::IsNullOrWhiteSpace($OutputDirectory)
if ($usesDefaultOutputDirectory) {
    $SingleFile = $true
}
$OutputDirectory = Resolve-SingleFileTestOutputDirectory `
    -ProjectRoot $projectRoot `
    -OutputDirectory $OutputDirectory `
    -Scenario "startup" `
    -RecoveredWpfResourceMode $RecoveredWpfResourceMode `
    -Configuration $Configuration `
    -RuntimeIdentifier $RuntimeIdentifier `
    -UseSmokeCopy:$SingleFile
$executable = Join-Path $OutputDirectory "pvfUtility.exe"
$sourceAssemblyManifest = Join-Path $OutputDirectory "recovered-source-libraries.txt"
$errorTitlePattern = "Error|Exception|$([char]0x9519)$([char]0x8BEF)|$([char]0x5F02)$([char]0x5E38)"

if ($ObservationSeconds -lt 15) {
    throw "ObservationSeconds must be at least 15 seconds."
}
if (-not (Test-Path -LiteralPath $executable -PathType Leaf)) {
    throw "Recovered executable not found: $executable"
}
if ($SingleFile -and -not (Test-Path -LiteralPath $sourceAssemblyManifest -PathType Leaf)) {
    throw "Recovered source assembly manifest not found for single-file verification: $sourceAssemblyManifest"
}

$verifiedSourceAssemblies = @{}
$verifiedSourceAssemblyCount = 0
if (Test-Path -LiteralPath $sourceAssemblyManifest -PathType Leaf) {
    $manifestEntries = @(Read-RecoveredSourceManifest `
        -Path $sourceAssemblyManifest `
        -RequireEntries:$SingleFile)
    foreach ($entry in $manifestEntries) {
        $assemblyName = $entry.AssemblyName
        $projectFileName = $entry.ProjectFileName
        $projectName = $entry.ProjectName

        $sourceBuildRoot = [IO.Path]::GetFullPath((Join-Path $projectRoot "SourceLibraries\.build"))
        $sourceBuildDirectory = [IO.Path]::GetFullPath(
            (Join-Path $sourceBuildRoot "$projectName\bin\$Configuration")
        )
        $sourceBuildPrefix = $sourceBuildRoot + [IO.Path]::DirectorySeparatorChar
        if (-not $sourceBuildDirectory.StartsWith($sourceBuildPrefix, [StringComparison]::OrdinalIgnoreCase)) {
            throw "Recovered source project build directory escaped its expected root: $projectFileName"
        }
        $sourceAssembly = Get-ChildItem `
            -LiteralPath $sourceBuildDirectory `
            -Recurse `
            -Filter "$assemblyName.dll" `
            -File `
            -ErrorAction SilentlyContinue |
            Sort-Object LastWriteTimeUtc -Descending |
            Select-Object -First 1
        $outputAssembly = Join-Path $OutputDirectory "$assemblyName.dll"

        if ($null -eq $sourceAssembly) {
            throw "Recovered source assembly build output not found: $assemblyName ($sourceBuildDirectory)"
        }
        $verifiedSourceAssemblyCount++
        if ($SingleFile) {
            continue
        }
        if (-not (Test-Path -LiteralPath $outputAssembly -PathType Leaf)) {
            throw "Recovered source assembly was not copied to the application output: $outputAssembly"
        }

        $sourceHash = (Get-FileHash -LiteralPath $sourceAssembly.FullName -Algorithm SHA256).Hash
        $outputHash = (Get-FileHash -LiteralPath $outputAssembly -Algorithm SHA256).Hash
        if ($sourceHash -ne $outputHash) {
            throw "Application output does not contain the recovered source build of $assemblyName."
        }

        $verifiedSourceAssemblies[$assemblyName] = [IO.Path]::GetFullPath($outputAssembly)
    }
}

$process = Start-Process `
    -FilePath $executable `
    -WorkingDirectory $OutputDirectory `
    -PassThru

$mainWindowSeen = $false
$mainWindow = $null
$observedTitles = [System.Collections.Generic.HashSet[string]]::new()
$deadline = [DateTime]::UtcNow.AddSeconds($ObservationSeconds)

try {
    while ([DateTime]::UtcNow -lt $deadline) {
        $process.Refresh()
        if ($process.HasExited) {
            throw "Recovered application exited with code $($process.ExitCode)."
        }

        $root = [System.Windows.Automation.AutomationElement]::RootElement
        $processCondition = [System.Windows.Automation.PropertyCondition]::new(
            [System.Windows.Automation.AutomationElement]::ProcessIdProperty,
            $process.Id
        )
        $windows = $root.FindAll(
            [System.Windows.Automation.TreeScope]::Children,
            $processCondition
        )

        for ($index = 0; $index -lt $windows.Count; $index++) {
            $window = $windows.Item($index)
            if ($window.Current.ControlType -ne [System.Windows.Automation.ControlType]::Window) {
                continue
            }

            $title = $window.Current.Name
            if (-not [string]::IsNullOrWhiteSpace($title)) {
                [void]$observedTitles.Add($title)
            }

            if ($title -eq "pvfUtility") {
                $mainWindowSeen = $true
                $mainWindow = $window
            }

            if ($title -match $errorTitlePattern) {
                $details = Get-WindowDiagnosticText -Window $window
                throw "Startup error window detected: $title$([Environment]::NewLine)$details"
            }
        }

        Start-Sleep -Milliseconds 250
    }

    if (-not $mainWindowSeen) {
        $titles = ($observedTitles | Sort-Object) -join ", "
        throw "Main window was not observed. Window titles: $titles"
    }

    $descendants = $mainWindow.FindAll(
        [System.Windows.Automation.TreeScope]::Descendants,
        [System.Windows.Automation.Condition]::TrueCondition
    )
    if ($descendants.Count -lt 100) {
        throw "Main window UI is incomplete: only $($descendants.Count) descendant controls were found."
    }

    $requiredAutomationIds = @(
        "BarSubItemLinksubFile",
        "FilelistLayoutPanel",
        "DocumentHost"
    )
    foreach ($automationId in $requiredAutomationIds) {
        $idCondition = [System.Windows.Automation.PropertyCondition]::new(
            [System.Windows.Automation.AutomationElement]::AutomationIdProperty,
            $automationId
        )
        $control = $mainWindow.FindFirst(
            [System.Windows.Automation.TreeScope]::Descendants,
            $idCondition
        )
        if ($null -eq $control) {
            throw "Main window control was not loaded: $automationId"
        }
    }

    $loadedRecoveredAssemblies = 0
    if ($verifiedSourceAssemblies.Count -gt 0) {
        $loadedModules = @($process.Modules)
        foreach ($entry in $verifiedSourceAssemblies.GetEnumerator()) {
            $module = $loadedModules |
                Where-Object { $_.ModuleName -eq "$($entry.Key).dll" } |
                Select-Object -First 1
            if ($null -eq $module) {
                continue
            }

            if ([IO.Path]::GetFullPath($module.FileName) -ne $entry.Value) {
                throw "Recovered assembly $($entry.Key) was loaded from an unexpected path: $($module.FileName)"
            }
            $loadedRecoveredAssemblies++
        }
    }

    $publishMode = if ($SingleFile) { "single-file" } else { "directory" }
    Write-Output "PASS: fully populated main window remained available for $ObservationSeconds seconds with no error window. Publish mode: $publishMode; recovered source assemblies verified: $verifiedSourceAssemblyCount; loaded during startup: $loadedRecoveredAssemblies."
}
finally {
    $process.Refresh()
    if (-not $process.HasExited) {
        Stop-Process -Id $process.Id -Force
        $process.WaitForExit()
    }
}
