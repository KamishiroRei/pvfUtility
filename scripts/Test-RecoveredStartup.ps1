param(
    [string]$Configuration = "Debug",
    [string]$TargetFramework = "net10.0-windows",
    [string]$RuntimeIdentifier = "win-x64",
    [string]$OutputDirectory,
    [int]$ObservationSeconds = 15
)

$ErrorActionPreference = "Stop"

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
if ([string]::IsNullOrWhiteSpace($OutputDirectory)) {
    $OutputDirectory = Join-Path $projectRoot "bin\$Configuration\$TargetFramework\$RuntimeIdentifier"
}
elseif (-not [IO.Path]::IsPathRooted($OutputDirectory)) {
    $OutputDirectory = Join-Path $projectRoot $OutputDirectory
}
$OutputDirectory = [IO.Path]::GetFullPath($OutputDirectory)
$executable = Join-Path $OutputDirectory "pvfUtility.exe"
$sourceAssemblyManifest = Join-Path $OutputDirectory "recovered-source-libraries.txt"
$errorTitlePattern = "Error|Exception|$([char]0x9519)$([char]0x8BEF)|$([char]0x5F02)$([char]0x5E38)"

if (-not (Test-Path -LiteralPath $executable -PathType Leaf)) {
    throw "Recovered executable not found: $executable"
}

$verifiedSourceAssemblies = @{}
if (Test-Path -LiteralPath $sourceAssemblyManifest -PathType Leaf) {
    foreach ($line in Get-Content -LiteralPath $sourceAssemblyManifest) {
        if ([string]::IsNullOrWhiteSpace($line)) {
            continue
        }

        $parts = $line -split "\|", 2
        if ($parts.Count -ne 2) {
            throw "Invalid recovered source assembly manifest entry: $line"
        }

        $assemblyName = $parts[0]
        $projectPath = $parts[1]
        $projectName = [IO.Path]::GetFileNameWithoutExtension($projectPath)
        $sourceBuildDirectory = Join-Path $projectRoot "SourceLibraries\.build\$projectName\bin\$Configuration"
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

    Write-Output "PASS: fully populated main window remained available for $ObservationSeconds seconds with no error window. Recovered source assemblies verified: $($verifiedSourceAssemblies.Count); loaded during startup: $loadedRecoveredAssemblies."
}
finally {
    $process.Refresh()
    if (-not $process.HasExited) {
        Stop-Process -Id $process.Id -Force
        $process.WaitForExit()
    }
}
