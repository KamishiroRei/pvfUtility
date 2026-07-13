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

    $windowPattern = $null
    if ($mainWindow.TryGetCurrentPattern(
        [System.Windows.Automation.WindowPattern]::Pattern,
        [ref]$windowPattern
    )) {
        $windowPattern.SetWindowVisualState(
            [System.Windows.Automation.WindowVisualState]::Maximized
        )
        Start-Sleep -Milliseconds 500
    }

    $deadline = [DateTime]::UtcNow.AddSeconds($TimeoutSeconds)
    $aiView = Wait-ForElementByAutomationId `
        -Root $mainWindow `
        -AutomationId "AiAssistantConversationView" `
        -Deadline $deadline
    $prompt = Wait-ForElementByAutomationId `
        -Root $mainWindow `
        -AutomationId "AiAssistantPrompt" `
        -Deadline $deadline
    if ($null -eq $aiView -or $null -eq $prompt) {
        throw "The default AI assistant panel did not expose its conversation controls."
    }

    $aiPanel = Find-ElementByAutomationId -Root $mainWindow -AutomationId "AiAssistantView"
    if ($null -eq $aiPanel) {
        throw "The AI assistant dock panel was not found."
    }

    $imageCondition = [System.Windows.Automation.PropertyCondition]::new(
        [System.Windows.Automation.AutomationElement]::ControlTypeProperty,
        [System.Windows.Automation.ControlType]::Image
    )
    $panelImages = $aiPanel.FindAll(
        [System.Windows.Automation.TreeScope]::Descendants,
        $imageCondition
    )
    for ($index = 0; $index -lt $panelImages.Count; $index++) {
        $imageBounds = $panelImages.Item($index).Current.BoundingRectangle
        if ($imageBounds.Width -gt 128 -or $imageBounds.Height -gt 128) {
            throw "The AI assistant is covered by an oversized panel image: $imageBounds."
        }
    }

    $documentHost = Find-ElementByAutomationId -Root $mainWindow -AutomationId "DocumentHost"
    $outputView = Find-ElementByAutomationId -Root $mainWindow -AutomationId "outPutViewTabId"
    if ($null -eq $documentHost -or $null -eq $outputView) {
        throw "The panels required to verify the default layout were not found."
    }

    $sendButton = Find-ElementByAutomationId -Root $mainWindow -AutomationId "AiAssistantSend"
    if ($null -eq $sendButton) {
        throw "The AI assistant composer did not expose its send action."
    }

    $promptBounds = $prompt.Current.BoundingRectangle
    $documentBounds = $documentHost.Current.BoundingRectangle
    $outputBounds = $outputView.Current.BoundingRectangle
    if ($promptBounds.Left -lt ($documentBounds.Right - 2) -or
        $promptBounds.Left -lt ($outputBounds.Right - 2)) {
        throw "The AI assistant is not the rightmost docked panel."
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

    while ([DateTime]::UtcNow -lt $deadline -and -not $prompt.Current.HasKeyboardFocus) {
        Start-Sleep -Milliseconds 100
    }
    if (-not $prompt.Current.HasKeyboardFocus) {
        throw "The AI assistant prompt did not receive keyboard focus."
    }

    Write-Output "PASS: the complete AI conversation panel is visible by default, docked at the far right, and receives toolbar focus."
}
finally {
    $process.Refresh()
    if (-not $process.HasExited) {
        Stop-Process -Id $process.Id -Force
        $process.WaitForExit()
    }
}
