# Recovered source libraries

This directory contains source reconstructed from 22 managed assemblies shipped
with pvfUtility. It is a buildable source solution, not a claim that the original
authoring repositories, comments, symbol names, or project history were recovered.

## Build

From the `pvfUtility-recovered` directory:

```powershell
dotnet restore .\SourceLibraries\pvfUtility.SourceLibraries.sln
dotnet build .\SourceLibraries\pvfUtility.SourceLibraries.sln --no-restore
```

Generated files are isolated under:

```text
SourceLibraries\.build\<ProjectName>\
```

The `.build` directory may be deleted or omitted when copying the source tree.

## Dependency policy

- Dependencies between recovered projects use `ProjectReference`. The solution
  contains 39 source-project edges and no references to the corresponding DLLs
  under `lib`.
- External managed dependencies use exact `Reference` items resolved from the
  project-local `lib` directory.
- Four .NET 10 extension assemblies that are not present in `lib` use explicit
  `PackageReference` entries: `System.Configuration.ConfigurationManager`,
  `System.Diagnostics.PerformanceCounter`, `System.Drawing.Common`, and
  `System.Security.Cryptography.ProtectedData`.
- No project contains an absolute filesystem `HintPath`.

The main `pvfUtility.csproj` now consumes 20 of these projects in the default
`RecoveredSourceLibraryMode=All` configuration. `Settings` and `SettingsModel`
remain independently buildable because they are not part of the executable's
dependency graph. `Binary`, `Leaf`, and `Core` modes remain available for staged
or differential verification. A generated manifest and SHA-256 checks in
`scripts/Test-RecoveredStartup.ps1` verify that application output came from the
source builds rather than same-named fallback DLLs in `lib`.

All projects target `net10.0` or `net10.0-windows`. A clean build with .NET SDK
10.0.300 completes with zero errors. The solution contains 1,394 C# files, 18
buildable XAML files, 16 RESX files, and no loose BAML.

## Recovered resources

- 18 dependency-library BAML documents were converted to buildable XAML: 7 in
  `TextEditLib`, 6 in `ICSharpCode.AvalonEdit`, 4 in `UnitComboLib`, and 1 in
  `WpfRangeControls`.
- 10 `UnitComboLib.resources.dll` satellite assemblies were restored as culture
  `.resx` files. Rebuilding and decompiling the generated satellites reproduces
  all 10 RESX files byte-for-byte.
- 3,540 encrypted string calls in `Utools`, `PvfCode.Models`, and
  `PvfCode.Services` were replaced with C# literals. Their maps are stored under
  the top-level `docs` directory.
- The three encrypted data streams, their decoders, six module-constant files,
  three empty obfuscator initializers, and three generated anonymous-type files
  were removed after their call sites were restored.

## Retained binary assets

`TextEditLib` retains seven multi-resolution ICO assets. `SevenZipSharp` retains
its SFX modules, archive capability probes, embedded 7-Zip payload, and upstream
strong-name key; these are runtime data rather than recoverable C# or XAML. See
`docs/BINARY_RECOVERY.md` for the complete inventory and rationale.

`SevenZipSharp` is primarily upstream third-party code. Its recovered assembly is
included because this release contains a pvfUtility-specific
`SevenZip.SevenZipHelper` extension that depends on `PvfCode.Dot`.

The final three local-project DLLs identified by the packaged dependency graph
are also restored here:

- `ICSharpCode.AvalonEdit`: 266 C# files, six XAML files, 33 recovered resource
  entries, and a verified strong-name token of `9cc39be672370310`.
- `Swordfish.NET.CollectionsV3`: 50 C# files, 781 public API entries matching the
  release DLL, and a verified strong-name token of `bca74c0d98d5aac3`.
- `Vulild.Ionic.Zlib`: 30 C# files, complete GZip/Deflate/Zlib/CRC round-trip
  checks, and no binary resource dependency.

## Source normalization status

The recovered source is being normalized incrementally because compiler-expanded
closures and obfuscated private names cannot be removed safely with a global text
replacement. The following service-layer areas have been semantically cleaned and
build-verified:

- `PvfParsingNew` parser state, section models, table formatters, custom section
  formatters, resource access, and Web API data models.
- `SearchService`, `ImagePack2Service`, and `PvfExtensionHelper`.
- `ServiceConvertChinaPlusPvf`, `ServiceCloud`, and `ServiceBatchOperation`.
- `ServiceExtractFiles`, including its export/compression worker closures,
  progress reporting, concurrency control, and error paths.
- `BoosterInfo` and the title-book model (`TitleBook`,
  `TitleCollectionInfoItem`, `TitleCollectionInfo`, and
  `TitleCollectionInfoHead`).
- The `PvfCode.Models` option, PVF, string-table, preview, NPC-shop, and stackable
  models selected by the `System.Runtime.CompilerServices` audit.
- The remaining `PvfCode.Services` parsing, import, item-code, release, and binary
  ANI compiler internals selected by that audit.
- All 13 matching `Utools` files, including the observable dictionaries,
  scheduler, system-information helpers, and time formatting helpers.
- The three matching main-application view models: `LineGuideLines`,
  `ToolTipViewModel_ItemCodeHoverTooltip`, and `WinShopManagerVm`.

Except for one already-committed `StringView` interpolation-handler residual
recorded in `docs/OBFUSCATED_NAME_MAP.md`, these files no longer contain explicit
display-class source types, pseudo `[SpecialName]` accessors, expanded
interpolated-string handlers, or invalid-IL decompiler comments. Private fields
and helpers now use behavior-based semantic names.

The targeted `using System.Runtime.CompilerServices;` audit is complete. Of the
remaining 31 matching files, 19 are assembly metadata, seven are generated
resource/localization accessors, one is generated settings code, one is a module
initializer, one is a `CallerMemberName` helper, and two are normal handwritten
or upstream-library implementations (`PvfSkillClassifier` and AvalonEdit's
`EmptySelection`). These files retain the using because it is required by their
generated contract or runtime implementation; they contain no unresolved private
obfuscation selected by this audit.

Normalization is not complete across all 1,394 C# files. Model libraries and
recovered UI dependencies still retain expanded auto-property accessors,
compiler closures, and private obfuscated members. Those areas should continue in
small build-verified batches. Public or resource-visible CLR names must remain
unchanged until BAML, reflection, serialization, and assembly consumers have been
audited.
