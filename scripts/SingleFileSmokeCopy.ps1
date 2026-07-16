function New-SingleFileSmokeCopy {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)]
        [string]$ProjectRoot,

        [Parameter(Mandatory)]
        [string]$SourceDirectory,

        [Parameter(Mandatory)]
        [ValidatePattern('^[A-Za-z0-9._-]+$')]
        [string]$Scenario,

        [Parameter(Mandatory)]
        [ValidateSet("Legacy", "Hybrid")]
        [string]$RecoveredWpfResourceMode,

        [Parameter(Mandatory)]
        [ValidateSet("Debug", "Release")]
        [string]$Configuration,

        [Parameter(Mandatory)]
        [ValidateSet("win-x64")]
        [string]$RuntimeIdentifier
    )

    $projectRootPath = [IO.Path]::GetFullPath($ProjectRoot)
    if (-not [IO.Path]::IsPathRooted($SourceDirectory)) {
        $SourceDirectory = Join-Path $projectRootPath $SourceDirectory
    }
    $sourcePath = [IO.Path]::GetFullPath($SourceDirectory)

    $publishRoot = [IO.Path]::GetFullPath((Join-Path $projectRootPath "artifacts\publish"))
    $publishPrefix = $publishRoot.TrimEnd(
        [IO.Path]::DirectorySeparatorChar,
        [IO.Path]::AltDirectorySeparatorChar
    ) + [IO.Path]::DirectorySeparatorChar
    if (-not $sourcePath.StartsWith($publishPrefix, [StringComparison]::OrdinalIgnoreCase)) {
        throw "Single-file smoke source must be inside artifacts\publish: $sourcePath"
    }
    if (-not (Test-Path -LiteralPath $sourcePath -PathType Container)) {
        throw "Single-file package not found. Run scripts\Build-SingleFile.ps1 first: $sourcePath"
    }

    $smokeRoot = [IO.Path]::GetFullPath((Join-Path $projectRootPath "artifacts\smoke\local-tests"))
    $smokeDirectory = [IO.Path]::GetFullPath((Join-Path `
        $smokeRoot `
        "$Scenario\$RecoveredWpfResourceMode\$Configuration\$RuntimeIdentifier"))
    $smokePrefix = $smokeRoot.TrimEnd(
        [IO.Path]::DirectorySeparatorChar,
        [IO.Path]::AltDirectorySeparatorChar
    ) + [IO.Path]::DirectorySeparatorChar
    if (-not $smokeDirectory.StartsWith($smokePrefix, [StringComparison]::OrdinalIgnoreCase)) {
        throw "Single-file smoke directory escaped artifacts\smoke: $smokeDirectory"
    }

    if (Test-Path -LiteralPath $smokeDirectory) {
        Remove-Item -LiteralPath $smokeDirectory -Recurse -Force
    }
    New-Item -ItemType Directory -Path (Split-Path -Parent $smokeDirectory) -Force | Out-Null
    Copy-Item -LiteralPath $sourcePath -Destination $smokeDirectory -Recurse
    return $smokeDirectory
}

function Resolve-SingleFileTestOutputDirectory {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)]
        [string]$ProjectRoot,

        [AllowEmptyString()]
        [string]$OutputDirectory,

        [Parameter(Mandatory)]
        [ValidatePattern('^[A-Za-z0-9._-]+$')]
        [string]$Scenario,

        [Parameter(Mandatory)]
        [ValidateSet("Legacy", "Hybrid")]
        [string]$RecoveredWpfResourceMode,

        [Parameter(Mandatory)]
        [ValidateSet("Debug", "Release")]
        [string]$Configuration,

        [Parameter(Mandatory)]
        [ValidateSet("win-x64")]
        [string]$RuntimeIdentifier,

        [switch]$UseSmokeCopy
    )

    $projectRootPath = [IO.Path]::GetFullPath($ProjectRoot)
    if ([string]::IsNullOrWhiteSpace($OutputDirectory)) {
        $OutputDirectory = Join-Path `
            $projectRootPath `
            "artifacts\publish\local\$RecoveredWpfResourceMode\$Configuration\$RuntimeIdentifier"
    }
    elseif (-not [IO.Path]::IsPathRooted($OutputDirectory)) {
        $OutputDirectory = Join-Path $projectRootPath $OutputDirectory
    }
    $outputPath = [IO.Path]::GetFullPath($OutputDirectory)

    if (-not $UseSmokeCopy) {
        return $outputPath
    }

    $publishRoot = [IO.Path]::GetFullPath((Join-Path $projectRootPath "artifacts\publish"))
    $publishPrefix = $publishRoot.TrimEnd(
        [IO.Path]::DirectorySeparatorChar,
        [IO.Path]::AltDirectorySeparatorChar
    ) + [IO.Path]::DirectorySeparatorChar
    if (-not $outputPath.StartsWith($publishPrefix, [StringComparison]::OrdinalIgnoreCase)) {
        return $outputPath
    }

    return New-SingleFileSmokeCopy `
        -ProjectRoot $projectRootPath `
        -SourceDirectory $outputPath `
        -Scenario $Scenario `
        -RecoveredWpfResourceMode $RecoveredWpfResourceMode `
        -Configuration $Configuration `
        -RuntimeIdentifier $RuntimeIdentifier
}
