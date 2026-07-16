[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string]$OutputDirectory,

    [ValidateSet("Binary", "Leaf", "Core", "All")]
    [string]$RecoveredSourceLibraryMode = "All"
)

$ErrorActionPreference = "Stop"
. (Join-Path $PSScriptRoot "RecoveredSourceManifest.ps1")

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
if (-not (Test-Path -LiteralPath $OutputDirectory -PathType Container)) {
    throw "Single-file package directory not found: $OutputDirectory"
}

$executable = Join-Path $OutputDirectory "pvfUtility.exe"
if (-not (Test-Path -LiteralPath $executable -PathType Leaf) -or
    (Get-Item -LiteralPath $executable).Length -le 0) {
    throw "Published executable is missing or empty: $executable"
}

$executables = @(Get-ChildItem -LiteralPath $OutputDirectory -File -Recurse -Filter "*.exe")
if ($executables.Count -ne 1 -or
    $executables[0].FullName -ne [IO.Path]::GetFullPath($executable)) {
    throw "The single-file package must contain exactly one executable, pvfUtility.exe, at its root."
}

$unexpectedFiles = @(Get-ChildItem -LiteralPath $OutputDirectory -File -Recurse | Where-Object {
    $_.Extension -in @(".dll", ".pdb") -or
    $_.Name.EndsWith(".deps.json", [StringComparison]::OrdinalIgnoreCase) -or
    $_.Name.EndsWith(".runtimeconfig.json", [StringComparison]::OrdinalIgnoreCase)
})
if ($unexpectedFiles.Count -gt 0) {
    throw "Unexpected standalone single-file publish files: $($unexpectedFiles.FullName -join ', ')"
}

foreach ($requiredFile in @(
    "Options\AppConfig.json",
    "Options\Bookmarks.json",
    "Defaults\Options\AppConfig.json",
    "Defaults\Options\Bookmarks.json"
)) {
    $requiredPath = Join-Path $OutputDirectory $requiredFile
    if (-not (Test-Path -LiteralPath $requiredPath -PathType Leaf)) {
        throw "Required single-file package file not found: $requiredPath"
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

$leafManifestEntries = @(
    "ICSharpCode.AvalonEdit|ICSharpCode.AvalonEdit.csproj",
    "pvfUtility.WebApi.Dto|pvfUtility.WebApi.Dto.csproj",
    "Swordfish.NET.CollectionsV3|Swordfish.NET.CollectionsV3.csproj",
    "UnitComboLib|UnitComboLib.csproj",
    "Utools|Utools.csproj",
    "Whetstone.ChatGPT|Whetstone.ChatGPT.csproj",
    "WpfRangeControls|WpfRangeControls.csproj"
)
$coreManifestEntries = $leafManifestEntries + @(
    "GMTool.Dot|GMTool.Dot.csproj",
    "GMTool.Services|GMTool.Services.csproj",
    "GMTool.SqlModel|GMTool.SqlModel.csproj",
    "HL|HL.csproj",
    "PvfCode.Dot|PvfCode.Dot.csproj",
    "PvfCode.LoggerBase|PvfCode.LoggerBase.csproj",
    "PvfCode.NPK.Utils|PvfCode.NPK.Utils.csproj",
    "ServiceLocator|ServiceLocator.csproj",
    "SevenZipSharp|SevenZipSharp.csproj",
    "Vulild.Ionic.Zlib|Vulild.Ionic.Zlib.csproj"
)
$expectedManifestEntries = switch ($RecoveredSourceLibraryMode) {
    "Binary" { @() }
    "Leaf" { $leafManifestEntries }
    "Core" { $coreManifestEntries }
    "All" {
        $coreManifestEntries + @(
            "PvfCode.Models|PvfCode.Models.csproj",
            "PvfCode.Services|PvfCode.Services.csproj",
            "TextEditLib|TextEditLib.csproj"
        )
    }
}

$manifest = Join-Path $OutputDirectory "recovered-source-libraries.txt"
if ($RecoveredSourceLibraryMode -eq "Binary") {
    if (Test-Path -LiteralPath $manifest -PathType Leaf) {
        $binaryManifestLines = @(Get-Content -LiteralPath $manifest | Where-Object {
            -not [string]::IsNullOrWhiteSpace($_)
        })
        if ($binaryManifestLines.Count -gt 0) {
            throw "Binary source-library mode must not contain source project manifest entries: $manifest"
        }
    }
    $manifestEntryCount = 0
}
else {
    $manifestEntries = @(Read-RecoveredSourceManifest -Path $manifest -RequireEntries)
    $actualManifestEntries = @($manifestEntries | ForEach-Object {
        "$($_.AssemblyName)|$($_.ProjectFileName)"
    })
    $differences = @(Compare-Object `
        -ReferenceObject $expectedManifestEntries `
        -DifferenceObject $actualManifestEntries)
    if ($differences.Count -gt 0) {
        $differenceText = $differences | ForEach-Object {
            "$($_.SideIndicator) $($_.InputObject)"
        }
        throw "Recovered source manifest does not match mode $RecoveredSourceLibraryMode`: $($differenceText -join '; ')"
    }
    $manifestEntryCount = $manifestEntries.Count
}

$executableSizeMiB = [math]::Round((Get-Item -LiteralPath $executable).Length / 1MB, 2)
Write-Output "PASS: Single-file package verified. Executable size: $executableSizeMiB MiB; recovered source projects: $manifestEntryCount ($RecoveredSourceLibraryMode)."
