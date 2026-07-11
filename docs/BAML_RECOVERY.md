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

## Why XAML is not recompiled

Decompiled BAML is not equivalent to the original design-time XAML. ILSpy emitted
270 `Unknown connection ID` comments across 74 views. They were decompiler
diagnostics rather than executable markup, so they have been moved out of the
readable XAML into `docs/BAML_CONNECTION_ID_AUDIT.csv`. This cleanup does not
recover the missing connector metadata. The recovered C# already contains the
original `InitializeComponent` and `IComponentConnector` implementations, and
recompiling all XAML could generate different connection IDs or duplicate
generated members, changing event handlers and named-control wiring.

`Directory.Build.targets` therefore removes the recovered XAML from WPF `Page`
and `ApplicationDefinition` items and retains it as `None`. The application loads
the exact original BAML from `Resources/pvfUtility.g.resources`, while
developers can inspect and edit the readable XAML source separately.

A future conversion to fully compiled source XAML should be performed one view at
a time: reconcile `x:Class`, make the code-behind partial, remove recovered
generated connector code, rebuild, and run the full UI regression test after each
view.

## Offline compatibility types

Continuing to load the original BAML also preserves its serialized CLR schema.
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

These compatibility contracts do not restore network behavior. The active
offline boundary is enforced separately:

- `ServiceCloud` GET and POST transport returns an offline error without making
  an HTTP request.
- ChatGPT compatibility code does not instantiate a client or retain an API key.
- Auto-update and exception-telemetry assemblies are excluded from output.
- Account, cloud, store, sharing, and ChatGPT controls serialized in BAML are
  removed from the populated main-window UI.
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
