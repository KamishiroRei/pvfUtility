# Main-program source XAML migration

## Current state

The recovered main program contains 126 readable XAML documents and an immutable
original WPF resource container at `Resources/pvfUtility.g.resources`. The
container has 247 entries: 126 BAML documents and 121 non-BAML resources.

The application now supports a staged source-XAML migration. The first verified
batch source-compiles two documents:

| XAML | BAML key | Hybrid behavior |
| --- | --- | --- |
| `app.xaml` | `app.baml` | Compiled as `Page`; `App.Main`, the startup handler, and `App.InitializeComponent` remain code-owned. |
| `views/viewscripteditor.xaml` | `views/viewscripteditor.baml` | Compiled with `x:Class="PvfCode.Views.ViewScriptEditor"`; WPF generates `InitializeComponent` in Hybrid. |

The other 124 BAML entries and all 121 non-BAML entries still come byte-for-byte
from the original container. A verified Hybrid Release build contains 247 total
entries and 126 BAML entries; only the two keys above differ from the original
resource payload hashes.

This migration does not require a particular Visual Studio release. The build
baseline remains `.NET 10`, `net10.0-windows`, and `win-x64`.

## Compatibility boundaries

- Never modify or regenerate `Resources/pvfUtility.g.resources` in place. It is
  the recovery baseline and the complete `Legacy` rollback source.
- Do not add recovered XAML to WPF compilation outside the explicit
  `RecoveredSourceXaml` allowlist in `Directory.Build.targets`.
- Do not convert `app.xaml` to `ApplicationDefinition`: doing so would generate
  another application entry point and conflict with the recovered `App.Main`.
- `ViewScriptEditor` uses generated WPF code only in Hybrid/SourceOnly. The
  conditional `PvfCode/Views/ViewScriptEditor.Legacy.cs` supplies the recovered
  loader and connector only for `Legacy`.
- A generated overlay may contain only the BAML keys declared by the allowlist.
  Unexpected keys, undeclared duplicates, or missing replacements fail the
  build instead of silently falling back.
- Keep `RecoveredSourceLibraryMode` independent from the WPF resource mode. Its
  default remains `All`.
- Keep trimming and ReadyToRun disabled for WPF, DevExpress, BAML compatibility,
  and reflection-loaded dependencies.

## Build modes

`RecoveredWpfResourceMode` is case-sensitive and accepts exactly three values:

| Mode | Behavior |
| --- | --- |
| `Legacy` | Compiles no recovered main-program XAML and embeds the original container unchanged. Use this for rollback and differential diagnosis. |
| `Hybrid` | Compiles the two allowlisted XAML documents and overlays their BAML on an intermediate copy of the original resource set. This is the default. |
| `SourceOnly` | Uses source-generated WPF resources without legacy supplementation. It intentionally fails until all 126 BAML mappings are allowlisted. |

Examples:

```powershell
.\scripts\Build-SingleFile.ps1 `
  -Configuration Release `
  -RecoveredWpfResourceMode Legacy

.\scripts\Build-SingleFile.ps1 `
  -Configuration Release `
  -RecoveredWpfResourceMode Hybrid

# Guard verification only; this is expected to fail at 2/126.
dotnet build .\pvfUtility.csproj -c Release `
  -p:RecoveredWpfResourceMode=SourceOnly
```

At the current 2/126 migration state, `SourceOnly` must fail before markup
compilation with a message reporting 126 required and 2 migrated entries.

## Hybrid resource pipeline

1. The general recovered-XAML rule classifies all 126 main-program XAML files as
   `None`.
2. `RecoveredSourceXaml` reclassifies only `app.xaml` and
   `views/viewscripteditor.xaml` as WPF `Page` inputs in Hybrid/SourceOnly.
3. WPF writes the source-generated overlay to the intermediate
   `pvfUtility.g.resources`.
4. `tools/PvfResourceMerger` reads the original and overlay containers with
   `ResourceReader.GetResourceData`, retaining raw resource type names and data.
5. The merger accepts only keys in the generated replacement list, copies all
   other original entries unchanged, writes in ordinal key order through a
   temporary file, and atomically moves the successful output into place.
6. MSBuild removes competing resource items and embeds exactly one manifest
   resource with logical name `pvfUtility.g.resources`.

The merger's console regression project covers declared replacement, missing or
unknown overlay keys, undeclared duplicates, duplicate replacement entries,
empty output rejection, raw payload preservation, deterministic output, audit
success/failure behavior, and the CLI contract:

```powershell
dotnet run --project `
  .\tests\PvfResourceMerger.RegressionTests\PvfResourceMerger.RegressionTests.csproj `
  -c Release
```

After a Hybrid Release build, run the repository-level container audit:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass `
  -File .\scripts\Test-RecoveredWpfResourceContainer.ps1 `
  -Configuration Release
```

It invokes the merger's `audit` command against the immutable base, generated
overlay, merged intermediate output, replacement manifest, and final application
assembly. The audit requires 247 total resources, 126 BAML resources, exactly
the two declared changed keys, and one embedded `pvfUtility.g.resources` whose
bytes match the audited merged container.

## Source-XAML semantic self-test

`PvfCode/Compatibility/RecoveredXamlSelfTest.cs` provides a deterministic,
non-interactive check of the first migration batch. `App.Main` runs it only when
`PVFUTILITY_RECOVERED_XAML_SELF_TEST=1`; normal application startup is unchanged.
The test verifies:

- all 11 application merged resource dictionaries load;
- `ViewScriptEditor` creates its `TextEditorBase` child;
- the recovered `AllowCompletion`, `AllowFolding`, and `ContentMargin` values;
- the Title, FontSize, Document, and IsReadOnly bindings, including the two-way
  Document binding.

The single-file build entry runs it automatically for the selected mode. It can
also be rerun explicitly against the default local packages:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass `
  -File .\scripts\Test-RecoveredXamlMigration.ps1 `
  -Configuration Release `
  -RecoveredWpfResourceMode Legacy

powershell -NoProfile -ExecutionPolicy Bypass `
  -File .\scripts\Test-RecoveredXamlMigration.ps1 `
  -Configuration Release `
  -RecoveredWpfResourceMode Hybrid
```

The wrapper writes results through a temporary file, requires exit code zero,
and restores the caller's environment variables. Run it for both Legacy and
Hybrid single-file packages so the original and source-generated BAML paths are
checked against the same semantic contract. This focused test supplements, but
does not replace, the full UI Automation startup test.

## Single-file distribution

`Properties/PublishProfiles/SingleFile.pubxml` defines the common local, CI, and
release package contract:

- self-contained Windows x64;
- one compressed `pvfUtility.exe`;
- native libraries included for self-extraction;
- no trimming or ReadyToRun;
- no standalone PDB files;
- embedded symbols in Debug and no symbols in Release.

Use the repository build script instead of assembling a publish directory by
hand:

```powershell
.\scripts\Build-SingleFile.ps1 `
  -Configuration Release `
  -RecoveredWpfResourceMode Hybrid `
  -RecoveredSourceLibraryMode All
```

The default output is
`artifacts/publish/local/<mode>/<configuration>/win-x64`. The clean package must
contain exactly one executable plus editable/runtime data:

```text
pvfUtility.exe
Options/
Resources/
Defaults/Options/
recovered-source-libraries.txt
```

`scripts/Test-SingleFilePackage.ps1` rejects standalone DLL, PDB, deps, or
runtimeconfig files and checks the external data directories and recovered
source-library manifest. `scripts/Test-GitHubReleasePackage.ps1` remains a
compatibility wrapper over the same package validator.

The pull-request/`master` workflow runs the merger regression project and the
incomplete-SourceOnly guard, then builds, source-XAML-self-tests, and starts both
Legacy and Hybrid single-file packages. Startup runs from a disposable smoke
copy so runtime-generated files cannot enter the uploaded clean artifact. The
tagged Release workflow uses the same Hybrid single-file build, then validates a
clean copied package and the final ZIP extraction before uploading the ZIP and
SHA-256 file.

## Verification and rollback

Every migrated batch must pass:

- the resource-merger regression executable;
- Release builds in both Legacy and Hybrid;
- `scripts/Test-RecoveredWpfResourceContainer.ps1`, proving that only declared
  replacement keys changed and that the final assembly embeds the audited
  container;
- `scripts/Test-RecoveredXamlMigration.ps1` against both Legacy and Hybrid
  outputs;
- `scripts/Test-RecoveredStartup.ps1`, including the 15-second observation,
  error-window rejection, at least 100 UI Automation nodes, and the
  `BarSubItemLinksubFile`, `FilelistLayoutPanel`, and `DocumentHost` controls;
- clean single-file package validation and single-file startup.

Rollback does not require changing source:

```powershell
.\scripts\Build-SingleFile.ps1 `
  -Configuration Release `
  -RecoveredWpfResourceMode Legacy
```

To roll back one future migration, remove that document from
`RecoveredSourceXaml` together with its source-generated code adjustments. Never
silently use an original BAML after a merge validation error.

## Adding another XAML document

Each later migration should be a single reviewable unit:

1. Reconcile the root CLR type, assembly identities, namespaces, pack URIs, and
   decompiler artifacts.
2. Decide explicitly whether initialization remains code-owned or moves to WPF
   generated code. Do not leave both implementations active in one mode.
3. Reconstruct named fields, handlers, commands, templates, and every applicable
   row in `docs/BAML_CONNECTION_ID_AUDIT.csv`.
4. Add the file and its BAML logical name to `RecoveredSourceXaml` only in the
   same change.
5. Prove that the final container still has 247 entries and 126 BAML entries and
   that only the declared replacement set changed.
6. Build and start Legacy and Hybrid packages, then update
   `docs/BAML_XAML_MAP.csv` and this document.

The original container must not be deleted until all 126 mappings pass
SourceOnly, SourceOnly is a required successful CI mode rather than the current
expected-failure guard, and a release candidate has passed the full single-file
startup gate.
