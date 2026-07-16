param(
    [string]$Configuration = "Debug",
    [string]$RuntimeIdentifier = "win-x64",
    [string]$OutputDirectory
)

$ErrorActionPreference = "Stop"
. (Join-Path $PSScriptRoot "SingleFileSmokeCopy.ps1")
$toolsMenuPrefix = -join ([char]0x5DE5, [char]0x5177)
$publishName = -join ([char]0x53D1, [char]0x5E03)
$entryName = -join (
    [char]0x5B98, [char]0x65B9, [char]0x6CE8,
    [char]0x91CA, [char]0x6587, [char]0x6863
)

Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
Add-Type -AssemblyName System.Windows.Forms

function Find-ByAutomationId {
    param(
        [System.Windows.Automation.AutomationElement]$Root,
        [string]$AutomationId
    )

    $condition = [System.Windows.Automation.PropertyCondition]::new(
        [System.Windows.Automation.AutomationElement]::AutomationIdProperty,
        $AutomationId
    )
    return $Root.FindFirst(
        [System.Windows.Automation.TreeScope]::Descendants,
        $condition
    )
}

function Invoke-Element {
    param([System.Windows.Automation.AutomationElement]$Element)

    $pattern = $null
    if ($Element.TryGetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern, [ref]$pattern)) {
        $pattern.Invoke()
        return
    }
    throw "Element cannot be invoked: $($Element.Current.Name)"
}

function Get-ElementText {
    param([System.Windows.Automation.AutomationElement]$Element)

    $pattern = $null
    if ($Element.TryGetCurrentPattern([System.Windows.Automation.TextPattern]::Pattern, [ref]$pattern)) {
        return $pattern.DocumentRange.GetText(-1)
    }
    if ($Element.TryGetCurrentPattern([System.Windows.Automation.ValuePattern]::Pattern, [ref]$pattern)) {
        return $pattern.Current.Value
    }
    return ""
}

$projectRoot = Split-Path -Parent $PSScriptRoot
$OutputDirectory = Resolve-SingleFileTestOutputDirectory `
    -ProjectRoot $projectRoot `
    -OutputDirectory $OutputDirectory `
    -Scenario "official-annotation" `
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
    $mainWindow = $null
    $toolsMenu = $null
    $deadline = [DateTime]::UtcNow.AddSeconds(30)
    while ([DateTime]::UtcNow -lt $deadline -and ($null -eq $mainWindow -or $null -eq $toolsMenu)) {
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
            if ($element.Current.ControlType -eq [System.Windows.Automation.ControlType]::Window -and
                $element.Current.Name -eq "pvfUtility") {
                $mainWindow = $element
            }
            elseif ($element.Current.ControlType -eq [System.Windows.Automation.ControlType]::MenuItem -and
                $element.Current.Name -like "$toolsMenuPrefix*") {
                $toolsMenu = $element
            }
        }
    }

    if ($null -eq $mainWindow -or $null -eq $toolsMenu) {
        throw "Main window or Tools menu was not found."
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
    $entry = $null
    $visibleButtonNames = [System.Collections.Generic.List[string]]::new()
    $deadline = [DateTime]::UtcNow.AddSeconds(5)
    while ([DateTime]::UtcNow -lt $deadline -and $null -eq $entry) {
        $visibleButtonNames.Clear()
        $elements = [System.Windows.Automation.AutomationElement]::RootElement.FindAll(
            [System.Windows.Automation.TreeScope]::Descendants,
            $processCondition
        )
        for ($index = 0; $index -lt $elements.Count; $index++) {
            $element = $elements.Item($index)
            if ($element.Current.ControlType -eq [System.Windows.Automation.ControlType]::Button -and
                -not $element.Current.IsOffscreen) {
                [void]$visibleButtonNames.Add($element.Current.Name)
                if ($element.Current.Name -eq $entryName) {
                    $entry = $element
                    break
                }
            }
        }
        if ($null -eq $entry) {
            Start-Sleep -Milliseconds 250
        }
    }
    if ($null -eq $entry) {
        throw "Official-annotation entry is not visible in the Tools menu. Visible buttons: $($visibleButtonNames -join ', ')"
    }
    if (-not $entry.Current.IsEnabled) {
        throw "Official-annotation entry must be enabled without an open PVF."
    }

    $walker = [System.Windows.Automation.TreeWalker]::ControlViewWalker
    $previous = $walker.GetPreviousSibling($entry)
    while ($null -ne $previous -and $previous.Current.ControlType -ne [System.Windows.Automation.ControlType]::Button) {
        $previous = $walker.GetPreviousSibling($previous)
    }
    if ($null -eq $previous -or $previous.Current.Name -ne $publishName) {
        throw "Official-annotation entry is not directly below Publish."
    }

    Invoke-Element -Element $entry

    $filePicker = $null
    $reader = $null
    $wordWrap = $null
    $documentHost = $null
    $deadline = [DateTime]::UtcNow.AddSeconds(15)
    while ([DateTime]::UtcNow -lt $deadline -and
        ($null -eq $filePicker -or $null -eq $reader -or $null -eq $wordWrap)) {
        Start-Sleep -Milliseconds 250
        $filePicker = Find-ByAutomationId -Root $mainWindow -AutomationId "OfficialAnnotationFilePicker"
        $reader = Find-ByAutomationId -Root $mainWindow -AutomationId "OfficialAnnotationReader"
        $wordWrap = Find-ByAutomationId -Root $mainWindow -AutomationId "OfficialAnnotationWordWrap"
        $documentHost = Find-ByAutomationId -Root $mainWindow -AutomationId "DocumentHost"
    }
    if ($null -eq $filePicker -or $null -eq $reader -or $null -eq $wordWrap -or $null -eq $documentHost) {
        throw "Official-annotation document controls did not appear."
    }

    $togglePattern = $null
    if (-not $wordWrap.TryGetCurrentPattern([System.Windows.Automation.TogglePattern]::Pattern, [ref]$togglePattern)) {
        throw "Official-annotation word-wrap control does not support toggling."
    }
    $initialWrapState = $togglePattern.Current.ToggleState
    $togglePattern.Toggle()
    $deadline = [DateTime]::UtcNow.AddSeconds(3)
    while ([DateTime]::UtcNow -lt $deadline -and $togglePattern.Current.ToggleState -eq $initialWrapState) {
        Start-Sleep -Milliseconds 100
    }
    if ($togglePattern.Current.ToggleState -eq $initialWrapState) {
        throw "Official-annotation word-wrap state did not change after toggling."
    }
    $togglePattern.Toggle()

    $hostBounds = $documentHost.Current.BoundingRectangle
    $readerBounds = $reader.Current.BoundingRectangle
    if ($readerBounds.Left -lt ($hostBounds.Left + ($hostBounds.Width * 0.45))) {
        throw "Official-annotation reader was not placed to the right of the main document area. Host=$hostBounds Reader=$readerBounds"
    }

    $expandPattern = $null
    if (-not $filePicker.TryGetCurrentPattern([System.Windows.Automation.ExpandCollapsePattern]::Pattern, [ref]$expandPattern)) {
        throw "Official-annotation file picker cannot be expanded."
    }
    $expandPattern.Expand()
    Start-Sleep -Milliseconds 300

    $sample = $null
    $elements = [System.Windows.Automation.AutomationElement]::RootElement.FindAll(
        [System.Windows.Automation.TreeScope]::Descendants,
        $processCondition
    )
    for ($index = 0; $index -lt $elements.Count; $index++) {
        $element = $elements.Item($index)
        if ($element.Current.ControlType -eq [System.Windows.Automation.ControlType]::ListItem -and
            $element.Current.Name -eq "actionsample.act.txt") {
            $sample = $element
            break
        }
    }
    if ($null -eq $sample) {
        throw "actionsample.act.txt was not offered by the file picker."
    }

    $selectionPattern = $null
    if ($sample.TryGetCurrentPattern([System.Windows.Automation.SelectionItemPattern]::Pattern, [ref]$selectionPattern)) {
        $selectionPattern.Select()
    }
    elseif ($sample.TryGetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern, [ref]$selectionPattern)) {
        $selectionPattern.Invoke()
    }
    else {
        $valuePattern = $null
        if (-not $filePicker.TryGetCurrentPattern([System.Windows.Automation.ValuePattern]::Pattern, [ref]$valuePattern)) {
            throw "Official-annotation file picker does not support text entry."
        }
        $valuePattern.SetValue("actionsample.act.txt")
        $filePicker.SetFocus()
        [System.Windows.Forms.SendKeys]::SendWait("{ENTER}")
    }

    $readerText = ""
    $deadline = [DateTime]::UtcNow.AddSeconds(5)
    while ([DateTime]::UtcNow -lt $deadline -and $readerText -notmatch '\[MOTION\]') {
        Start-Sleep -Milliseconds 200
        $readerText = Get-ElementText -Element $reader
    }
    if ($readerText -notmatch '\[MOTION\]') {
        $status = Find-ByAutomationId -Root $mainWindow -AutomationId "OfficialAnnotationStatus"
        $statusText = if ($null -eq $status) { "<missing>" } else { $status.Current.Name }
        $pickerText = Get-ElementText -Element $filePicker
        throw "Selecting actionsample.act.txt did not update the read-only reader. Picker=$pickerText Status=$statusText ReaderLength=$($readerText.Length)"
    }

    Write-Output "PASS: official-annotation menu order, right-side document, word-wrap toggle, file selection, and content loading verified."
}
finally {
    if (-not $process.HasExited) {
        Stop-Process -Id $process.Id -Force
    }
}
