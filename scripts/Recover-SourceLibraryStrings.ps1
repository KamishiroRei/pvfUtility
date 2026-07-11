[CmdletBinding()]
param(
    [string]$ProjectRoot = (Split-Path -Parent $PSScriptRoot),
    [switch]$Apply
)

$ErrorActionPreference = 'Stop'

$ProjectRoot = (Resolve-Path -LiteralPath $ProjectRoot).Path
$inlineScript = Join-Path $PSScriptRoot 'Inline-ObfuscatedStrings.ps1'
$sourceRoot = Join-Path $ProjectRoot 'SourceLibraries'
$binaryRoot = Join-Path $ProjectRoot 'lib'
$mapRoot = Join-Path $ProjectRoot 'docs'

$libraries = @(
    [pscustomobject]@{
        Name = 'Utools'
        ResolverCallPrefix = 'tM01hAKa6eKr81Hu7Bp.hph6KJ8eMu('
        ResolverTypeName = 'rLJlZLKm3LsxdNk7i8i.tM01hAKa6eKr81Hu7Bp'
        ResolverMethodName = 'hph6KJ8eMu'
        ModuleTypeName = '_003CModule_003E_007B3bb06327_002D9a57_002D42cd_002Db977_002D93fc4f7e063f_007D'
        ModuleReflectionTypeName = '<Module>{3bb06327-9a57-42cd-b977-93fc4f7e063f}'
        ModuleHolderName = 'm_24a6b42d3c1f40edbaa5def8e1ff6cd6'
        CallerGuardFieldName = 'Yfh6siGJo6'
        ResolverSourcePath = 'rLJlZLKm3LsxdNk7i8i\tM01hAKa6eKr81Hu7Bp.cs'
        MapName = 'RECOVERED_UTOOLS_STRING_MAP.csv'
    }
    [pscustomobject]@{
        Name = 'PvfCode.Models'
        ResolverCallPrefix = 'y0NX9A5ilkvMVUiSJPT.DdnpDGStV9('
        ResolverTypeName = 'aHAIcu5MRehohss6q1w.y0NX9A5ilkvMVUiSJPT'
        ResolverMethodName = 'DdnpDGStV9'
        ModuleTypeName = '_003CModule_003E_007Bca6a0dd5_002Dc5c4_002D415b_002Db834_002D43407116930c_007D'
        ModuleReflectionTypeName = '<Module>{ca6a0dd5-c5c4-415b-b834-43407116930c}'
        ModuleHolderName = 'm_85a9146d95c445febccf19a89589f9ed'
        CallerGuardFieldName = 'QdvpBEar4B'
        ResolverSourcePath = 'aHAIcu5MRehohss6q1w\y0NX9A5ilkvMVUiSJPT.cs'
        MapName = 'RECOVERED_PVFCODE_MODELS_STRING_MAP.csv'
    }
    [pscustomobject]@{
        Name = 'PvfCode.Services'
        ResolverCallPrefix = 'UgWCJrAu3GFOif0JZec.vynAWe9nKW('
        ResolverTypeName = 'MAZjo0AIuF8Yaay9Gx8.UgWCJrAu3GFOif0JZec'
        ResolverMethodName = 'vynAWe9nKW'
        ModuleTypeName = '_003CModule_003E_007B664e49ba_002D2879_002D4bd1_002D93bc_002D7aba2e7a5e8e_007D'
        ModuleReflectionTypeName = '<Module>{664e49ba-2879-4bd1-93bc-7aba2e7a5e8e}'
        ModuleHolderName = 'm_4313c27851d048d9a6b5a75d5d0ef703'
        CallerGuardFieldName = 'Cf1Af3fxUW'
        ResolverSourcePath = 'MAZjo0AIuF8Yaay9Gx8\UgWCJrAu3GFOif0JZec.cs'
        MapName = 'RECOVERED_PVFCODE_SERVICES_STRING_MAP.csv'
    }
)

foreach ($library in $libraries) {
    Write-Host "Recovering strings from $($library.Name)..."
    & $inlineScript `
        -ProjectRoot (Join-Path $sourceRoot $library.Name) `
        -AssemblyPath (Join-Path $binaryRoot ($library.Name + '.dll')) `
        -MapPath (Join-Path $mapRoot $library.MapName) `
        -ResolverCallPrefix $library.ResolverCallPrefix `
        -ResolverTypeName $library.ResolverTypeName `
        -ResolverMethodName $library.ResolverMethodName `
        -ModuleTypeName $library.ModuleTypeName `
        -ModuleReflectionTypeName $library.ModuleReflectionTypeName `
        -ModuleHolderName $library.ModuleHolderName `
        -CallerGuardFieldName $library.CallerGuardFieldName `
        -ResolverSourcePath $library.ResolverSourcePath `
        -Apply:$Apply
}
