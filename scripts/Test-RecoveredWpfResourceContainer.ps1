[CmdletBinding()]
param(
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Debug",

    [string]$TargetFramework = "net10.0-windows",

    [ValidateSet("win-x64")]
    [string]$RuntimeIdentifier = "win-x64"
)

$ErrorActionPreference = "Stop"

$projectRoot = [IO.Path]::GetFullPath((Split-Path -Parent $PSScriptRoot))
$intermediateDirectory = Join-Path `
    $projectRoot `
    "obj\$Configuration\$TargetFramework\$RuntimeIdentifier"
$targetDirectory = Join-Path `
    $projectRoot `
    "bin\$Configuration\$TargetFramework\$RuntimeIdentifier"
$tool = Join-Path `
    $projectRoot `
    "tools\PvfResourceMerger\bin\$Configuration\net10.0\PvfResourceMerger.dll"
$baseResource = Join-Path $projectRoot "Resources\pvfUtility.g.resources"
$overlayResource = Join-Path $intermediateDirectory "pvfUtility.g.resources"
$mergedResource = Join-Path $intermediateDirectory "RecoveredWpfResources\pvfUtility.g.resources"
$replacementManifest = Join-Path $intermediateDirectory "RecoveredWpfResources\replacement-keys.txt"
$applicationAssembly = Join-Path $targetDirectory "pvfUtility.dll"

$arguments = @(
    $tool,
    "audit",
    "--base", $baseResource,
    "--overlay", $overlayResource,
    "--output", $mergedResource,
    "--replacement-manifest", $replacementManifest,
    "--expected-count", "247",
    "--expected-baml-count", "126",
    "--assembly", $applicationAssembly,
    "--resource-name", "pvfUtility.g.resources"
)

foreach ($requiredPath in @(
    $tool,
    $baseResource,
    $overlayResource,
    $mergedResource,
    $replacementManifest,
    $applicationAssembly
)) {
    if (-not (Test-Path -LiteralPath $requiredPath -PathType Leaf)) {
        throw "Recovered WPF resource audit input was not found: $requiredPath"
    }
}

& dotnet @arguments
if ($LASTEXITCODE -ne 0) {
    throw "Recovered WPF resource container audit failed with exit code $LASTEXITCODE."
}
