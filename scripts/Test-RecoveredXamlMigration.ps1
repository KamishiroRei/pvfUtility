param(
    [string]$Configuration = "Debug",
    [string]$RuntimeIdentifier = "win-x64",

    [ValidateSet("Legacy", "Hybrid", "SourceOnly")]
    [string]$RecoveredWpfResourceMode = "Hybrid",

    [string]$OutputDirectory
)

$ErrorActionPreference = "Stop"

$projectRoot = Split-Path -Parent $PSScriptRoot
if ([string]::IsNullOrWhiteSpace($OutputDirectory)) {
    $OutputDirectory = Join-Path `
        $projectRoot `
        "artifacts\publish\local\$RecoveredWpfResourceMode\$Configuration\$RuntimeIdentifier"
}
elseif (-not [IO.Path]::IsPathRooted($OutputDirectory)) {
    $OutputDirectory = Join-Path $projectRoot $OutputDirectory
}
$OutputDirectory = [IO.Path]::GetFullPath($OutputDirectory)

$executable = Join-Path $OutputDirectory "pvfUtility.exe"
if (-not (Test-Path -LiteralPath $executable -PathType Leaf)) {
    throw "pvfUtility executable was not found: $executable"
}

$resultPath = Join-Path ([IO.Path]::GetTempPath()) "pvfUtility-xaml-self-test-$([Guid]::NewGuid().ToString('N')).txt"
$previousMode = $env:PVFUTILITY_RECOVERED_XAML_SELF_TEST
$previousResult = $env:PVFUTILITY_RECOVERED_XAML_SELF_TEST_RESULT
$process = $null

try {
    $env:PVFUTILITY_RECOVERED_XAML_SELF_TEST = "1"
    $env:PVFUTILITY_RECOVERED_XAML_SELF_TEST_RESULT = $resultPath

    $process = Start-Process `
        -FilePath $executable `
        -WorkingDirectory $OutputDirectory `
        -PassThru

    if (-not $process.WaitForExit(30000)) {
        Stop-Process -Id $process.Id -Force -ErrorAction SilentlyContinue
        throw "Recovered XAML self-test did not exit within 30 seconds."
    }

    if (-not (Test-Path -LiteralPath $resultPath -PathType Leaf)) {
        throw "The recovered XAML self-test did not write a result file."
    }
    $details = Get-Content -LiteralPath $resultPath -Raw -Encoding UTF8

    if ($process.ExitCode -ne 0) {
        throw "Recovered XAML self-test failed with exit code $($process.ExitCode).`n$details"
    }

    if ([string]::IsNullOrWhiteSpace($details) -or
        -not $details.StartsWith("PASS:", [StringComparison]::Ordinal)) {
        throw "Recovered XAML self-test did not report an explicit PASS result.`n$details"
    }

    Write-Output $details.Trim()
}
finally {
    if ($null -ne $process) {
        $process.Dispose()
    }
    $env:PVFUTILITY_RECOVERED_XAML_SELF_TEST = $previousMode
    $env:PVFUTILITY_RECOVERED_XAML_SELF_TEST_RESULT = $previousResult
    Remove-Item -LiteralPath $resultPath -Force -ErrorAction SilentlyContinue
}
