param(
    [string]$Configuration = "Debug",
    [string]$RuntimeIdentifier = "win-x64",
    [string]$OutputDirectory
)

$ErrorActionPreference = "Stop"
. (Join-Path $PSScriptRoot "SingleFileSmokeCopy.ps1")
$toolsMenuPrefix = -join ([char]0x5DE5, [char]0x5177)
$independentDropName = -join (
    [char]0x72EC, [char]0x7ACB, [char]0x6389, [char]0x843D,
    [char]0x7F16, [char]0x8F91, [char]0x5668
)
$entryName = -join (
    [char]0x6DF1, [char]0x6E0A, '/', [char]0x7FFB, [char]0x724C,
    [char]0x7206, [char]0x7387, [char]0x7BA1, [char]0x7406
)

Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes

$projectRoot = Split-Path -Parent $PSScriptRoot
$OutputDirectory = Resolve-SingleFileTestOutputDirectory `
    -ProjectRoot $projectRoot `
    -OutputDirectory $OutputDirectory `
    -Scenario "drop-rate-menu" `
    -RecoveredWpfResourceMode Hybrid `
    -Configuration $Configuration `
    -RuntimeIdentifier $RuntimeIdentifier `
    -UseSmokeCopy
$executable = Join-Path $OutputDirectory "pvfUtility.exe"
if (-not (Test-Path -LiteralPath $executable -PathType Leaf)) {
    throw "Executable not found: $executable"
}

$process = Start-Process -FilePath $executable -WorkingDirectory $OutputDirectory -PassThru
$processCondition = [System.Windows.Automation.PropertyCondition]::new(
    [System.Windows.Automation.AutomationElement]::ProcessIdProperty,
    $process.Id
)

try {
    $toolsMenu = $null
    $deadline = [DateTime]::UtcNow.AddSeconds(30)
    while ([DateTime]::UtcNow -lt $deadline -and $null -eq $toolsMenu) {
        Start-Sleep -Milliseconds 250
        $process.Refresh()
        if ($process.HasExited) {
            throw "Application exited with code $($process.ExitCode) before the Tools menu appeared."
        }
        $elements = [System.Windows.Automation.AutomationElement]::RootElement.FindAll(
            [System.Windows.Automation.TreeScope]::Descendants,
            $processCondition
        )
        for ($index = 0; $index -lt $elements.Count; $index++) {
            $element = $elements.Item($index)
            if ($element.Current.ControlType -eq [System.Windows.Automation.ControlType]::MenuItem -and
                $element.Current.Name -like "$toolsMenuPrefix*") {
                $toolsMenu = $element
                break
            }
        }
    }

    if ($null -eq $toolsMenu) {
        throw "Tools menu was not found."
    }

    $pattern = $null
    if ($toolsMenu.TryGetCurrentPattern([System.Windows.Automation.ExpandCollapsePattern]::Pattern, [ref]$pattern)) {
        $pattern.Expand()
    }
    elseif ($toolsMenu.TryGetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern, [ref]$pattern)) {
        $pattern.Invoke()
    }
    else {
        throw "Tools menu cannot be expanded through UI Automation."
    }

    Start-Sleep -Milliseconds 500
    $elements = [System.Windows.Automation.AutomationElement]::RootElement.FindAll(
        [System.Windows.Automation.TreeScope]::Descendants,
        $processCondition
    )
    $menuNames = [System.Collections.Generic.List[string]]::new()
    $independentDropElement = $null
    $entryElement = $null
    for ($index = 0; $index -lt $elements.Count; $index++) {
        $element = $elements.Item($index)
        if ($element.Current.ControlType -eq [System.Windows.Automation.ControlType]::Button -and
            -not $element.Current.IsOffscreen) {
            $name = $element.Current.Name
            [void]$menuNames.Add($name)
            if ($name -eq $independentDropName) {
                $independentDropElement = $element
            }
            elseif ($name -eq $entryName) {
                $entryElement = $element
            }
        }
    }

    $independentDropIndex = $menuNames.IndexOf($independentDropName)
    $entryIndex = $menuNames.IndexOf($entryName)
    if ($entryIndex -lt 0) {
        throw "Drop-rate management entry is not visible in the Tools menu."
    }
    if ($entryIndex -ne $independentDropIndex + 1) {
        throw "Drop-rate management entry is not directly below Independent Drop Editor. Visible buttons: $($menuNames -join ', ')"
    }
    if ($null -eq $independentDropElement -or $independentDropElement.Current.IsEnabled) {
        throw "Independent Drop Editor should be disabled before a PVF file is opened."
    }
    if ($null -eq $entryElement -or $entryElement.Current.IsEnabled) {
        throw "Drop-rate management entry should be disabled before a PVF file is opened."
    }

    Write-Output "PASS: drop-rate management entry is directly below Independent Drop Editor and shares its disabled state before a PVF file is opened."
}
finally {
    if (-not $process.HasExited) {
        Stop-Process -Id $process.Id -Force
    }
}
