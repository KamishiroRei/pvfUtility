param(
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Release",

    [ValidateSet("win-x64")]
    [string]$RuntimeIdentifier = "win-x64",

    [Parameter(Mandatory)]
    [ValidateRange(0, [int]::MaxValue)]
    [int]$ExpectedMigratedCount,

    [ValidateRange(1, [int]::MaxValue)]
    [int]$ExpectedTotalCount = 126
)

$ErrorActionPreference = "Stop"

if ($ExpectedMigratedCount -ge $ExpectedTotalCount) {
    throw "The incomplete SourceOnly guard requires a migrated count below the total count."
}

$projectRoot = Split-Path -Parent $PSScriptRoot
$project = Join-Path $projectRoot "pvfUtility.csproj"
$arguments = @(
    "build",
    $project,
    "-c", $Configuration,
    "-r", $RuntimeIdentifier,
    "--no-restore",
    "-p:RecoveredWpfResourceMode=SourceOnly",
    "-p:RecoveredSourceLibraryMode=All"
)

$startInfo = [Diagnostics.ProcessStartInfo]::new()
$startInfo.FileName = "dotnet"
$startInfo.WorkingDirectory = $projectRoot
$startInfo.UseShellExecute = $false
$startInfo.RedirectStandardOutput = $true
$startInfo.RedirectStandardError = $true
foreach ($argument in $arguments) {
    [void]$startInfo.ArgumentList.Add($argument)
}

$process = [Diagnostics.Process]::new()
$process.StartInfo = $startInfo
try {
    if (-not $process.Start()) {
        throw "Failed to start dotnet for the SourceOnly guard check."
    }

    $standardOutputTask = $process.StandardOutput.ReadToEndAsync()
    $standardErrorTask = $process.StandardError.ReadToEndAsync()
    $process.WaitForExit()

    $standardOutput = $standardOutputTask.GetAwaiter().GetResult()
    $standardError = $standardErrorTask.GetAwaiter().GetResult()
    $exitCode = $process.ExitCode
}
finally {
    $process.Dispose()
}

$outputParts = @(
    $standardOutput.TrimEnd(),
    $standardError.TrimEnd()
) | Where-Object { -not [string]::IsNullOrWhiteSpace($_) }
$output = $outputParts -join [Environment]::NewLine

if ($exitCode -eq 0) {
    if (-not [string]::IsNullOrWhiteSpace($output)) {
        Write-Output $output
    }
    throw "SourceOnly unexpectedly succeeded with an incomplete migration ledger."
}

$expectedMessage =
    "SourceOnly requires $ExpectedTotalCount migrated XAML entries; found $ExpectedMigratedCount"
if ($output.IndexOf($expectedMessage, [StringComparison]::Ordinal) -lt 0) {
    if (-not [string]::IsNullOrWhiteSpace($output)) {
        Write-Output $output
    }
    throw "SourceOnly failed without the expected $ExpectedMigratedCount/$ExpectedTotalCount migration guard."
}

Write-Output "PASS: SourceOnly rejected the incomplete $ExpectedMigratedCount/$ExpectedTotalCount migration ledger."
