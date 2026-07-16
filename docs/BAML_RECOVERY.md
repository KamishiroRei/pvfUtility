# BAML and XAML recovery

The published application contained 126 compiled WPF BAML resources. ILSpy
recovered a readable XAML document for every BAML logical name.

## Verification

- Loose BAML files found in the recovered source tree: 126.
- BAML entries in `Resources/pvfUtility.g.resources`: 126.
- Loose files byte-identical to their container entries: 126.
- Recovered XAML files mapped uniquely to BAML logical names: 126.
- Missing mappings: 0.
- Ambiguous or duplicate mappings: 0.
- Recovered XAML documents that are well-formed XML: 126.
- Non-generated XAML documents parsed as well-formed XML: 144 (126 main
  application documents plus 18 recovered-library documents).
- ILSpy `Unknown connection ID` diagnostics externalized: 270 rows from 74 main
  application documents; diagnostics remaining in source XAML: 0.
- Default Hybrid migration entries: 2 (`app.baml` and
  `views/viewscripteditor.baml`).
- Verified Hybrid container: 247 total entries, 126 BAML entries, 2 changed raw
  payload hashes, and 245 entries identical to the original container.

The complete resource mapping is stored in `docs/BAML_XAML_MAP.csv`. The removed
connection-ID diagnostics are stored in `docs/BAML_CONNECTION_ID_AUDIT.csv` with
the columns `Path`, `OriginalLine`, `ConnectionId`, and `Context`; `OriginalLine`
refers to the readable XAML before the diagnostic comments were removed.

## Project layout

Each readable XAML document now occupies the logical path represented by its BAML
resource. Examples:

```text
app.baml                                      -> app.xaml
mainwindow.baml                              -> mainwindow.xaml
controls/avatarcontrol/avatarcontrolex.baml  -> controls/avatarcontrol/avatarcontrolex.xaml
themes/vs2019dark.baml                       -> themes/vs2019dark.xaml
```

The duplicate loose `.baml` files were removed. The generated assembly contains
one `pvfUtility.g.resources` manifest resource and no standalone `.baml` manifest
resources.

## Why XAML is migrated incrementally

Decompiled BAML is not equivalent to the original design-time XAML. ILSpy emitted
270 `Unknown connection ID` comments across 74 views. They were decompiler
diagnostics rather than executable markup, so they have been moved out of the
readable XAML into `docs/BAML_CONNECTION_ID_AUDIT.csv`. This cleanup does not
recover the missing connector metadata. The recovered C# already contains the
original `InitializeComponent` and `IComponentConnector` implementations, and
recompiling all XAML could generate different connection IDs or duplicate
generated members, changing event handlers and named-control wiring.

`Directory.Build.targets` therefore removes all recovered main-program XAML from
WPF items first and retains it as `None`. The explicit `RecoveredSourceXaml`
allowlist then re-enables only verified documents. The first batch compiles
`app.xaml` and `views/viewscripteditor.xaml`; `PvfResourceMerger` overlays their
generated BAML on an intermediate resource container while preserving all other
raw resource types and data from `Resources/pvfUtility.g.resources`.

The build modes are:

- `Legacy`: no recovered main-program XAML compilation; embed the original
  container unchanged.
- `Hybrid`: compile the current allowlist and supplement it from the original
  container. This is the default.
- `SourceOnly`: do not supplement from original BAML; fail until all 126 BAML
  mappings have migrated.

`app.xaml` remains a `Page`, not an `ApplicationDefinition`, so the recovered
`App.Main` and startup initialization remain authoritative. `ViewScriptEditor`
uses standard generated partial-class code in Hybrid, while its conditional
legacy partial supplies the recovered loader only in Legacy.

Every future conversion must be performed one document or dependency cluster at
a time: reconcile the root type and `x:Class`, choose a single owner for
`InitializeComponent`, restore connector semantics, compare the final resource
container, and run the full UI regression test. The detailed contract and
rollback commands are in `docs/SOURCE_XAML_MIGRATION.md`.

## Network boundaries and compatibility types

Continuing to retain and load unmigrated original BAML preserves its serialized
CLR schema.
The schema contains references to account, cloud-backup, store, and ChatGPT
views, plus deferred properties such as `ViewMacroStoreDataTemplate`,
`BookMarkStoreTemplate`, `ViewStoreList`, and `ChatGPT` on
`DocumentItemContentTemplateSelector`. Some of these members look unused when
searching C# call sites, but WPF writes them while materializing deferred BAML
resources.

Removing one of those properties caused the main window to fail after several
child controls had loaded. WPF reported only a `NullReferenceException` from
`WpfXamlLoader.TransformNodes` and `ResourceDictionary.CreateObject`; restoring
the serialized property surface fixed the failure. Treat BAML-referenced types,
constructors, public properties, event handlers, and enum identities as runtime
contracts even when the corresponding feature is disabled.

These compatibility contracts do not implicitly restore network behavior. The
active boundary is enforced separately:

- `ServiceCloud` GET and POST transport returns an offline error without making
  an HTTP request.
- The original ChatGPT toolbar binding is an explicit opt-in PVF assistant. It
  opens in a full-height panel at the far right. Endpoint/model settings
  are local, the API key is session-only or comes from `OPENAI_API_KEY`, and
  current-PVF tools require an in-session read toggle.
- Auto-update and exception-telemetry assemblies are excluded from output.
- Account, cloud, store, and sharing controls serialized in BAML are removed
  from the populated main-window UI; only the ChatGPT assistant is enabled.
- Application settings, bookmarks, tree comments, and extension-scoped PVF tag
  comments remain JSON-only.

Any change to those compatibility types requires both a normal build and
`scripts/Test-RecoveredStartup.ps1`; compilation alone does not materialize all
deferred BAML resources.

## Recovered library BAML

The managed dependency audit found another 18 BAML files outside the main
`pvfUtility.g.resources` container:

- 6 in `ICSharpCode.AvalonEdit`.
- 7 in `TextEditLib`.
- 4 in `UnitComboLib`.
- 1 in `WpfRangeControls`.

These files are resource dictionaries or controls with a much smaller connection
surface than the main application views. ILSpy 10.1 `--decompile-baml` produced
18 XAML files that all pass WPF markup compilation. Their projects now compile
the XAML as `Page` items, and no loose BAML remains under
`SourceLibraries`.

`UnitComboLib`, `WpfRangeControls`, and AvalonEdit were rebuilt and checked again.
AvalonEdit reproduces all 33 original resource entries and successfully loads its
generic theme, `TextEditor`, and built-in C# highlighting definition. TextEditLib's
seven resource dictionaries also pass a clean project build with their original
pack URIs preserved.
