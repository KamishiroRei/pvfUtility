[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string]$OutputDirectory,

    [ValidateSet("Binary", "Leaf", "Core", "All")]
    [string]$RecoveredSourceLibraryMode = "All"
)

$ErrorActionPreference = "Stop"

& (Join-Path $PSScriptRoot "Test-SingleFilePackage.ps1") `
    -OutputDirectory $OutputDirectory `
    -RecoveredSourceLibraryMode $RecoveredSourceLibraryMode
