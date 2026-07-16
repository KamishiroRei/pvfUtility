function Read-RecoveredSourceManifest {
    param(
        [Parameter(Mandatory)]
        [string]$Path,

        [switch]$RequireEntries
    )

    if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) {
        throw "Recovered source assembly manifest not found: $Path"
    }

    $entries = [System.Collections.Generic.List[object]]::new()
    foreach ($line in Get-Content -LiteralPath $Path) {
        if ([string]::IsNullOrWhiteSpace($line)) {
            continue
        }

        $parts = $line -split "\|", 2
        if ($parts.Count -ne 2) {
            throw "Invalid recovered source assembly manifest entry: $line"
        }

        $assemblyName = $parts[0]
        $projectFileName = $parts[1]
        if ([string]::IsNullOrWhiteSpace($assemblyName) -or
            [IO.Path]::IsPathRooted($assemblyName) -or
            $assemblyName -match '[\\/]' -or
            $assemblyName.IndexOfAny([IO.Path]::GetInvalidFileNameChars()) -ge 0) {
            throw "Recovered source assembly name must not contain a path: $line"
        }
        if ([string]::IsNullOrWhiteSpace($projectFileName) -or
            [IO.Path]::IsPathRooted($projectFileName) -or
            $projectFileName -match '[\\/]' -or
            $projectFileName.IndexOfAny([IO.Path]::GetInvalidFileNameChars()) -ge 0 -or
            [IO.Path]::GetExtension($projectFileName) -ne ".csproj") {
            throw "Recovered source assembly manifest must not contain a project path: $line"
        }

        $projectName = [IO.Path]::GetFileNameWithoutExtension($projectFileName)
        if ([string]::IsNullOrWhiteSpace($projectName) -or $projectName -eq "." -or $projectName -eq "..") {
            throw "Invalid recovered source project filename: $projectFileName"
        }

        $entries.Add([pscustomobject]@{
            AssemblyName = $assemblyName
            ProjectFileName = $projectFileName
            ProjectName = $projectName
        })
    }

    if ($RequireEntries -and $entries.Count -eq 0) {
        throw "Recovered source assembly manifest must contain at least one project entry: $Path"
    }

    return $entries
}
