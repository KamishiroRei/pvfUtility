[CmdletBinding()]
param(
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Debug",

    [ValidateSet("Legacy", "Hybrid", "SourceOnly")]
    [string]$RecoveredWpfResourceMode = "Hybrid",

    [ValidateSet("Binary", "Leaf", "Core", "All")]
    [string]$RecoveredSourceLibraryMode = "All",

    [ValidateSet("win-x64")]
    [string]$RuntimeIdentifier = "win-x64",

    [string]$OutputDirectory
)

$ErrorActionPreference = "Stop"

$validWpfResourceModes = @("Legacy", "Hybrid", "SourceOnly")
if ($null -eq ($validWpfResourceModes | Where-Object {
    [StringComparer]::Ordinal.Equals($_, $RecoveredWpfResourceMode)
} | Select-Object -First 1)) {
    throw "RecoveredWpfResourceMode is case-sensitive and must be Legacy, Hybrid, or SourceOnly."
}

function Invoke-DotNet {
    param(
        [Parameter(Mandatory)]
        [string[]]$Arguments,

        [Parameter(Mandatory)]
        [string]$FailureMessage
    )

    & dotnet @Arguments
    if ($LASTEXITCODE -ne 0) {
        throw "$FailureMessage Exit code: $LASTEXITCODE."
    }
}

$projectRoot = [IO.Path]::GetFullPath((Split-Path -Parent $PSScriptRoot))
$publishRoot = [IO.Path]::GetFullPath((Join-Path $projectRoot "artifacts\publish"))

if ([string]::IsNullOrWhiteSpace($OutputDirectory)) {
    $OutputDirectory = Join-Path `
        $publishRoot `
        "local\$RecoveredWpfResourceMode\$Configuration\$RuntimeIdentifier"
}
elseif (-not [IO.Path]::IsPathRooted($OutputDirectory)) {
    $OutputDirectory = Join-Path $projectRoot $OutputDirectory
}
$OutputDirectory = [IO.Path]::GetFullPath($OutputDirectory)

$publishPrefix = $publishRoot.TrimEnd(
    [IO.Path]::DirectorySeparatorChar,
    [IO.Path]::AltDirectorySeparatorChar
) + [IO.Path]::DirectorySeparatorChar
if (-not $OutputDirectory.StartsWith($publishPrefix, [StringComparison]::OrdinalIgnoreCase)) {
    throw "OutputDirectory must be a child of the repository artifacts\publish directory: $OutputDirectory"
}

$installedSdks = @(& dotnet --list-sdks)
if ($LASTEXITCODE -ne 0) {
    throw "Unable to query installed .NET SDKs. Exit code: $LASTEXITCODE."
}
if ($null -eq ($installedSdks | Where-Object { $_ -match '^\s*10\.' } | Select-Object -First 1)) {
    throw ".NET 10 SDK is required. Installed SDKs: $($installedSdks -join '; ')"
}

if (Test-Path -LiteralPath $OutputDirectory) {
    Remove-Item -LiteralPath $OutputDirectory -Recurse -Force
}
New-Item -ItemType Directory -Path $OutputDirectory | Out-Null

$solution = Join-Path $projectRoot "pvfUtility.sln"
$project = Join-Path $projectRoot "pvfUtility.csproj"
$commonProperties = @(
    "-p:RecoveredWpfResourceMode=$RecoveredWpfResourceMode",
    "-p:RecoveredSourceLibraryMode=$RecoveredSourceLibraryMode"
)

Invoke-DotNet `
    -Arguments (@("restore", $solution, "-r", $RuntimeIdentifier) + $commonProperties) `
    -FailureMessage "dotnet restore failed."

$publishArguments = @(
    "publish",
    $project,
    "-c", $Configuration,
    "-r", $RuntimeIdentifier,
    "--self-contained", "true",
    "--no-restore",
    "-p:PublishProfile=SingleFile",
    "-o", $OutputDirectory
) + $commonProperties
Invoke-DotNet -Arguments $publishArguments -FailureMessage "dotnet publish failed."

if ($RecoveredWpfResourceMode -eq "Hybrid") {
    & (Join-Path $PSScriptRoot "Test-RecoveredWpfResourceContainer.ps1") `
        -Configuration $Configuration `
        -RuntimeIdentifier $RuntimeIdentifier
}

$defaultOptions = Join-Path $OutputDirectory "Defaults\Options"
if (-not (Test-Path -LiteralPath $defaultOptions -PathType Container)) {
    throw "Published default options directory not found: $defaultOptions"
}

$editableOptions = Join-Path $OutputDirectory "Options"
if (Test-Path -LiteralPath $editableOptions) {
    Remove-Item -LiteralPath $editableOptions -Recurse -Force
}
Copy-Item -LiteralPath $defaultOptions -Destination $editableOptions -Recurse

& (Join-Path $PSScriptRoot "Test-SingleFilePackage.ps1") `
    -OutputDirectory $OutputDirectory `
    -RecoveredSourceLibraryMode $RecoveredSourceLibraryMode

& (Join-Path $PSScriptRoot "Test-RecoveredXamlMigration.ps1") `
    -Configuration $Configuration `
    -RuntimeIdentifier $RuntimeIdentifier `
    -RecoveredWpfResourceMode $RecoveredWpfResourceMode `
    -OutputDirectory $OutputDirectory

$executable = Join-Path $OutputDirectory "pvfUtility.exe"
Write-Output "Single-file publish completed."
Write-Output "  Configuration: $Configuration"
Write-Output "  WPF resource mode: $RecoveredWpfResourceMode"
Write-Output "  Source library mode: $RecoveredSourceLibraryMode"
Write-Output "  Executable: $executable"
