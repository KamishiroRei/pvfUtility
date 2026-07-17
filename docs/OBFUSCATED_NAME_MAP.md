# Obfuscated Name Recovery Map

The published executable was obfuscated. Semantic names in this document are
recovered from behavior, callers, interfaces, original IL, readable XAML, and
compiled BAML; they are not claims about the unavailable original source names.

## Rename policy

A CLR rename is accepted only when all relevant checks agree:

- current C# and recovered XAML references are known and can be updated;
- the original assembly IL has no unknown consumer of the old type;
- `Resources/pvfUtility.g.resources` does not contain the old namespace,
  type, command, or property name in its BAML payload;
- reflection, serialization, configuration, and assembly-name lookup paths have
  been searched;
- the full build and populated-window startup test pass after the rename.

Dead types are deleted only when current source has no caller, the original IL
contains no reference outside the type itself, and BAML contains no matching
identity. Historical rows in the string-recovery CSV files are retained even when
their source file has since been removed.

## Compatibility-retained CLR identities

These are the only random-looking top-level namespaces still compiled by the main
project. Their names occur in the original BAML, so the semantic physical file
name is intentionally different from the CLR identity.

| Retained CLR type | Semantic file | Reason |
| --- | --- | --- |
| `FKRF7IQiMoSJtPdh45P.NRjpElQyfkexPvgV2wp` | `PvfCode/MVVMServices/DocumentGroupService.cs` | Original BAML resolves the DevExpress `DocumentGroup` service by this CLR identity. |
| `GKcC3iQxbUOdt2roQPY.Kq4YIkQG1jekqKdxajG` | `PvfCode/MVVMServices/DocumentPanelService.cs` | Original BAML resolves the DevExpress `DocumentPanel` service by this CLR identity. |
| `dRvgUYFXgiUlumD56M2.qjqilnF7lAFbCxZ5lIf` | `PvfCode/ViewModels/TreeFolder/TreeFileClipboardManager.cs` | The identity is present in original BAML and remains a compatibility boundary. |

Do not rename these three types unless the corresponding BAML type records are
rewritten and the complete UI is revalidated.

## Restored CLR names

The following internal types had no BAML/reflection dependency and now use their
semantic CLR names. Callers were updated and each batch passed build and startup
verification.

| Published CLR type | Recovered CLR type |
| --- | --- |
| `DvVWRSlu4YITlvMVbF5.mBxGAYliNKKnxteVLsl` | `PvfCode.Services.AutoUpdateManager` |
| `eD06U85fSLYMYPFGfUq.ObGUWa528hMrK3CyPl5` | `PvfCode.Controls.TextEditorFolder.TextEditExtensions` |
| `kBTL1AiBB7o4pYlSaeI.x7WcXFiv1JXL294LjEO` | `PvfCode.Controls.TextEditorFolder.ScriptCommentController` |
| `SqNd8rQB0Al7wqhIiIN.TpwXfZQveDd1ZdsdRrV` | `PvfCode.Services.PvfAutoBackupService` |
| `O1YqPfYlhR6fAB7qsDM.wZfkkQYDiXtTEIqoYij` | `PvfCode.ViewModels.DocumentFolder.KorElementGenerator.StringQuoteElementGenerator` |
| `B5jeNploOaYLgZCqaFO.nSGnuNlw6pBYH6AlBtm` | `PvfCode.Localization.LanguageResourceManager` |
| `gjrWgQG48n0aBxpQHJj.P47d65GAOB8qOMwkYPh` | `PvfCode.ViewModels.DocumentFolder.AniNpkLineElement.ImgRightVirtualLineViewModel` |
| `kVk2rVAoikmSdwnyBB8.rwRtNiAwx8XA8FtkAg8` | `PvfCode.ViewModels.DocumentFolder.BackgroundRenderers.SearchResultBackgroundRenderer` |
| `HZF4Ga5aQDE37MnTxW6.Tv9yyt5QI6y472foG3A` | `PvfCode.ViewModels.DocumentFolder.VisualLineElementGenerators.SectionCommentElementGenerator` |
| `uwg414yfG8gtNGH6NMQ.eoPR8vy26b6IMvyD3gj` | `PvfCode.ViewModels.DocumentFolder.Foldings.FoldingEnitys.XmlElementFolding` |
| `uhkYlHQDqxpcnb5Rli.kX0aynxMj6UtvlFwyN` | `PvfCode.ViewModels.TreeFolder.Drop.ItemCodeDropHandler` |
| `VFW3vAgCYpQH1lKgaf.neKgs7a8uCTXyJY28q` | `PvfCode.ViewModels.TreeFolder.Drop.LstFileDropHandler` |
| `HP8aCiCd7fFV8gKBTMJ.wgXvY5CqbZJOyiLlpw7` | `PvfCode.Views.Tools.WorldDropToolViewModel` |
| `xSbNWvuslv9mM3Ql6hy.apqvG7uosVQDJMTd4Zw` | `PvfCode.ViewModels.DocumentFolder.CodeCompletion.ScriptCodeCompletion` |
| `pCPmPHuyXhmXpTRUIci.X4Rkb3uY6GRxKJbeNlV` | `PvfCode.ViewModels.DocumentFolder.CodeCompletion.NutCodeCompletion` |
| `n4rHqnYNCGOjAr0cZUo.EsjaYkYR4eyUNhP8sbg` | `PvfCode.ViewModels.DocumentFolder.Foldings.FoldingMarginMarker` |
| `Nrhh4wyaV9DGwX3wywo.Su4LqWyQ7DjoKt5GYri` | `PvfCode.ViewModels.DocumentFolder.EditorHoverTooltip.EditorHoverTooltipManager` |

The recovered XAML for `ImgRightVirtualLineView` and `WorlddropTool` exposed the
expected DevExpress command properties. Their methods were restored to
`OnPreview`, `OnGoToFile`, and `OnDelete`, producing the XAML-bound
`OnPreviewCommand`, `OnGoToFileCommand`, and `OnDeleteCommand` properties.

## Removed unreachable types

The following files/types were deleted after source, original-IL, and BAML audits
showed no reachable consumer:

| Published or recovered type | Removed file / reason |
| --- | --- |
| `XgNN9xrosRI1h2ZaJEq.dQejB0rwe0pg8vOq5A9` | `NextMatchingTreeNodeFinder.cs`; no external IL reference and contained an uninitialized traversal list. |
| `PvfCode.ViewModels.DocumentFolder.ErrorMarker.lst.LstErrorMarker` | Stub threw `NotImplementedException` and was never constructed. |
| `PvfCode.ViewModels.DocumentFolder.OffsetColorizers.ColorizeAvalonEditDiffLines2` | Superseded colorizer with no caller. |
| `PvfCode.ViewModels.DocumentFolder.KorElementGenerator.StringQuoteLineElement` and `ajaWe84N3IpvhVDQbul.PqrLJn4RqeisnIRPaYj` | Unused formatted-text pair; the active implementation uses `StringQuoteElementGenerator`. |
| `kPnd3trKXaWa5asHnTx.cWbCIKrO8Fl2pNRU4GM` | `TreeListNodeExtensions.cs`; only consumer was the removed next-node finder. |
| `PlkSVXrFppsIM4ocJhE.dmuenMrBsfJVqNl18gl` | `TreeBuildPostProcessor.cs`; method body was empty, so five no-op calls were removed. |
| `mnmcMylgyk7EDOBcfvr.zxKIkilatmO9VOkgSww` | `TaskDialogHelper.cs`; no caller in source, IL, or BAML. |
| `mpaTXsuw14oOwutx4He.mYEKIgu1NOlxiDVBOly` | Unused count-to-visibility converter. |
| `UFbwd9u4T8VedpaNtKW.CXGwyuuAqwNfH7I2xG4` | Unused string data-template selector. |
| `FiXetka1QYIQFUafpBk.TEjNfja62gLuqYETf1b` | Unused converter whose only body threw `NotImplementedException`. |
| `iUJZJwGlLdZamuTfjEf.e8XPppGDqEDx00HIPEq` | Unused line-highlight background renderer. |
| `gc9fDbYwi0wvCS5BsxT.fXKRo3Y11eND8bDjCLO` | Unused item-reference folding strategy. |
| `XAREluYCLv9MrQLpdys.MA2PDLYT4IVchxxBHUg` | Unused script-section folding strategy. |
| `MemX1K4EnRgYAGb5mUg.zFlsDd4IFlxGGZm1Kr8` | Unused string-quote template selector. |
| `Fr5AwlsrgFyT6bDRUR.AIrnrooAQCoa9IfmEB` and `HG2jkpnx7UKkKjkSmx.f2ArcDLKJ71h2pqu1e` | Unused wavy-underline style/rendering pair. |
| `PvfCode.Controls.Test.AnimationHelper` and `PvfCode.Controls.Test.ZoomAndPanControl` | Unreferenced prototype control pair; source search found only self-references and the 126 original BAML payloads contain neither CLR identity. |

Previously removed encrypted-string resolvers, module constants, empty
obfuscator initializers, anonymous-type source files, and generated module files
remain documented in the string maps and `BINARY_RECOVERY.md`.

## Compiler-artifact normalization

Normalization is intended to restore ordinary C# structure without changing
public behavior. Completed areas include:

- `FoldingStrategyBase`: 1,145 lines reduced to 532; four explicit display
  classes, pseudo-properties, expanded interpolation, and four unreachable private
  methods removed while preserving the folding algorithms.
- `AutoUpdateManager`, `DockLayoutManagerService`, `DocumentBase`,
  `DocumentRoot`, and `ViewImportFilesViewModel` in the main application.
- `TextEditExtensions`, `ScriptCommentController`, `PvfAutoBackupService`,
  `StringQuoteElementGenerator`, `LanguageResourceManager`, both tree-drop
  handlers, `WorldDropToolViewModel`, `SearchResultBackgroundRenderer`, and the
  section-comment generator.
- `DiffEditorCompareResult`, `DocumentItemContentTemplateSelector`,
  `KorFileElementGenerator`, `AniFileGroup`, `TabFoldingStrategy`, and
  `VsCodeEditor`; explicit backing fields, display classes, placeholder menu
  plumbing, and discarded allocations were replaced with ordinary C#.
- `TreeListService` and `ViewConvertChinaPvfFilesViewModel`; private pseudo
  accessors and explicit async/dispatcher display classes were collapsed back
  into normal properties, locals, and lambdas. Additional result-less
  allocations were removed from tree construction, NPK loading, PVF saving,
  import, parsing, completion, and main-window paths.
- `ImagePack2Service`, `PvfExtensionHelper`, `SearchService`,
  `ServiceConvertChinaPlusPvf`, `ServiceCloud`, `ServiceBatchOperation`,
  `ServiceExtractFiles`, `BoosterInfo`, and title-book models in recovered
  libraries.
- Seventy-eight files selected by the
  `using System.Runtime.CompilerServices;` audit across `PvfCode.Models`,
  `PvfCode.Services`, `Utools`, and the main application no longer require that
  using. Random private identifiers and `P_` parameters were renamed only when
  their behavior or an adjacent equivalent implementation established the
  meaning. The policy for the later batches was to retain ordinary locals such as
  `num`, `value`, `list`, and numbered decompiler locals.
- The final `Utools` batches restored 13 files: generated backing fields became
  automatic properties or events, observable-dictionary and scheduler helpers
  gained behavior-derived names, the `SystemInfo` counters were named for their
  configured categories, and expanded interpolation was collapsed without
  changing WMI, P/Invoke, or formatting behavior.
- The final main-application batch restored `LineGuideLines`,
  `ToolTipViewModel_ItemCodeHoverTooltip`, and `WinShopManagerVm`. The shop
  helper names reuse the equivalent `GetSectionName` and `GetNextRowNumber`
  vocabulary already present in `CreateShopItemViewModel`.
- `VsCodeEditorModelBase`, `WindowLoadingViewModel`, `MacroHelper`, both the main
  and recovered-library `UnitViewModelBase` implementations, plus the
  recovered-library `WindowSizeConfig`, `BatchOperationLog`, `ImportFileItem`, and
  `ItemAuraExplainData`; decompiler backing fields and pseudo-accessors were
  restored to ordinary fields, properties, and initialization code. The public
  compatibility names `Widht`, `IndexForm7zip`, `Instance`, and `OnRun` were
  intentionally retained.
- `RelativeAnimatingContentControl`, `NavigationData`,
  `DocumentNavigationService`, `ViewSelectTreeFiles`, `TreeListDropGroup`, and
  `PvfTreeFileBase`; private random identifiers, pseudo-auto-properties,
  expanded interpolation, and an unreachable compiler helper were restored to
  semantic C#. Public binding names and the XAML-visible control identity remain
  unchanged.
- `PvfCode/Converts`; the remaining eight decompiler parameter sets and the
  private localization-label fields in the PVF-diff and store-type converters
  now use interface- and behavior-derived names. Converter CLR identities and
  XAML resource contracts remain unchanged.
- Eighteen small WPF windows and controls under `PvfCode/Views` plus
  `WindowLoading`; generated content-loaded flags, private event handlers, and
  event parameters now use semantic names. Two unreferenced compiler helper
  methods were removed after source and BAML searches found no consumer;
  connection IDs, generated fields, resource URIs, and public CLR identities
  remain unchanged.
- Twenty small search, preview, tree-selector, validation, NPC-shop, drop-list,
  and diff-event models; pseudo-event accessors and pseudo-auto-properties were
  restored to ordinary C#, while private regex/wildcard helpers and selector
  parameters now use behavior-derived names. Public properties, dependency
  properties, events, and selector class identities remain unchanged.
- The three BAML-retained service/clipboard types now use ordinary closures,
  auto-properties, and semantic private or C#-only member names internally.
  Their serialized CLR identities and the clipboard singleton's BAML-bound
  `Instance` and `PasedIsEnabled` members remain unchanged.
- Readable-XAML cleanup replaced 13 ILSpy pseudo-`<Ctor>` nodes with valid markup
  extension/property syntax and moved 270 `Unknown connection ID` comments from
  74 documents into `docs/BAML_CONNECTION_ID_AUDIT.csv`. These edits improve the
  audit source. Hybrid now source-compiles the three explicitly verified entries
  (`app.xaml`, `themes/styles/iconsdark.xaml`, and
  `views/viewscripteditor.xaml`); the other 123 documents remain non-compiled and
  their original BAML continues to define runtime type and connection identities.
- A subsequent placeholder sweep removed all 572 remaining `//IL_xxxx` diagnostic
  comments, two unreferenced controls under the former `PvfCode.Controls.Test`
  namespace, unused `Test` properties in search and macro models, eight discarded
  allocations, no-op private event subscriptions/handlers, and loose-XAML-only
  test buttons, styles, tags, and resources. The
  game-image `Test` property remains as a compatibility alias because the original
  BAML binds that name; normal source and readable XAML now use `CurrentValue`.

The explicit artifact scan used by this recovery checks `[CompilerGenerated]`,
encoded `<>` identifiers, `[SpecialName]`, IL labels, expanded interpolated-string
handlers, dynamic call sites, display classes, and invalid-IL comments. The first
recorded checkpoint was 4,369 matches in 313 files, followed by 4,147 matches in
301 files. The current checkpoint is 3,369 matches in 245 files, with no IL-label
diagnostic comments remaining. Remaining matches are concentrated in large
recovered UI/model files and must be handled in small behavior-verified batches;
they are not evidence that source library substitution failed.

The narrower `using System.Runtime.CompilerServices;` audit now leaves 31
reviewed files unchanged: 19 `AssemblyInfo.cs` files, seven generated
resource/localization accessors, one generated settings file, one module
initializer, one `CallerMemberName` helper, and two normal handwritten or
upstream-library files. Their compiler-services dependencies are intentional;
generated resource backing fields and public/resource-visible names remain
untouched even when the recovered binary exposes non-semantic private names.

### Final review residuals

The final `origin/master...HEAD` review identified four exceptions in earlier
commits. They remain recorded rather than amended because the subsequent task
constraint explicitly prohibited revisiting already committed files:

- `StringView.SearchstrInFiles` changed the regex predicate from
  `regex.IsMatch(keyWord)` to `regex.IsMatch(item.Value.Data)`. This is a public
  behavior change, even though the former predicate appears unusual.
- `StringView.StrListFile.ToText` still contains one explicit
  `StringBuilder.AppendInterpolatedStringHandler` block.
- The `TextEditConfig` follow-up renamed ordinary locals such as `list`, `value`,
  `num`, and `solidColorBrush*`, contrary to the later instruction to avoid
  changing non-obfuscated variable names.
- `ServiceItemCodeTable.cs` did not contain the target using at the fixed point
  but was included in the parsing-services batch, so that file exceeded the
  narrow audit scope.
