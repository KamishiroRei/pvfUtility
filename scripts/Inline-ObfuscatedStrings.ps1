[CmdletBinding()]
param(
    [string]$ProjectRoot = (Split-Path -Parent $PSScriptRoot),
    [string]$AssemblyPath,
    [string]$MapPath,
    [string]$ResolverCallPrefix = 'XuWKbxKPiIriguO3qdm.isB9S9CTfp(',
    [string]$ResolverTypeName = 'Kaf2VaKZrNWGtTDcnO6.XuWKbxKPiIriguO3qdm',
    [string]$ResolverMethodName = 'isB9S9CTfp',
    [string]$ModuleTypeName = '_003CModule_003E_007Be69553f9_002Deb39_002D49cd_002D9d77_002Da4ee304d94f7_007D',
    [string]$ModuleReflectionTypeName,
    [string]$ModuleHolderName = 'm_7b86004a82fb42ee97ea7d5e58574563',
    [string]$CallerGuardFieldName = 'Iwk9qWaB81',
    [string]$ResolverSourcePath,
    [switch]$Apply,
    [switch]$RepairEscapingFromMap
)

$ErrorActionPreference = 'Stop'

$ProjectRoot = (Resolve-Path -LiteralPath $ProjectRoot).Path
if (-not $AssemblyPath) {
    $AssemblyPath = Join-Path $ProjectRoot 'bin\Debug\net6.0-windows\pvfUtility.dll'
}
if (-not $MapPath) {
    $MapPath = Join-Path $ProjectRoot 'docs\RECOVERED_STRING_MAP.csv'
}

$AssemblyPath = (Resolve-Path -LiteralPath $AssemblyPath).Path
$assemblyDirectory = Split-Path -Parent $AssemblyPath
$callPrefix = $ResolverCallPrefix
$ModuleReflectionTypeName = if ($ModuleReflectionTypeName) { $ModuleReflectionTypeName } else { $ModuleTypeName }
$fieldAccessPattern = [regex]::Escape($moduleTypeName) + '\.' + [regex]::Escape($moduleHolderName) + '\.(?<field>m_[0-9a-f]{32})'
$integerLiteralPattern = '(?:[+\-~]\s*)*(?:0x[0-9A-Fa-f]+|\d+)'
$argumentPattern = [regex]::new('^\s*(?<numbers>' + $integerLiteralPattern + '(?:\s*\^\s*' + $integerLiteralPattern + ')*)\s*\^\s*' + $fieldAccessPattern + '\s*$')
$utf8NoBom = [System.Text.UTF8Encoding]::new($false, $true)

function Read-SourceFile {
    param([Parameter(Mandatory)][string]$Path)

    $bytes = [IO.File]::ReadAllBytes($Path)
    $offset = 0
    $encoding = $utf8NoBom

    if ($bytes.Length -ge 3 -and $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF) {
        $offset = 3
        $encoding = [System.Text.UTF8Encoding]::new($true, $true)
    }
    elseif ($bytes.Length -ge 2 -and $bytes[0] -eq 0xFF -and $bytes[1] -eq 0xFE) {
        $offset = 2
        $encoding = [System.Text.UnicodeEncoding]::new($false, $true, $true)
    }
    elseif ($bytes.Length -ge 2 -and $bytes[0] -eq 0xFE -and $bytes[1] -eq 0xFF) {
        $offset = 2
        $encoding = [System.Text.UnicodeEncoding]::new($true, $true, $true)
    }

    [pscustomobject]@{
        Text = $encoding.GetString($bytes, $offset, $bytes.Length - $offset)
        Encoding = $encoding
    }
}

function ConvertTo-UInt32Bits {
    param([Parameter(Mandatory)][string]$Literal)

    $Literal = $Literal.Replace(' ', '').Replace("`t", '').Trim()
    $match = [regex]::Match($Literal, '^(?<operators>[+\-~]*)(?<number>0x[0-9A-Fa-f]+|\d+)$')
    if (-not $match.Success) {
        throw "Unsupported integer literal: $Literal"
    }

    $numberLiteral = $match.Groups['number'].Value
    if ($numberLiteral.StartsWith('0x', [StringComparison]::OrdinalIgnoreCase)) {
        [uint32]$bits = [Convert]::ToUInt32($numberLiteral.Substring(2), 16)
    }
    else {
        $number = [uint64]::Parse($numberLiteral, [Globalization.CultureInfo]::InvariantCulture)
        if ($number -gt [uint32]::MaxValue) {
            throw "Integer literal is outside 32-bit range: $Literal"
        }
        [uint32]$bits = $number
    }

    $operators = $match.Groups['operators'].Value.ToCharArray()
    [array]::Reverse($operators)
    foreach ($operator in $operators) {
        switch ($operator) {
            '+' { continue }
            '-' {
                $bits = if ($bits -eq 0) { 0 } else { [uint32]([uint64]0x100000000 - [uint64]$bits) }
                continue
            }
            '~' {
                $bits = [uint32]($bits -bxor [uint32]::MaxValue)
                continue
            }
        }
    }
    return $bits
}

function ConvertInt32To-UInt32Bits {
    param([Parameter(Mandatory)][int]$Value)

    return [BitConverter]::ToUInt32([BitConverter]::GetBytes($Value), 0)
}

function ConvertTo-CSharpStringLiteral {
    param([AllowEmptyString()][Parameter(Mandatory)][string]$Value)

    $builder = [Text.StringBuilder]::new($Value.Length + 2)
    [void]$builder.Append('"')
    foreach ($character in $Value.ToCharArray()) {
        $code = [int]$character
        if ($code -eq 0x08) { [void]$builder.Append('\b'); continue }
        if ($code -eq 0x09) { [void]$builder.Append('\t'); continue }
        if ($code -eq 0x0A) { [void]$builder.Append('\n'); continue }
        if ($code -eq 0x0C) { [void]$builder.Append('\f'); continue }
        if ($code -eq 0x0D) { [void]$builder.Append('\r'); continue }
        if ($code -eq 0x22) { [void]$builder.Append('\"'); continue }
        if ($code -eq 0x5C) { [void]$builder.Append('\\'); continue }

        $category = [Globalization.CharUnicodeInfo]::GetUnicodeCategory($character)
        if ($category -in @(
                [Globalization.UnicodeCategory]::Control,
                [Globalization.UnicodeCategory]::Format,
                [Globalization.UnicodeCategory]::Surrogate,
                [Globalization.UnicodeCategory]::LineSeparator,
                [Globalization.UnicodeCategory]::ParagraphSeparator)) {
            [void]$builder.AppendFormat('\u{0:X4}', $code)
        }
        else {
            [void]$builder.Append($character)
        }
    }
    [void]$builder.Append('"')
    return $builder.ToString()
}

function ConvertTo-PreviousBuggyStringLiteral {
    param([AllowEmptyString()][Parameter(Mandatory)][string]$Value)

    $builder = [Text.StringBuilder]::new($Value.Length + 2)
    [void]$builder.Append('"')
    foreach ($character in $Value.ToCharArray()) {
        $code = [int]$character
        if ($code -eq 0x08) { [void]$builder.Append('\b') }
        elseif ($code -eq 0x09) { [void]$builder.Append('\t') }
        elseif ($code -eq 0x0A) { [void]$builder.Append('\n') }
        elseif ($code -eq 0x0C) { [void]$builder.Append('\f') }
        elseif ($code -eq 0x0D) { [void]$builder.Append('\r') }
        elseif ($code -eq 0x22) { [void]$builder.Append('\"') }
        elseif ($code -eq 0x5C) { [void]$builder.Append('\\') }

        if ([Globalization.CharUnicodeInfo]::GetUnicodeCategory($character) -in @(
                [Globalization.UnicodeCategory]::Control,
                [Globalization.UnicodeCategory]::Format,
                [Globalization.UnicodeCategory]::Surrogate,
                [Globalization.UnicodeCategory]::LineSeparator,
                [Globalization.UnicodeCategory]::ParagraphSeparator)) {
            [void]$builder.AppendFormat('\u{0:X4}', $code)
        }
        else {
            [void]$builder.Append($character)
        }
    }
    [void]$builder.Append('"')
    return $builder.ToString()
}

function Find-Calls {
    param(
        [Parameter(Mandatory)][string]$Path,
        [AllowEmptyString()][Parameter(Mandatory)][string]$Text
    )

    $calls = [Collections.Generic.List[object]]::new()
    $position = 0
    while (($start = $Text.IndexOf($callPrefix, $position, [StringComparison]::Ordinal)) -ge 0) {
        $argumentStart = $start + $callPrefix.Length
        $depth = 1
        $cursor = $argumentStart
        while ($cursor -lt $Text.Length -and $depth -gt 0) {
            if ($Text[$cursor] -eq '(') {
                $depth++
            }
            elseif ($Text[$cursor] -eq ')') {
                $depth--
            }
            $cursor++
        }
        if ($depth -ne 0) {
            throw "Unbalanced obfuscated string call in $Path at character $start"
        }

        $expression = $Text.Substring($argumentStart, $cursor - $argumentStart - 1).Trim()
        $match = $argumentPattern.Match($expression)
        if (-not $match.Success) {
            throw "Unsupported obfuscated string expression in ${Path}: $expression"
        }

        $calls.Add([pscustomobject]@{
                Start = $start
                End = $cursor
                Expression = $expression
                NumberLiterals = @($match.Groups['numbers'].Value -split '\s*\^\s*')
                FieldName = $match.Groups['field'].Value
            })
        $position = $cursor
    }
    return $calls
}

if ($RepairEscapingFromMap) {
    if (-not (Test-Path -LiteralPath $MapPath)) {
        throw "Recovered string map was not found: $MapPath"
    }

    $rows = Import-Csv -LiteralPath $MapPath
    $changedFiles = 0
    $changedCalls = 0
    foreach ($fileGroup in @($rows | Group-Object File)) {
        $path = Join-Path $ProjectRoot $fileGroup.Name
        $source = Read-SourceFile -Path $path
        $text = $source.Text
        $repairs = @(
            foreach ($valueGroup in @($fileGroup.Group | Group-Object Value)) {
                $value = [string]$valueGroup.Group[0].Value
                $buggy = ConvertTo-PreviousBuggyStringLiteral -Value $value
                $correct = ConvertTo-CSharpStringLiteral -Value $value
                if ($buggy -ne $correct) {
                    [pscustomobject]@{
                        Buggy = $buggy
                        Correct = $correct
                        Expected = $valueGroup.Count
                    }
                }
            }
        ) | Sort-Object { $_.Buggy.Length } -Descending

        $fileChanged = $false
        foreach ($repair in $repairs) {
            $actual = [regex]::Matches($text, [regex]::Escape($repair.Buggy)).Count
            if ($actual -gt $repair.Expected) {
                throw "Escaping repair count exceeded the map in ${path}: expected at most $($repair.Expected), found $actual for $($repair.Buggy)"
            }
            if ($actual -gt 0) {
                $text = $text.Replace($repair.Buggy, $repair.Correct)
                $changedCalls += $actual
                $fileChanged = $true
            }
        }

        if ($fileChanged) {
            [IO.File]::WriteAllText($path, $text, $source.Encoding)
            $changedFiles++
        }
    }

    [pscustomobject]@{
        RepairedFiles = $changedFiles
        RepairedCalls = $changedCalls
        MapPath = $MapPath
    } | Format-List
    exit 0
}

$resolverSourceFullPath = if ($ResolverSourcePath) { Join-Path $ProjectRoot $ResolverSourcePath } else { $null }
$sourceFiles = Get-ChildItem -LiteralPath $ProjectRoot -Recurse -File -Filter '*.cs' |
    Where-Object {
        $_.FullName -notmatch '\\(bin|obj)\\' -and
        (-not $resolverSourceFullPath -or $_.FullName -ne $resolverSourceFullPath)
    }

$hasObfuscatedCalls = $false
foreach ($sourceFile in $sourceFiles) {
    if ([IO.File]::ReadAllText($sourceFile.FullName).Contains($ResolverCallPrefix, [StringComparison]::Ordinal)) {
        $hasObfuscatedCalls = $true
        break
    }
}
if (-not $hasObfuscatedCalls) {
    Write-Host 'No obfuscated string calls were found.'
    exit 0
}

$bindingFlags = [Reflection.BindingFlags]'Static,Instance,Public,NonPublic'
$loadContext = [Runtime.Loader.AssemblyLoadContext]::Default
$resolveHandler = [System.Func[Runtime.Loader.AssemblyLoadContext, Reflection.AssemblyName, Reflection.Assembly]] {
    param($context, $name)

    $candidate = Join-Path $assemblyDirectory ($name.Name + '.dll')
    if (Test-Path -LiteralPath $candidate) {
        return $context.LoadFromAssemblyPath($candidate)
    }
    return $null
}

$loadContext.add_Resolving($resolveHandler)
try {
    $assembly = $loadContext.LoadFromAssemblyPath($AssemblyPath)
    $resolverType = $assembly.GetType($resolverTypeName, $true)
    $moduleType = $assembly.GetType($ModuleReflectionTypeName, $true)
    $moduleHolder = $moduleType.GetField($moduleHolderName, $bindingFlags).GetValue($null)
    $resolveMethod = $resolverType.GetMethod($ResolverMethodName, $bindingFlags)

    # Reflection is intentional here. Skip the obfuscator's caller-assembly guard.
    if ($CallerGuardFieldName) {
        $resolverType.GetField($CallerGuardFieldName, $bindingFlags).SetValue($null, 80)
    }

    $fieldValues = @{}
    $resolvedOffsets = @{}
    $fileUpdates = [Collections.Generic.List[object]]::new()
    $mapRows = [Collections.Generic.List[object]]::new()

    foreach ($sourceFile in $sourceFiles) {
        $source = Read-SourceFile -Path $sourceFile.FullName
        $calls = @(Find-Calls -Path $sourceFile.FullName -Text $source.Text)
        if ($calls.Count -eq 0) {
            continue
        }

        foreach ($call in $calls) {
            if (-not $fieldValues.ContainsKey($call.FieldName)) {
                $field = $moduleType.GetField($call.FieldName, $bindingFlags)
                if (-not $field) {
                    throw "Module field was not found: $($call.FieldName)"
                }
                $fieldValues[$call.FieldName] = [int]$field.GetValue($moduleHolder)
            }

            [uint32]$offsetBits = ConvertInt32To-UInt32Bits -Value $fieldValues[$call.FieldName]
            foreach ($literal in $call.NumberLiterals) {
                $offsetBits = [uint32]($offsetBits -bxor (ConvertTo-UInt32Bits -Literal $literal))
            }
            $offset = [BitConverter]::ToInt32([BitConverter]::GetBytes($offsetBits), 0)
            if ($offset -lt 0) {
                throw "Resolved a negative string-table offset in $($sourceFile.FullName): $($call.Expression)"
            }

            if (-not $resolvedOffsets.ContainsKey($offset)) {
                $resolvedOffsets[$offset] = [string]$resolveMethod.Invoke($null, [object[]]@($offset))
            }
            $value = $resolvedOffsets[$offset]
            $call | Add-Member -NotePropertyName Offset -NotePropertyValue $offset
            $call | Add-Member -NotePropertyName Value -NotePropertyValue $value
            $call | Add-Member -NotePropertyName Literal -NotePropertyValue (ConvertTo-CSharpStringLiteral -Value $value)

            $mapRows.Add([pscustomobject]@{
                    File = [IO.Path]::GetRelativePath($ProjectRoot, $sourceFile.FullName)
                    Offset = $offset
                    Expression = $call.Expression
                    Value = $value
                })
        }

        $fileUpdates.Add([pscustomobject]@{
                Path = $sourceFile.FullName
                Text = $source.Text
                Encoding = $source.Encoding
                Calls = $calls
            })
    }

    if ($mapRows.Count -eq 0) {
        Write-Host 'No obfuscated string calls were found.'
        exit 0
    }

    if ($Apply) {
        foreach ($fileUpdate in $fileUpdates) {
            $builder = [Text.StringBuilder]::new($fileUpdate.Text)
            foreach ($call in @($fileUpdate.Calls | Sort-Object Start -Descending)) {
                [void]$builder.Remove($call.Start, $call.End - $call.Start)
                [void]$builder.Insert($call.Start, $call.Literal)
            }
            [IO.File]::WriteAllText($fileUpdate.Path, $builder.ToString(), $fileUpdate.Encoding)
        }

        $mapDirectory = Split-Path -Parent $MapPath
        if ($mapDirectory -and -not (Test-Path -LiteralPath $mapDirectory)) {
            [void](New-Item -ItemType Directory -Path $mapDirectory)
        }
        $mapRows | Sort-Object File, Offset | Export-Csv -LiteralPath $MapPath -NoTypeInformation -Encoding utf8
    }

    [pscustomobject]@{
        Files = $fileUpdates.Count
        Calls = $mapRows.Count
        UniqueOffsets = $resolvedOffsets.Count
        UniqueModuleFields = $fieldValues.Count
        Applied = [bool]$Apply
        MapPath = if ($Apply) { $MapPath } else { $null }
    } | Format-List
}
finally {
    $loadContext.remove_Resolving($resolveHandler)
}
