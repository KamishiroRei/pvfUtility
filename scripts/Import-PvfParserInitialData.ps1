param(
    [string]$SourceRoot = 'G:\dnfsifu\develop\pvf-parser-ts'
)

$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path -Parent $PSScriptRoot
$targetRoot = Join-Path $projectRoot 'Resources\OfflineDefaults\Options'
$commentsTarget = Join-Path $targetRoot 'PvfComments'
$treeSource = Join-Path $SourceRoot 'src\config\pvf\treeComments.json'
$bookmarksSource = Join-Path $SourceRoot 'src\config\pvf\bookmarks.json'
$tagsSource = Join-Path $SourceRoot 'src\config\scriptLang\scriptTags'

foreach ($required in @($treeSource, $bookmarksSource, $tagsSource)) {
    if (-not (Test-Path -LiteralPath $required)) {
        throw "Missing pvf-parser-ts source: $required"
    }
}

New-Item -ItemType Directory -Force -Path $commentsTarget | Out-Null

$tree = [IO.File]::ReadAllText($treeSource, [Text.Encoding]::UTF8) | ConvertFrom-Json
$treeComments = [ordered]@{}
foreach ($entry in $tree.comments.PSObject.Properties) {
    $path = ([string]$entry.Name).Replace('\', '/').Trim('/').ToLowerInvariant()
    $value = $entry.Value
    if ($value -is [string]) {
        $comment = $value
        $detailedComment = $null
    } else {
        $comment = $value.comment
        $detailedComment = $value.detailedComment
    }
    $treeComments[$path] = [ordered]@{
        FilePath = $path
        Comment = $comment
        DetailedComment = $detailedComment
    }
}
$appConfig = [ordered]@{
    FirstTime = $false
    PvfConfig = [ordered]@{ TreelistCommentDic = $treeComments }
}
$appConfig | ConvertTo-Json -Depth 30 | Set-Content -LiteralPath (Join-Path $targetRoot 'AppConfig.json') -Encoding utf8

function Convert-BookmarkNode($node, [int]$sort) {
    $children = [ordered]@{}
    $childSort = 0
    foreach ($child in @($node.children)) {
        if ($null -eq $child) { continue }
        $children[[string]$child.label] = Convert-BookmarkNode $child $childSort
        $childSort++
    }
    return [ordered]@{
        FilePath = if ($node.path) { [string]$node.path } else { '' }
        IsFile = [bool]$node.path
        Sort = $sort
        Children = $children
    }
}

$bookmarks = [IO.File]::ReadAllText($bookmarksSource, [Text.Encoding]::UTF8) | ConvertFrom-Json
$myBookmarksLabel = -join @([char]0x6211, [char]0x7684, [char]0x4E66, [char]0x7B7E)
$rootChildren = [ordered]@{}
$sort = 0
foreach ($root in @($bookmarks.roots)) {
    $rootChildren[[string]$root.label] = Convert-BookmarkNode $root $sort
    $sort++
}
$bookmarkGroup = [ordered]@{
    Trees = [ordered]@{
        $myBookmarksLabel = [ordered]@{ FilePath = ''; IsFile = $false; Sort = 0; Children = $rootChildren }
    }
    IsShare = $false
    Title = 'Local bookmarks'
    Instructions = 'Initial data imported from pvf-parser-ts'
}
$bookmarkGroup | ConvertTo-Json -Depth 50 | Set-Content -LiteralPath (Join-Path $targetRoot 'Bookmarks.json') -Encoding utf8

$validSuffixes = [System.Collections.Generic.HashSet[string]]::new([StringComparer]::OrdinalIgnoreCase)
@('equ','ani','act','stk','ptl','key','dgn','tbl','ai','atk','map','mob','obj','gdata','qst','skl','ui','als','txt','til','msn','aic','shp','wrd','npc','apd','etc','rep','co','nut','cre','lst','evt','emo','dat','chi','jap','kor','log','exj','cmb','twn','ora','sd','str','rgn','chr','bt','lay','wdm','mm','cbt','info','vm','evn','img','bm','stm','hsp','pet','xml','bak','glist','bin','dl','blu','tlk','pos') | ForEach-Object { [void]$validSuffixes.Add($_) }

Get-ChildItem -LiteralPath $commentsTarget -Filter '*.json' -File | Remove-Item -Force
foreach ($file in Get-ChildItem -LiteralPath $tagsSource -Filter '*.json' -File) {
    $suffix = $file.BaseName.ToLowerInvariant()
    if (-not $validSuffixes.Contains($suffix)) { continue }
    $source = [IO.File]::ReadAllText($file.FullName, [Text.Encoding]::UTF8) | ConvertFrom-Json
    $seenTags = @{}
    $comments = foreach ($tag in @($source.tags)) {
        if (-not $tag.name) { continue }
        $tagKey = ([string]$tag.name).Trim().ToLowerInvariant()
        if ($seenTags.ContainsKey($tagKey)) { continue }
        $seenTags[$tagKey] = $true
        [ordered]@{
            PvfCommentType = 'Section'
            FileType = $suffix
            Section = [string]$tag.name
            Title = $tag.title
            Comment = if ($tag.description) { [string]$tag.description } elseif ($tag.officialDescription) { [string]$tag.officialDescription } else { '' }
            OfficialDescription = $tag.officialDescription
            Closing = [bool]$tag.closing
            Authors = $tag.authors
        }
    }
    $output = [ordered]@{ SchemaVersion = 1; FileType = $suffix; Comments = @($comments) }
    $output | ConvertTo-Json -Depth 30 | Set-Content -LiteralPath (Join-Path $commentsTarget ($suffix + '.json')) -Encoding utf8
}

Write-Host "Imported defaults into $targetRoot"
