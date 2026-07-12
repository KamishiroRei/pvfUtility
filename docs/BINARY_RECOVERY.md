# Binary recovery audit

This audit covers `pvfUtility-recovered` source inputs. Generated `bin`, `obj`,
and `SourceLibraries/.build` files are excluded from inventory counts.

## Managed assemblies

The vendored `lib` directory contains 274 DLL files:

| Category | Count | Recovery decision |
| --- | ---: | --- |
| Managed assemblies | 267 | Audited through assembly metadata and `pvfUtility.deps.json`. |
| Native DLLs | 7 | Cannot be converted to C#; retained as runtime dependencies. |
| Top-level managed DLLs | 184 | Includes application libraries and external dependencies. |
| Satellite resource DLLs | 83 | Ten UnitComboLib satellites were restored to RESX; external satellites remain vendored. |

Twenty-two managed assemblies now have buildable projects under
`SourceLibraries`:

| Project | Target | Result | Recovery notes |
| --- | --- | --- | --- |
| `GMTool.Dot` | `net10.0` | Builds | Internal dependencies use source projects. |
| `GMTool.Services` | `net10.0` | Builds | Internal dependencies use source projects. |
| `GMTool.SqlModel` | `net10.0` | Builds | Internal dependencies use source projects. |
| `HL` | `net10.0-windows` | Builds | 42 XML/XSHD/XSHDT resources retained as source. |
| `ICSharpCode.AvalonEdit` | `net10.0-windows` | Builds | 266 C# and six XAML files; original strong-name token restored. |
| `PvfCode.Dot` | `net10.0` | Builds | Internal dependencies use source projects. |
| `PvfCode.LoggerBase` | `net10.0-windows` | Builds | WPF target restored. |
| `PvfCode.Models` | `net10.0-windows` | Builds | 982 encrypted strings inlined; data stream removed. |
| `PvfCode.NPK.Utils` | `net10.0-windows` | Builds | Windows drawing target restored. |
| `PvfCode.Services` | `net10.0-windows` | Builds | 2,040 encrypted strings inlined; data stream removed. |
| `pvfUtility.WebApi.Dto` | `net10.0` | Builds | Framework-only DTO project. |
| `ServiceLocator` | `net10.0-windows` | Builds | References recovered `HL` and AvalonEdit source. |
| `Settings` | `net10.0-windows` | Builds | References recovered `SettingsModel` source. |
| `SettingsModel` | `net10.0` | Builds | Portable package reference replaces absolute framework path. |
| `SevenZipSharp` | `net10.0` | Builds | Upstream library plus pvfUtility-specific `SevenZipHelper`; strong name restored. |
| `Swordfish.NET.CollectionsV3` | `net10.0` | Builds | 50 C# files; public API and strong-name token match the release DLL. |
| `TextEditLib` | `net10.0-windows` | Builds | Seven BAML files restored to XAML. |
| `UnitComboLib` | `net10.0-windows` | Builds | Four BAML files restored to XAML; ten satellite RESX files restored. |
| `Utools` | `net10.0` | Builds | 518 encrypted strings inlined; anonymous types and data stream removed. |
| `Vulild.Ionic.Zlib` | `net10.0` | Builds | 30 C# files; compression and CRC round-trip checks pass. |
| `Whetstone.ChatGPT` | `net10.0` | Builds | Restored configurable OpenAI-compatible base URI, client ownership, cancellation, bounded responses, safe errors, and function-tool DTOs; nine focused regressions pass. |
| `WpfRangeControls` | `net10.0-windows` | Builds | One BAML file restored to XAML. |

The solution contains 1,394 C# files, 18 XAML files, 16 RESX files, and zero
loose BAML files. A clean solution build completes with zero errors. Remaining
warnings are decompiler nullable annotations, obsolete APIs, Windows platform
analysis, unused recovered fields, and DevExpress framework-version conflicts.

## String data streams

Three opaque embedded resources were runtime string tables rather than business
assets. Reflection against the original assemblies recovered all call sites:

| Assembly | Calls | Unique offsets | Map |
| --- | ---: | ---: | --- |
| `Utools` | 518 | 255 | `RECOVERED_UTOOLS_STRING_MAP.csv` |
| `PvfCode.Models` | 982 | 744 | `RECOVERED_PVFCODE_MODELS_STRING_MAP.csv` |
| `PvfCode.Services` | 2,040 | 1,042 | `RECOVERED_PVFCODE_SERVICES_STRING_MAP.csv` |

After replacement, the three binary streams, decoder classes, six module files,
and stale resolver imports were deleted. No recovered source project reads those
resources at runtime.

## WPF resources

The main application still requires
`Resources/pvfUtility.g.resources`. It contains 126 BAML entries and
121 non-BAML entries (88 SVG, 32 PNG, and 1 TTF). Every non-BAML entry has a
byte-identical loose source asset, and every BAML entry has readable recovered
XAML, but the container remains the only exact runtime representation of the 126
compiled views.

A full source-XAML compile probe was performed without the container:

| Probe result | Count |
| --- | ---: |
| XAML documents | 126 |
| Markup compilation failures | 77 |
| Passed initial markup stage | 49 |
| `MC3057` | 33 |
| `MC3072` | 32 |
| `MC3074` | 7 |
| `MC4102` | 3 |
| `MC3066` | 2 |

Failures come from decompiler pseudo-elements such as `Ctor`/`DefTag`, incorrect
attached-property owners, lost type information, and unknown connection IDs.
Recompiling those files would also conflict with recovered
`InitializeComponent`/`IComponentConnector` code. The readable XAML is therefore
kept as source/reference while runtime loading uses the exact original BAML.

Dependency-library BAML had a smaller and safer surface. All 12 files were
converted to XAML and compile successfully in their recovered projects.

## Retained binary data

The following binary inputs are intentional and cannot be represented as C# or
XAML without changing their format or behavior:

- `TextEditLib`: seven multi-resolution ICO toolbar assets.
- `SevenZipSharp`: fourteen SFX executable modules, eleven archive capability
  probe files, one embedded 7-Zip byte payload in RESX, and the upstream SNK used
  to reproduce public key token `c8ff6ba0184838bb`.
- `PvfCode.Models`: one default application-layout byte array stored in RESX.
- Main application assets: PNG/ICO/TTF files. SVG files are retained as readable
  XML source.
- `pvfUtility.g.resources`: exact main WPF BAML container, for the reasons above.

The SevenZipSharp SFX and archive files are runtime inputs, not unused test
debris. `SevenZipSfx` loads the SFX modules, while `SevenZipLibraryManager` uses
the sample archives to probe native 7-Zip format support.

## Native dependencies

These seven PE files contain native machine code and cannot be recovered as C#:

```text
7z.dll
7z64.dll
e_sqlite3.dll
Microsoft.Data.SqlClient.SNI.dll
sni.dll
WebView2Loader.dll
runtimes\win-x64\native\WebView2Loader.dll
```

They remain under `lib` so the copied project can run without paths outside
`pvfUtility-recovered`.

## External managed dependencies

The remaining managed DLLs are Microsoft/.NET components, 82 DevExpress
assemblies, documented NuGet/open-source libraries, and their satellite
resources. They are external dependencies rather than missing pvfUtility source.
DevExpress assemblies are commercial binaries and were not copied into recovered
source projects.

Metadata and dependency-graph inspection found no additional hidden
`PvfCode.*`, `GMTool.*`, or `pvfUtility.*` assemblies. The final local `project`
entries in the packaged dependency graph were `AvalonEdit`,
`Swordfish.NET.CollectionsV3`, and `Vulild.Ionic.Zlib`; all three now have source
projects. No dependency marked as a local project remains binary-only.

## Main executable source substitution

`Directory.Build.targets` exposes `Binary`, `Leaf`, `Core`, and `All` dependency
modes. `All` is the default and replaces 20 packaged local DLLs with the matching
projects under `SourceLibraries`. `Settings` and `SettingsModel` are
the only two recovered projects not required by the main executable. Same-named
fallback DLLs remain in `lib` for `Binary` mode but are excluded from normal build
and publish output when a project supplies that relative path.

The original `PvfCode.Dot`, `PvfCode.Services`, and `Whetstone.ChatGPT` binaries
predate several APIs used by the recovered source application. The main project
defines narrow compatibility symbols whenever a mode selects those legacy DLLs:
missing extended comment fields are left blank in `Binary`/`Leaf`, and
booster-selection content uses the existing generic preview in
`Binary`/`Leaf`/`Core`. Binary AI requests report that a source-library mode is
required because only that mode uses the legacy ChatGPT DLL. `All` retains the
complete behavior, and no second chat protocol stack is compiled for the binary
fallback.

Each source-mode build writes `recovered-source-libraries.txt`. The startup test
uses it to compare every output DLL with its source build using SHA-256, rejects a
hash matching the old `lib` binary, inspects loaded module paths, and then checks
the populated main window and absence of an error window. Manifest entries retain
only the assembly name and project filename, so published output does not expose
the build machine's absolute workspace path. Release builds also enable
deterministic source paths for the emitted assemblies and PDBs. The standard `All`
manifest contains 20 entries, all represented as `type: project` in the generated
`.NET 10` deps file.

Identity and behavior checks for the final three recoveries include:

- AvalonEdit: 292 non-generated types and 33 resources match; public key token
  `9cc39be672370310` is preserved and the rebuilt signature verifies.
- Swordfish Collections: 63 business types and 781 public API entries match;
  public key token `bca74c0d98d5aac3` is preserved and verifies.
- Ionic Zlib: 48 types match; GZip, Deflate, Zlib, parallel-deflate, and CRC32
  round-trip tests pass.

## .NET 10 migration

The main project targets `net10.0-windows` with RID `win-x64`; all recovered
libraries target `net10.0` or `net10.0-windows`. The four extension packages were
updated to version `10.0.0`, and WPF projects use `Microsoft.NET.Sdk` with
`UseWPF=true`.

The old packaged `.NET 6` dependency manifest is retained under `lib` only as a
recovery input and is excluded from output. The .NET 10 SDK generates the active
manifest, whose runtime target is `.NETCoreApp,Version=v10.0/win-x64`.
Compiled-BAML dependencies are explicit MSBuild references, while an
application-directory resolver covers optional providers loaded dynamically by
the vendored release libraries.

DevExpress WPF 24.1 assumes the pre-.NET-10 private wrapper around
`Popup._popupRoot`. A startup compatibility adapter supplies direct-field access
before any DevExpress popup is created. This keeps the commercial assembly
unchanged and removes the main-window `NullReferenceException` observed on the
first .NET 10 run.

## Cleanup and verification

- Removed the shared decompiler informational-version hash and hard-coded
  `Release` configuration metadata from the main assembly and 22 recovered
  libraries. Assembly versions, file versions, names, cultures, and strong-name
  identities remain unchanged.
- Removed the obsolete ILSpy scratch tree after integrating its useful source
  material. Its 35 duplicate binary assets were either byte-identical to the
  formal image/font paths or already represented by the retained string map.
- No source project references `_analysis_extract`, `pvfUtility-old`, the original
  executable, or an absolute filesystem path.
- All ten regenerated UnitComboLib satellite assemblies decompile back to RESX
  files byte-identical to the restored source.
- SevenZipSharp rebuild verification found 647 public/protected API entries and
  29 manifest resources in both original and rebuilt DLLs, with no differences;
  all 29 resource payload hashes match.
- Clean recovered-library and main-application builds complete with 0 errors.
- Two consecutive 15-second UI Automation runs observed a populated main window,
  all required controls, and no error dialog.
- A separate Release publish retained the .NET 10 dependency target, verified all
  20 source-assembly hashes, and passed the same 15-second populated-window check.

Build the recovered library graph with:

```powershell
dotnet restore .\SourceLibraries\pvfUtility.SourceLibraries.sln
dotnet build .\SourceLibraries\pvfUtility.SourceLibraries.sln --no-restore
```
