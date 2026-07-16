# pvfUtility recovered source

This directory contains source reconstructed from `pvfUtility.exe`. It is a
self-contained recovery project, not the original authoring repository. Original
comments, source-control history, symbols removed by obfuscation, and most
design-time metadata cannot be recovered exactly from the published executable.

## Project layout

- `pvfUtility.sln`: primary IDE solution containing the main application,
  all 22 recovered source-library projects, the Hybrid resource merger, and its
  regression project. VS Code is configured to open this solution instead of
  scanning sibling recovery and analysis directories.
- `pvfUtility.csproj`: buildable WPF project targeting `.NET 10 for Windows x64`.
- `global.json`: selects a stable .NET 10 SDK feature band without depending on
  an absolute SDK installation path.
- `Directory.Build.targets`: recovered resource, local dependency-copy, and
  compiled-BAML dependency integration used by both build and publish.
- `tools/PvfResourceMerger`: deterministic raw-resource merger and final
  assembly-container auditor required by Hybrid builds.
- `tests/PvfResourceMerger.RegressionTests`: console regression coverage for
  merge, validation, determinism, audit, and CLI behavior.
- `lib`: vendored external managed assemblies, native libraries, satellite
  resources, optional runtime files, and binary-mode fallbacks extracted from the
  application package. In the default source mode, DLLs supplied by recovered
  projects are excluded from output in favor of their project build products.
- `SourceLibraries`: a separate solution containing source recovered
  from 22 managed assemblies, including all private PvfCode/GMTool libraries,
  AvalonEdit, Swordfish Collections, Ionic Zlib, and the locally modified
  SevenZipSharp assembly. Internal dependencies use
  `ProjectReference`; external binary references resolve only from local `lib`.
- `Resources/pvfUtility.g.resources`: original compiled WPF resource
  container and immutable recovery baseline. Its 126 BAML entries preserve the
  pack URIs used by `Application.LoadComponent`; Hybrid builds replace only the
  explicitly migrated entries in an intermediate merged container.
- `*.xaml`: 126 readable XAML files restored to their logical source paths. They
  remain source/reference files by default. `app.xaml` and
  `views/viewscripteditor.xaml` are the first source-compiled Hybrid entries.
- `images`, `styles`, `themes`, `iconfont`: recovered application resources.
- The obsolete ILSpy scratch tree was removed after its useful source and
  resource material had been integrated into the canonical project paths.
- `docs/BAML_RECOVERY.md`, `docs/BAML_XAML_MAP.csv`, and
  `docs/BAML_CONNECTION_ID_AUDIT.csv`: BAML/XAML recovery method, the complete
  one-to-one resource mapping, and the connection-ID diagnostics removed from
  readable XAML.
- `docs/OBFUSCATED_NAME_MAP.md`: audited CLR-name recovery status. Safe internal
  types have semantic CLR names; only three BAML-referenced identities remain
  obfuscated for compatibility.
- `docs/RECOVERED_STRING_MAP.csv`: audit map for every encrypted-string call that
  was replaced with a C# string literal.
- `docs/RECOVERED_UTOOLS_STRING_MAP.csv`,
  `docs/RECOVERED_PVFCODE_MODELS_STRING_MAP.csv`, and
  `docs/RECOVERED_PVFCODE_SERVICES_STRING_MAP.csv`: 3,540 additional string
  recoveries from managed dependency assemblies.
- `docs/BINARY_RECOVERY.md`: managed/native DLL classification, source-project
  status, retained binary-resource rationale, and verification results.
- `docs/NET10_MIGRATION.md`: target-framework migration, runtime compatibility
  work, dependency-manifest changes, and clean verification results.
- `scripts/Test-RecoveredStartup.ps1`: UI smoke test for the populated main window; publish inputs are copied to a disposable smoke directory before launch.
- `scripts/Test-RecoveredWpfResourceContainer.ps1`: verifies the 247/126
  resource counts, declared replacement set, and embedded assembly container.
- `scripts/Test-RecoveredXamlMigration.ps1`: focused semantic self-test for the
  first two source-compiled XAML documents in Legacy and Hybrid outputs.
- `scripts/Test-AiAssistantDocking.ps1`: verifies from a disposable package copy that the AI panel starts hidden, then opens at the far right and receives input focus from the toolbar.
- `scripts/Inline-ObfuscatedStrings.ps1`: recovery utility used to produce the
  string map from the pre-inlining assembly.
- `scripts/Recover-SourceLibraryStrings.ps1`: reproducible wrapper for the three
  recovered library string tables.

## Standalone use

The project has no path dependency on the original executable, `pvfUtility-old`,
any workspace-level extraction directory, or DLLs outside this directory. Copying
the complete `pvfUtility-recovered` directory is sufficient. Generated `bin` and
`obj` directories and `SourceLibraries/.build` may be omitted.

Required environment:

- Windows x64.
- A stable .NET 10 SDK. `global.json` accepts .NET 10 feature bands at or above
  `10.0.100`; this recovery was verified with SDK `10.0.300`.
- NuGet access on the first restore. The main project has two standard package
  references; the recovered library solution has four .NET 10 extension-package
  references for assemblies not shipped under `lib`.

Create the local runnable package from the project directory:

```powershell
.\scripts\Build-SingleFile.ps1
```

This is the same compressed, self-contained Windows x64 single-file pipeline
used by CI and GitHub Release. Its default output is:

```text
artifacts\publish\local\Hybrid\Debug\win-x64\pvfUtility.exe
```

Ordinary solution builds remain available for compiler and IDE checks, but their
`bin` output is not a runnable or distributable project deliverable:

```powershell
dotnet restore .\pvfUtility.sln
dotnet build .\pvfUtility.sln --no-restore
```

Validate that the actual WPF window loads without an error dialog:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass `
  -File .\scripts\Test-RecoveredStartup.ps1 `
  -Configuration Debug `
  -OutputDirectory .\artifacts\publish\local\Hybrid\Debug\win-x64 `
  -SingleFile
```

The test checks the `pvfUtility` main-window title, a populated visual tree, and
key controls including `BarSubItemLinksubFile`, `FilelistLayoutPanel`, and
`DocumentHost`.

## Offline data, optional AI, and BAML compatibility

Account operations, cloud backup, shared uploads, remote stores, automatic
updates, remote start content, and exception telemetry remain intentionally
offline. The ChatGPT compatibility entry is the single opt-in network
exception: it now opens a PVF assistant in the full-height docked panel at the
far right. Runtime state is otherwise stored as readable,
unencrypted JSON:

- `Options/AppConfig.json` contains application settings and resource-tree
  comments.
- `Options/AiAssistant.json` contains the assistant endpoint, model, and output
  limit, but never the API key.
- `Options/Bookmarks.json` contains the local bookmark tree.
- `Options/PvfComments/<extension>.json` contains PVF tag comments isolated by
  file extension. Tag documentation supports Markdown and the `Title` and
  `OfficialDescription` fields.

Initial copies are tracked under `Resources/OfflineDefaults/Options`. The
application does not import the former binary, encrypted, XML bookmark, or
SQLite comment formats. Runtime `Options` is user data and is excluded from Git.

The original BAML schema still names several account, store, cloud-backup, and
ChatGPT CLR types and deferred template properties. These compatibility members
remain so WPF can deserialize that schema. Cloud GET/POST transport still
returns an offline error, and update and telemetry assemblies remain excluded.
Only the original ChatGPT toolbar binding is deliberately enabled: its API key
is session-only or supplied by `OPENAI_API_KEY`, and access to the open PVF is
disabled until the user explicitly enables read-only tools. Removing the BAML
compatibility members can still surface as an unhelpful
`WpfXamlLoader.TransformNodes` null reference.

Build all recovered managed libraries from source:

```powershell
dotnet restore .\SourceLibraries\pvfUtility.SourceLibraries.sln
dotnet build .\SourceLibraries\pvfUtility.SourceLibraries.sln --no-restore
```

The 22-project library solution contains 39 `ProjectReference` edges, no absolute
`HintPath`, and no binary references to another recovered project. Its generated
files are isolated under `SourceLibraries/.build` and may be omitted
when copying the project.

## Recovered XAML

All 126 loose `.baml` files were matched one-to-one with the XAML recovered by
ILSpy. Each loose file was also compared with the corresponding entry in
`Resources/pvfUtility.g.resources`; all 126 pairs were byte-identical.

The duplicate loose BAML files and their 252 project-item declarations were
therefore removed. The readable XAML files now occupy the original logical paths,
including `app.xaml`, `mainwindow.xaml`, control views, styles, and themes.

The recovered XAML is well-formed XML. The main build first marks all 126 files
as `None`, because BAML decompilation loses some connection IDs and generated
code relationships. An explicit Hybrid allowlist currently re-enables only
`app.xaml` and `views/viewscripteditor.xaml` as WPF `Page` items. Their generated
BAML replaces the two matching keys in an intermediate resource container; the
other 124 BAML and all 121 non-BAML resources retain their original raw data.
`Legacy` remains a complete original-BAML rollback mode, while `SourceOnly`
fails until all 126 mappings are migrated. See `docs/BAML_RECOVERY.md` and
`docs/SOURCE_XAML_MIGRATION.md` for the technical details.

ILSpy emitted 270 `Unknown connection ID` diagnostic comments across 74
main-application XAML documents. Those non-executable comments have been removed
from the readable XAML and recorded in `docs/BAML_CONNECTION_ID_AUDIT.csv` with
their source path, pre-cleanup line number, connection ID, and available context.
No such diagnostic remains in source XAML. Moving the diagnostics to an audit
file does not reconstruct connector metadata or make the main-application XAML
safe to compile.

## Encrypted strings

The published assembly stored application strings in an opaque embedded resource.
All 2,389 call sites were evaluated against the recovered module constants and
replaced with correctly escaped C# literals. The result contains 1,531 unique
string-table offsets. The following runtime-only artifacts were then removed:

- The opaque encrypted resource file.
- `ObfuscatedStringResolver.cs`.
- The module-constant source used only to calculate string-table offsets.
- All stale `using` directives for the removed resolver namespace.

The project and generated executable therefore no longer depend on that encrypted
resource or its obfuscation-time string resolver.

The same process was applied to dependency assemblies:

- `Utools`: 518 calls, 255 unique offsets.
- `PvfCode.Models`: 982 calls, 744 unique offsets.
- `PvfCode.Services`: 2,040 calls, 1,042 unique offsets.

Their three opaque resources, decoder classes, and six module-constant source
files were removed. The recovered library projects also no longer depend on
those runtime string tables.

## Runtime dependencies

.NET 10 now generates `pvfUtility.deps.json` normally. The recovery build no
longer overwrites it with the packaged .NET 6 manifest. Assemblies referenced
only by compiled BAML are declared explicitly, including
`DevExpress.Xpf.Controls.v24.1`, `DevExpress.Xpf.PropertyGrid.v24.1`,
`DevExpress.Xpf.TypedStyles.v24.1`, and the recovered DevExpress theme
assemblies. The generated runtime target is
`.NETCoreApp,Version=v10.0/win-x64`.

Some vendored libraries load optional providers by name at runtime rather than
through a compile-time reference. `RecoveredAssemblyResolver` handles only a
failed normal load and then probes the application directory for a matching
managed assembly identity. This preserves those release-time extension points
without depending on the old dependency manifest or any directory outside the
copied project.

DevExpress WPF 24.1 reflects the old WPF private representation of
`Popup._popupRoot`. .NET 10 exposes that field directly, which caused the main
window to fail during BAML loading. `DevExpressNet10Compatibility` installs a
targeted direct-field adapter before any popup is created; the vendored
commercial DLL itself is not modified.

`lib/7z64.dll` is copied to the output root with the remaining runtime files,
which preserves the location expected by SevenZipSharp.

The main project now supports `Binary`, `Leaf`, `Core`, and `All` recovered-source
modes through `Directory.Build.targets`; `All` is the default. It wires 20 of the
22 recovered projects into the actual application dependency graph. `Settings`
and `SettingsModel` remain independently buildable but are not dependencies of
the main executable. Assembly names, versions, and public-key tokens were checked
against the release DLLs. The startup verifier reads
`recovered-source-libraries.txt`, compares all 20 output hashes with the matching
`.build` products, checks loaded-module paths, and then performs the populated
main-window/error-window assertions. The manifest stores project filenames rather
than absolute paths, and Release builds map source paths deterministically so the
published manifest, assemblies, and PDBs do not expose the build workspace.

## Recovery provenance

The executable package was extracted and decompiled with ILSpy 10.1. The recovery
workflow was equivalent to:

```powershell
ilspycmd --dump-package -o <package-dir> .\pvfUtility.exe
ilspycmd -p --nested-directories --no-dead-code --no-dead-stores `
  -r <package-dir> `
  -o <recovered-source-dir> `
  <package-dir>\pvfUtility.dll
ilspycmd --resource pvfUtility.g.resources `
  -o <resource-dir> `
  <package-dir>\pvfUtility.dll
```

The 41 source files originally emitted under random-looking top-level directories
were assigned semantic physical file names and moved into the recovered business
structure. Those names are inferred descriptions, not claims about the author's
original identifiers. A later IL/BAML audit also restored semantic CLR identities
for safe internal types and removed unreachable ones. Only three random CLR
identities remain, all because their names are present in the original BAML; see
`docs/OBFUSCATED_NAME_MAP.md`.

The two decompiled compiler-generated anonymous-type files were removed after
their call sites were restored to normal LINQ anonymous objects. The unreferenced
empty module-type file was also removed.

The project currently builds with zero compiler errors. Remaining warnings are
decompiler-oriented, primarily nullable annotations, unused generated fields, and
async calls whose original discard/await intent cannot be proven without symbols.

The managed dependency audit additionally recovered 22 source projects with
1,394 C# files, converted all 18 dependency-library BAML files to buildable XAML,
restored ten UnitComboLib satellite RESX files, and identified the seven native
DLLs that cannot be converted to managed source. Clean .NET 10 builds complete
with zero errors, and the populated main window passed repeated Debug and Release
15-second UI startup checks with recovered-assembly hash validation. See
`docs/BINARY_RECOVERY.md` and `docs/NET10_MIGRATION.md`.
