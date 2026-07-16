param(
    [Parameter(Mandatory)]
    [string]$OutputDirectory
)

$ErrorActionPreference = "Stop"

function Assert-DirectoryHasFiles {
    param(
        [Parameter(Mandatory)]
        [string]$Path,

        [Parameter(Mandatory)]
        [string]$Description
    )

    if (-not (Test-Path -LiteralPath $Path -PathType Container)) {
        throw "$Description directory not found: $Path"
    }
    if ($null -eq (Get-ChildItem -LiteralPath $Path -File -Recurse | Select-Object -First 1)) {
        throw "$Description directory is empty: $Path"
    }
}

$projectRoot = Split-Path -Parent $PSScriptRoot
if (-not [IO.Path]::IsPathRooted($OutputDirectory)) {
    $OutputDirectory = Join-Path $projectRoot $OutputDirectory
}
$OutputDirectory = [IO.Path]::GetFullPath($OutputDirectory)

$executable = Join-Path $OutputDirectory "pvfUtility.exe"
if (-not (Test-Path -LiteralPath $executable -PathType Leaf) -or
    (Get-Item -LiteralPath $executable).Length -le 0) {
    throw "Published executable is missing or empty: $executable"
}

$executables = @(Get-ChildItem -LiteralPath $OutputDirectory -File -Recurse -Filter "*.exe")
if ($executables.Count -ne 1 -or
    $executables[0].FullName -ne [IO.Path]::GetFullPath($executable)) {
    throw "The release package must contain exactly one executable at its root."
}

$unexpectedFiles = @(Get-ChildItem -LiteralPath $OutputDirectory -File -Recurse | Where-Object {
    $_.Extension -in @(".dll", ".pdb") -or
    $_.Name.EndsWith(".deps.json", [StringComparison]::OrdinalIgnoreCase) -or
    $_.Name.EndsWith(".runtimeconfig.json", [StringComparison]::OrdinalIgnoreCase)
})
if ($unexpectedFiles.Count -gt 0) {
    throw "Unexpected standalone release files: $($unexpectedFiles.FullName -join ', ')"
}

foreach ($requiredFile in @(
    "Options\AppConfig.json",
    "Options\Bookmarks.json",
    "Defaults\Options\AppConfig.json",
    "Defaults\Options\Bookmarks.json",
    "recovered-source-libraries.txt"
)) {
    $requiredPath = Join-Path $OutputDirectory $requiredFile
    if (-not (Test-Path -LiteralPath $requiredPath -PathType Leaf)) {
        throw "Required release file not found: $requiredPath"
    }
}

Assert-DirectoryHasFiles `
    -Path (Join-Path $OutputDirectory "Options\PvfComments") `
    -Description "Editable PvfComments"
Assert-DirectoryHasFiles `
    -Path (Join-Path $OutputDirectory "Defaults\Options\PvfComments") `
    -Description "Default PvfComments"
Assert-DirectoryHasFiles `
    -Path (Join-Path $OutputDirectory "Resources\AgentKnowledge") `
    -Description "AI knowledge resources"
Assert-DirectoryHasFiles `
    -Path (Join-Path $OutputDirectory "Resources\OfficialAnnotationTranslation") `
    -Description "Official annotation resources"

$manifest = Join-Path $OutputDirectory "recovered-source-libraries.txt"
$manifestEntryCount = 0
foreach ($line in Get-Content -LiteralPath $manifest) {
    if ([string]::IsNullOrWhiteSpace($line)) {
        continue
    }

    $parts = $line -split "\|", 2
    if ($parts.Count -ne 2 -or
        [string]::IsNullOrWhiteSpace($parts[0]) -or
        [string]::IsNullOrWhiteSpace($parts[1]) -or
        $line -match '([A-Za-z]:[\\/]|\\\\)' -or
        [IO.Path]::IsPathRooted($parts[1]) -or
        $parts[1] -match '[\\/]' -or
        [IO.Path]::GetExtension($parts[1]) -ne ".csproj") {
        throw "Recovered source manifest contains an invalid or machine-specific entry: $line"
    }

    $manifestEntryCount++
}
if ($manifestEntryCount -eq 0) {
    throw "Recovered source manifest must contain at least one project entry: $manifest"
}

$executableSizeMiB = [math]::Round((Get-Item -LiteralPath $executable).Length / 1MB, 2)
Write-Output "PASS: GitHub release package verified. Executable size: $executableSizeMiB MiB; recovered source projects: $manifestEntryCount."
