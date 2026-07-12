param(
    [string]$Configuration = "Debug",
    [string]$TargetFramework = "net10.0-windows",
    [string]$RuntimeIdentifier = "win-x64",
    [string]$OutputDirectory,
    [int]$TimeoutSeconds = 20
)

$ErrorActionPreference = "Stop"

Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes

function Find-ElementByAutomationId {
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

function Wait-ForElementByAutomationId {
    param(
        [System.Windows.Automation.AutomationElement]$Root,
        [string]$AutomationId,
        [DateTime]$Deadline
    )

    while ([DateTime]::UtcNow -lt $Deadline) {
        $element = Find-ElementByAutomationId -Root $Root -AutomationId $AutomationId
        if ($null -ne $element) {
            return $element
        }
        Start-Sleep -Milliseconds 100
    }
    return $null
}

function Get-IsSelected {
    param([System.Windows.Automation.AutomationElement]$Element)

    $selectionItem = $null
    if (-not $Element.TryGetCurrentPattern(
        [System.Windows.Automation.SelectionItemPattern]::Pattern,
        [ref]$selectionItem
    )) {
        throw "SelectionItem pattern is unavailable: $($Element.Current.AutomationId)"
    }
    return $selectionItem.Current.IsSelected
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
$aiAssistantCaption = "AI $([char]0x52A9)$([char]0x624B)"
if (-not (Test-Path -LiteralPath $executable -PathType Leaf)) {
    throw "Recovered executable not found: $executable"
}

$process = Start-Process `
    -FilePath $executable `
    -WorkingDirectory $OutputDirectory `
    -PassThru

try {
    $deadline = [DateTime]::UtcNow.AddSeconds($TimeoutSeconds)
    $mainWindow = $null
    while ([DateTime]::UtcNow -lt $deadline -and $null -eq $mainWindow) {
        $process.Refresh()
        if ($process.HasExited) {
            throw "Recovered application exited with code $($process.ExitCode)."
        }

        $windowCondition = [System.Windows.Automation.AndCondition]::new(
            [System.Windows.Automation.PropertyCondition]::new(
                [System.Windows.Automation.AutomationElement]::ProcessIdProperty,
                $process.Id
            ),
            [System.Windows.Automation.PropertyCondition]::new(
                [System.Windows.Automation.AutomationElement]::NameProperty,
                "pvfUtility"
            )
        )
        $mainWindow = [System.Windows.Automation.AutomationElement]::RootElement.FindFirst(
            [System.Windows.Automation.TreeScope]::Children,
            $windowCondition
        )
        if ($null -eq $mainWindow) {
            Start-Sleep -Milliseconds 250
        }
    }
    if ($null -eq $mainWindow) {
        throw "Main window was not observed within $TimeoutSeconds seconds."
    }

    $deadline = [DateTime]::UtcNow.AddSeconds($TimeoutSeconds)
    $walker = [System.Windows.Automation.TreeWalker]::ControlViewWalker
    $findTab = $null
    $aiTab = $null
    $sameTabGroup = $false
    while ([DateTime]::UtcNow -lt $deadline -and -not $sameTabGroup) {
        $findTab = Find-ElementByAutomationId -Root $mainWindow -AutomationId "FindViewTabId"
        $aiTab = Find-ElementByAutomationId -Root $mainWindow -AutomationId "AiAssistantViewTabId"
        if ($null -ne $findTab -and $null -ne $aiTab) {
            $findTabParent = $walker.GetParent($findTab)
            $aiTabParent = $walker.GetParent($aiTab)
            $sameTabGroup = $null -ne $findTabParent -and $null -ne $aiTabParent -and
                (($findTabParent.GetRuntimeId() -join ",") -eq ($aiTabParent.GetRuntimeId() -join ","))
        }
        if (-not $sameTabGroup) {
            Start-Sleep -Milliseconds 100
        }
    }
    if ($null -eq $findTab -or $null -eq $aiTab) {
        $tabCondition = [System.Windows.Automation.PropertyCondition]::new(
            [System.Windows.Automation.AutomationElement]::ControlTypeProperty,
            [System.Windows.Automation.ControlType]::TabItem
        )
        $loadedTabs = $mainWindow.FindAll(
            [System.Windows.Automation.TreeScope]::Descendants,
            $tabCondition
        )
        $tabDescriptions = for ($index = 0; $index -lt $loadedTabs.Count; $index++) {
            $tab = $loadedTabs.Item($index)
            "Name='$($tab.Current.Name)', Id='$($tab.Current.AutomationId)'"
        }
        throw "The Find and AI assistant dock tabs were not both loaded. Loaded tabs: $($tabDescriptions -join '; ')"
    }
    if (-not $sameTabGroup) {
        throw "The Find and AI assistant views are not in the same tab group."
    }

    $buttonCondition = [System.Windows.Automation.AndCondition]::new(
        [System.Windows.Automation.PropertyCondition]::new(
            [System.Windows.Automation.AutomationElement]::NameProperty,
            $aiAssistantCaption
        ),
        [System.Windows.Automation.PropertyCondition]::new(
            [System.Windows.Automation.AutomationElement]::ControlTypeProperty,
            [System.Windows.Automation.ControlType]::Button
        )
    )
    $toolbarButton = $mainWindow.FindFirst(
        [System.Windows.Automation.TreeScope]::Descendants,
        $buttonCondition
    )
    if ($null -eq $toolbarButton) {
        throw "The AI assistant toolbar button was not found."
    }
    if (-not $toolbarButton.Current.IsEnabled) {
        throw "The AI assistant toolbar button is disabled."
    }

    $invoke = $null
    if (-not $toolbarButton.TryGetCurrentPattern(
        [System.Windows.Automation.InvokePattern]::Pattern,
        [ref]$invoke
    )) {
        throw "The AI assistant toolbar button is not invokable."
    }
    $invoke.Invoke()

    $prompt = Wait-ForElementByAutomationId `
        -Root $mainWindow `
        -AutomationId "AiAssistantPrompt" `
        -Deadline $deadline
    if ($null -eq $prompt) {
        throw "The AI assistant prompt did not appear after toolbar invocation."
    }

    while ([DateTime]::UtcNow -lt $deadline -and
        (-not (Get-IsSelected -Element $aiTab) -or -not $prompt.Current.HasKeyboardFocus)) {
        Start-Sleep -Milliseconds 100
    }
    if (-not (Get-IsSelected -Element $aiTab)) {
        throw "The AI assistant tab was not selected after toolbar invocation."
    }
    if (-not $prompt.Current.HasKeyboardFocus) {
        throw "The AI assistant prompt did not receive keyboard focus."
    }

    $findSelectionItem = $null
    if (-not $findTab.TryGetCurrentPattern(
        [System.Windows.Automation.SelectionItemPattern]::Pattern,
        [ref]$findSelectionItem
    )) {
        throw "The Find tab cannot be selected."
    }
    $findSelectionItem.Select()

    while ([DateTime]::UtcNow -lt $deadline -and -not (Get-IsSelected -Element $findTab)) {
        Start-Sleep -Milliseconds 100
    }
    if (-not (Get-IsSelected -Element $findTab) -or (Get-IsSelected -Element $aiTab)) {
        throw "The dock group could not switch back to the Find tab."
    }

    Write-Output "PASS: the enabled AI assistant toolbar button selected the AI tab, focused its prompt, and the shared dock group switched back to the Find tab."
}
finally {
    $process.Refresh()
    if (-not $process.HasExited) {
        Stop-Process -Id $process.Id -Force
        $process.WaitForExit()
    }
}
