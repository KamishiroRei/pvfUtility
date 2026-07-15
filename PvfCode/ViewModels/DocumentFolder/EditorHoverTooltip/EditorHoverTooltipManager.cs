using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;
using PvfCode;
using PvfCode.Controls.TextEditorFolder;
using PvfCode.Dot.Desktop;
using PvfCode.Dot.Desktop.Enums;
using PvfCode.Models.Options.Editor;
using PvfCode.Models.Options.Editor.ItemCodeHoverConfigModels;
using PvfCode.Models.Options.Enums;
using PvfCode.Services;
using PvfCode.ViewModels.DocumentFolder.EditorHoverTooltip;
using PvfCode.ViewModels.DocumentFolder.EditorHoverTooltip.CommentHoverTooltip;
using PvfCode.ViewModels.DocumentFolder.EditorHoverTooltip.ItemCodeHoverTooltip;
using PvfCode.ViewModels.DocumentFolder.EditorHoverTooltip.ViewModels;
using PvfCode.ViewModels.DocumentFolder.Foldings.FoldingEnitys;
using PvfCode.ViewModels.DocumentFolder.LinkFolder;
using Utools;
using ViewModels.DocumentFolder.EditorHoverTooltip;

namespace PvfCode.ViewModels.DocumentFolder.EditorHoverTooltip;

internal class EditorHoverTooltipManager : IDisposable
{
	private readonly TextEditorBase Editor;

	private GeneralHoverTooltip ToolTip;

	private readonly PvfFile File;

	private bool isMouseOverTooltip;

	private readonly DispatcherTimer tooltipCloseTimer;

	private bool keepOpenWhenEditorIsHovered;

	private TextDocument Document => Editor.Document;

	private PvfGroup Pvf => AppCore.ViewModelBase.PVF;

	public EditorHoverTooltipManager(TextEditorBase editor, PvfFile file)
	{
		File = file;
		Editor = editor;
		Editor.TextArea.TextView.MouseHover += OnEditorMouseHover;
		Editor.TextArea.TextView.MouseHoverStopped += OnEditorMouseHoverStopped;
		Editor.TextArea.TextView.MouseLeftButtonDown += OnEditorMouseLeftButtonDown;
		Editor.Unloaded += OnEditorUnloaded;
		tooltipCloseTimer = new DispatcherTimer(DispatcherPriority.Input)
		{
			Interval = TimeSpan.FromMilliseconds(500.0)
		};
		tooltipCloseTimer.Tick += OnTooltipCloseTimerTick;
		ToolTip = new GeneralHoverTooltip(null);
		ToolTip.MouseLeave += OnTooltipMouseLeave;
		ToolTip.MouseEnter += OnTooltipMouseEnter;
		Application.Current.MainWindow.Deactivated += OnMainWindowDeactivated;
	}

	private void OnMainWindowDeactivated(object? sender, EventArgs e)
	{
		CloseTooltip();
	}

	private void OnEditorUnloaded(object sender, RoutedEventArgs e)
	{
		CloseTooltip();
	}

	private void OnEditorMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		CloseTooltip();
	}

	private void OnTooltipMouseEnter(object sender, MouseEventArgs e)
	{
		tooltipCloseTimer.Stop();
		isMouseOverTooltip = true;
	}

	private void OnTooltipMouseLeave(object sender, MouseEventArgs e)
	{
		isMouseOverTooltip = false;
		ScheduleTooltipClose(keepOpenOverEditor: true);
	}

	private void OnEditorMouseHoverStopped(object sender, MouseEventArgs e)
	{
		if (ToolTip.IsOpen && !isMouseOverTooltip)
		{
			VisualLineElement hoveredElement = Editor.TextArea.TextView.GetVisualLineElementFromPosition(e.GetPosition(Editor.TextArea.TextView) + Editor.TextArea.TextView.ScrollOffset);
			if (ToolTip.HoveredElement != hoveredElement)
			{
				ScheduleTooltipClose(keepOpenOverEditor: false);
			}
			e.Handled = true;
		}
	}

	private void ScheduleTooltipClose(bool keepOpenOverEditor)
	{
		keepOpenWhenEditorIsHovered = keepOpenOverEditor;
		tooltipCloseTimer.Stop();
		tooltipCloseTimer.Start();
	}

	private void OnTooltipCloseTimerTick(object? sender, EventArgs e)
	{
		tooltipCloseTimer.Stop();
		if (ToolTip == null || !ToolTip.IsOpen || ToolTip.IsMouseOver)
		{
			return;
		}
		if (keepOpenWhenEditorIsHovered && Editor.TextArea.TextView.IsMouseOver)
		{
			return;
		}
		CloseTooltip();
	}

	private void OnEditorMouseHover(object sender, MouseEventArgs e)
	{
		if (ToolTip.IsOpen)
		{
			CloseTooltip();
		}
		VisualLineElement hoveredElement = Editor.TextArea.TextView.GetVisualLineElementFromPosition(e.GetPosition(Editor.TextArea.TextView) + Editor.TextArea.TextView.ScrollOffset);
		if (hoveredElement == null)
		{
			return;
		}
		ToolTip.HoveredElement = hoveredElement;
		ToolTipViewModelBase tooltipViewModel;
		if (hoveredElement is FoldingElementGenerator.FoldingLineElement)
		{
			tooltipViewModel = CreateFoldingTooltip(hoveredElement);
			tooltipViewModel.TooltipType = EditorTooltipType.Folding;
		}
		else if (hoveredElement is ScriptCommentVisualLine)
		{
			tooltipViewModel = CreateCommentTooltip(hoveredElement);
			if (tooltipViewModel != null)
			{
				tooltipViewModel.TooltipType = EditorTooltipType.Comment;
			}
		}
		else if (hoveredElement is FilePathLinkVisualLine)
		{
			tooltipViewModel = new ToolTipViewModel_FilePathHoverTooltip(((FilePathLinkVisualLine)hoveredElement).GetFilePath().Replace("`", ""), GetTemplateSelector());
			tooltipViewModel.TooltipType = EditorTooltipType.FilePath;
		}
		else
		{
			tooltipViewModel = CreateItemCodeTooltipForHover();
			if (tooltipViewModel != null)
			{
				tooltipViewModel.TooltipType = EditorTooltipType.ItemCode;
			}
		}
		if (tooltipViewModel != null)
		{
			ToolTip.PlacementTarget = sender as UIElement;
			ToolTip.Placement = PlacementMode.RelativePoint;
			Point position = e.GetPosition(Editor.TextArea.TextView);
			ToolTip.HorizontalOffset = position.X;
			ToolTip.VerticalOffset = position.Y + 15.0;
			ToolTip.DataContext = tooltipViewModel;
			ToolTip.Width = double.NaN;
			ToolTip.Height = double.NaN;
			ToolTip.IsOpen = true;
			tooltipViewModel.Loaded();
			e.Handled = true;
		}
	}

	private EditorTooltipDataTemplateSelector GetTemplateSelector()
	{
		return (EditorTooltipDataTemplateSelector)ToolTip.TryFindResource("EditorTooltipDataTemplateSelector");
	}

	private ToolTipViewModelBase CreateFoldingTooltip(VisualLineElement visualElement)
	{
		Editor.GetMouseOffset();
		FoldingElementGenerator.FoldingLineElement foldingLineElement = (FoldingElementGenerator.FoldingLineElement)visualElement;
		TextSegment segment = new TextSegment
		{
			StartOffset = Editor.Document.GetLineByOffset(foldingLineElement.Section.StartOffset).Offset,
			EndOffset = foldingLineElement.Section.EndOffset
		};
		return new ToolTipViewModel_FoldingTooltip(File, segment, Editor, GetTemplateSelector());
	}

	private ToolTipViewModelBase CreateCommentTooltip(VisualLineElement visualElement)
	{
		TextSegment segment = ((ScriptCommentVisualLine)visualElement).GetSegment();
		PvfCommentType pvfCommentType = ((Editor.Document.GetCharAt(segment.StartOffset) != '[') ? PvfCommentType.String : PvfCommentType.Section);
		string commentText = Editor.Document.GetText(segment);
		if (string.IsNullOrEmpty(commentText) || commentText.Length > 60)
		{
			return null;
		}
		if (commentText.HasChinese())
		{
			return null;
		}
		bool shouldShow = false;
		switch (AppSetting.Instance.EditConfig.EditorCommentHoverTooltipMode)
		{
		case HoverTooltipMode.鼠标移入:
			shouldShow = true;
			break;
		case HoverTooltipMode.按住Ctrl鼠标移入:
			if (((int)Keyboard.Modifiers & 2) == 2)
			{
				shouldShow = true;
			}
			break;
		case HoverTooltipMode.按住Alt鼠标移入:
			if (((int)Keyboard.Modifiers & 1) == 1)
			{
				shouldShow = true;
			}
			break;
		default:
			return null;
		}
		if (!shouldShow)
		{
			return null;
		}
		ToolTipViewModel_SectionComment result = new ToolTipViewModel_SectionComment(new PvfCommentDtoRes
		{
			FileType = File.FileType,
			PvfCommentType = pvfCommentType,
			Section = commentText
		}, GetTemplateSelector());
		result.OpenEditorRequested = viewModel =>
		{
			CloseTooltip(force: true);
			AppCore.ViewModelBase.RootDocument.OpenPvfTagCommentEditor(viewModel.CommentRequest);
		};
		HighlightItemCode(new HighlightedSection
		{
			Offset = segment.StartOffset,
			Length = segment.Length
		});
		return result;
	}

	private ToolTipViewModelBase CreateItemCodeTooltipForHover()
	{
		if (File.FileType == PvfFileType.tbl)
		{
			return null;
		}
		if (!AppCore.ViewModelBase.PVF.PvfIsOpen)
		{
			return null;
		}
		int? mouseOffset = Editor.GetMouseOffset();
		if (!mouseOffset.HasValue)
		{
			return null;
		}
		ToolTipViewModelBase tooltip = null;
		bool ctrlPressed = ((int)Keyboard.Modifiers & 2) == 2;
		bool altPressed = ((int)Keyboard.Modifiers & 1) == 1;
		switch (AppSetting.Instance.EditConfig.EditorItemCodeHoverTooltipMode)
		{
		case HoverTooltipMode.鼠标移入:
			tooltip = CreateItemCodeTooltipAtOffset(mouseOffset.Value);
			break;
		case HoverTooltipMode.按住Ctrl鼠标移入:
			if (!ctrlPressed)
			{
				return null;
			}
			tooltip = CreateItemCodeTooltipAtOffset(mouseOffset.Value);
			break;
		case HoverTooltipMode.按住Alt鼠标移入:
			if (!altPressed)
			{
				return null;
			}
			tooltip = CreateItemCodeTooltipAtOffset(mouseOffset.Value);
			break;
		}
		return tooltip;
	}

	private ToolTipViewModelBase CreateItemCodeTooltipAtOffset(int offset)
	{
		int processingStep = 0;
		try
		{
			DocumentLine line = Editor.Document.GetLineByOffset(offset);
			processingStep++;
			if (IsIndependentDropFile())
			{
				return CreateIndependentDropTooltip(offset, line);
			}
			processingStep++;
			if (IsUiControlDungeonReferenceLine(line.LineNumber))
			{
				return CreateUiControlDungeonTooltip(offset, line);
			}
			processingStep++;
			int tokenIndex;
			HighlightedSection numberSection = Editor.GetHigSectionAndIndex(HighlightingType.Digits, line, offset, out tokenIndex);
			if (numberSection == null)
			{
				return null;
			}
			processingStep++;
			ItemCodeHoverInfoBase hoverInfo = null;
			string itemCodeText = Editor.Document.GetText(numberSection);
			if (itemCodeText == "-1")
			{
				return null;
			}
			processingStep++;
			if (!int.TryParse(itemCodeText, out int itemCode))
			{
				return null;
			}
			processingStep++;
			string currentSectionName = GetContainingSection(line, out int currentSectionLineNumber);
			if (currentSectionName == null)
			{
				return null;
			}
			processingStep++;
			if (File.FileType == PvfFileType.dgn && currentSectionName == "[special passive object item]")
			{
				return CreateSpecialPassiveObjectItemTooltip(offset, line);
			}
			processingStep++;
			List<KeyValuePair<string, ItemCodeHoverInfoBase>> configuredHoverInfos = AppSetting.Instance.EditConfig.ItemCodeConvertItemNameConfiger.Get(File.FileName, currentSectionName, tokenIndex, itemCode);
			if (configuredHoverInfos == null)
			{
				return null;
			}
			processingStep++;
			List<KeyValuePair<string, ItemCodeHoverInfoBase>> fallbackConfigs = new List<KeyValuePair<string, ItemCodeHoverInfoBase>>();
			foreach (KeyValuePair<string, ItemCodeHoverInfoBase> configEntry in configuredHoverInfos)
			{
				ItemCodeHoverInfoBase config = configEntry.Value;
				if (!string.IsNullOrEmpty(config.ParentSectionName))
				{
					if (MatchesParentSection(currentSectionLineNumber, config.ParentSectionName, currentSectionName) && MatchesValidationSections(config.ValidationSectionList, line))
					{
						hoverInfo = config;
						break;
					}
				}
				else
				{
					fallbackConfigs.Add(configEntry);
				}
			}
			processingStep++;
			if (hoverInfo == null && !fallbackConfigs.Any())
			{
				return null;
			}
			processingStep++;
			if (hoverInfo == null)
			{
				foreach (KeyValuePair<string, ItemCodeHoverInfoBase> fallbackEntry in fallbackConfigs)
				{
					ItemCodeHoverInfoBase fallbackConfig = fallbackEntry.Value;
					if (MatchesValidationSections(fallbackConfig.ValidationSectionList, line))
					{
						hoverInfo = fallbackConfig;
					}
				}
			}
			processingStep++;
			if (hoverInfo == null)
			{
				return null;
			}
			processingStep++;
			hoverInfo = hoverInfo.Get(tokenIndex, itemCode);
			if (hoverInfo == null)
			{
				return null;
			}
			processingStep++;
			string resolvedFilePath = null;
			foreach (string lstFileName in hoverInfo.LstFileNames)
			{
				if (string.IsNullOrEmpty(lstFileName))
				{
					AppCore.Logger.Error(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("EditorHoverTooltip_ConfigFileError_lstNameNull"), hoverInfo.ParentSectionName));
					return null;
				}
				resolvedFilePath = Pvf.ListFileTable.ItemCodeConvertFilePath(lstFileName, itemCode);
				if (resolvedFilePath != null)
				{
					break;
				}
			}
			processingStep++;
			HighlightItemCode(numberSection);
			processingStep++;
			return new ToolTipViewModel_ItemCodeHoverTooltip(resolvedFilePath, itemCode, hoverInfo, GetTemplateSelector());
		}
		catch (Exception ex)
		{
			string msg = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("EditorHoverTooltip_ConfigFileError"), processingStep, ex.Message);
			AppCore.Logger.Error(msg);
			return null;
		}
	}

	private bool MatchesValidationSections(List<KeyValuePair<string, ValidationSectionData>> validationSections, DocumentLine line)
	{
		if (validationSections == null || !validationSections.Any())
		{
			return true;
		}
		Dictionary<string, string> decompiledSections = null;
		string[] currentLineColumns = null;
		int matchedRuleCount = 0;
		foreach (KeyValuePair<string, ValidationSectionData> validation in validationSections)
		{
			ValidationSectionData rule = validation.Value;
			if (rule.CurrentLine.HasValue && rule.CurrentLine.Value)
			{
				if (currentLineColumns == null)
				{
					string currentLineText = Document.GetText(line);
					if (string.IsNullOrEmpty(currentLineText))
					{
						return false;
					}
					currentLineColumns = currentLineText.Split("\t", StringSplitOptions.RemoveEmptyEntries);
					if (currentLineColumns.Length == 0)
					{
						return false;
					}
				}
				int columnIndex = (rule.Index.HasValue ? rule.Index.Value : 0);
				if (currentLineColumns.Length < columnIndex)
				{
					return false;
				}
				if (currentLineColumns[columnIndex] == rule.Value)
				{
					matchedRuleCount++;
					continue;
				}
				return false;
			}
			if (decompiledSections == null)
			{
				ScriptFileCompilerOl compiler = new ScriptFileCompilerOl(AppCore.ViewModelBase.PVF);
				PvfFile compiledFile = new PvfFile(File.FileName);
				byte[] compiledData = compiler.Compile(new PvfFile(File.FileName), Document.Text);
				if (compiledData == null)
				{
					return false;
				}
				compiledFile.WriteFileData(compiledData);
				decompiledSections = compiler.DecompileDic(compiledFile);
				if (decompiledSections == null)
				{
					return false;
				}
			}
			if (decompiledSections.TryGetValue(validation.Key, out string sectionValue))
			{
				if (rule.Index.HasValue)
				{
					if (!string.IsNullOrEmpty(sectionValue))
					{
						string[] sectionColumns = sectionValue.Split("\t", StringSplitOptions.RemoveEmptyEntries);
						if (sectionColumns.Any() && rule.Index.Value < sectionColumns.Count() && sectionColumns[rule.Index.Value] == rule.Value)
						{
							matchedRuleCount++;
						}
					}
				}
				else if (rule.Value == sectionValue.Replace("\t", string.Empty))
				{
					matchedRuleCount++;
				}
				continue;
			}
			return false;
		}
		return validationSections.Count == matchedRuleCount;
	}

	private bool IsIndependentDropFile()
	{
		return File.FileName == "etc/independent_drop.etc";
	}

	private ToolTipViewModel_ItemCodeHoverTooltip CreateIndependentDropTooltip(int offset, DocumentLine line)
	{
		HighlightedSection numberSection = Editor.GetHigSectionAndIndex(HighlightingType.Digits, line, offset, out int tokenIndex);
		if (numberSection == null)
		{
			return null;
		}
		if (!MatchesParentSection(line.LineNumber, "[dungeon drop rate balance]", null))
		{
			string itemCodeText = Document.GetText(numberSection);
			if (itemCodeText == "-1")
			{
				return null;
			}
			if (!int.TryParse(itemCodeText, out int itemCode))
			{
				return null;
			}
			string[] columns = Document.GetText(line).Split("\t", StringSplitOptions.RemoveEmptyEntries);
			if (columns.Count() == 17)
			{
				switch (tokenIndex)
				{
				case 1:
					if (columns[0] == "0")
					{
						HighlightItemCode(numberSection);
						return new ToolTipViewModel_ItemCodeHoverTooltip(Pvf.ListFileTable.ItemCodeConvertFilePath("monster", itemCode), itemCode, new ItemCodeHoverInfoDefault
						{
							Description = AppSetting.Instance.GetIlogger().GetStr("Luang_MonsterName")
						}, GetTemplateSelector());
					}
					if (columns[0] == "1")
					{
						HighlightItemCode(numberSection);
						return new ToolTipViewModel_ItemCodeHoverTooltip(Pvf.ListFileTable.ItemCodeConvertFilePath("aicharacter", itemCode), itemCode, new ItemCodeHoverInfoDefault
						{
							Description = "APC"
						}, GetTemplateSelector());
					}
					break;
				case 2:
					if (itemCode == 0)
					{
						break;
					}
					if (columns[16] == "0")
					{
						HighlightItemCode(numberSection);
						string itemFilePath = Pvf.ListFileTable.ItemCodeConvertFilePath("equipment", itemCode);
						if (itemFilePath == null)
						{
							itemFilePath = Pvf.ListFileTable.ItemCodeConvertFilePath("stackable", itemCode);
						}
						return new ToolTipViewModel_ItemCodeHoverTooltip(itemFilePath, itemCode, new ItemCodeHoverInfoDefault
						{
							Description = AppSetting.Instance.GetIlogger()?.GetStr("EditorHoverTooltip_Description_DropItemName"),
							LstFileNames = new List<string>
							{
								"equipment",
								"stackable"
							}
						}, GetTemplateSelector());
					}
					return new ToolTipViewModel_ItemCodeHoverTooltip(AppSetting.Instance.GetIlogger()?.GetStr("mess_Independent_drop_OnlyZeroCanBeInputErr"), new ItemCodeHoverInfoDefault
					{
						Description = "错误信息"
					}, GetTemplateSelector());
				}
			}
			else
			{
				if (columns.Count() != 2)
				{
					if (columns.Count() == 1)
					{
						HighlightItemCode(numberSection);
						return new ToolTipViewModel_ItemCodeHoverTooltip(Pvf.ListFileTable.ItemCodeConvertFilePath("independentdrop", itemCode), itemCode, new ItemCodeHoverInfoDefault
						{
							Description = AppSetting.Instance.GetIlogger()?.GetStr("EditorHoverTooltip_Description_DropListName")
						}, GetTemplateSelector());
					}
					return null;
				}
				if (tokenIndex == 0)
				{
					HighlightItemCode(numberSection);
					string itemFilePath = Pvf.ListFileTable.ItemCodeConvertFilePath("equipment", itemCode);
					if (itemFilePath == null)
					{
						itemFilePath = Pvf.ListFileTable.ItemCodeConvertFilePath("stackable", itemCode);
					}
					return new ToolTipViewModel_ItemCodeHoverTooltip(itemFilePath, itemCode, new ItemCodeHoverInfoDefault
					{
						Description = AppSetting.Instance.GetIlogger()?.GetStr("EditorHoverTooltip_Description_DropItemName"),
						LstFileNames = new List<string>
						{
							"equipment",
							"stackable"
						}
					}, GetTemplateSelector());
				}
			}
		}
		return null;
	}

	private bool IsUiControlDungeonReferenceLine(int lineNumber)
	{
		if (File.FileType == PvfFileType.ui && TryFindHighlightedSectionLine(lineNumber, "[ui controls]", out int sectionLineNumber) && Document.LineCount > sectionLineNumber)
		{
			string nextLineText = Document.GetText(Document.GetLineByNumber(sectionLineNumber + 1));
			if (!string.IsNullOrEmpty(nextLineText))
			{
				nextLineText.Replace(" ", "").Replace("\t", "");
				string normalizedControlType = nextLineText.Replace(" ", "").Replace("\t", "");
				if (normalizedControlType == "`[switchbox]`" || normalizedControlType == "`[balloon]`")
				{
					return true;
				}
			}
		}
		return false;
	}

	private ToolTipViewModelBase CreateUiControlDungeonTooltip(int offset, DocumentLine line)
	{
		HighlightedSection numberSection = Editor.GetHigSectionAndIndex(HighlightingType.Digits, line, offset, out int tokenIndex);
		if (numberSection == null)
		{
			return null;
		}
		if (tokenIndex != 4)
		{
			return null;
		}
		string itemCodeText = Editor.Document.GetText(numberSection);
		if (itemCodeText == "-1")
		{
			return null;
		}
		if (!int.TryParse(itemCodeText, out int itemCode))
		{
			return null;
		}
		string? filePath = Pvf.ListFileTable.ItemCodeConvertFilePath("dungeon", itemCode);
		HighlightItemCode(numberSection);
		return new ToolTipViewModel_ItemCodeHoverTooltip(info: new ItemCodeHoverInfoDefault
		{
			Description = AppSetting.Instance.GetIlogger().GetStr("EditorHoverTooltip_Description_DungeonInterface")
		}, filePath: filePath, itemCode: itemCode, editorTooltipDataTemplateSelector: GetTemplateSelector());
	}

	private ToolTipViewModelBase CreateSpecialPassiveObjectItemTooltip(int offset, DocumentLine line)
	{
		HighlightedSection numberSection = Editor.GetHigSectionAndIndex(HighlightingType.Digits, line, offset, out int tokenIndex);
		if (numberSection == null)
		{
			return null;
		}
		if (tokenIndex <= 2)
		{
			return null;
		}
		if (tokenIndex % 2 == 0)
		{
			return null;
		}
		string itemCodeText = Editor.Document.GetText(numberSection);
		if (itemCodeText == "-1")
		{
			return null;
		}
		if (!int.TryParse(itemCodeText, out int itemCode))
		{
			return null;
		}
		string itemFilePath = Pvf.ListFileTable.ItemCodeConvertFilePath("equipment", itemCode);
		if (string.IsNullOrEmpty(itemFilePath))
		{
			itemFilePath = Pvf.ListFileTable.ItemCodeConvertFilePath("stackable", itemCode);
		}
		HighlightItemCode(numberSection);
		ItemCodeHoverInfoBase hoverInfo = new ItemCodeHoverInfoDefault
		{
			Description = AppSetting.Instance.GetIlogger()?.GetStr("EditorHoverTooltip_Description_SpecialPassiveObjectItem")
		};
		return new ToolTipViewModel_ItemCodeHoverTooltip(itemFilePath, itemCode, hoverInfo, GetTemplateSelector());
	}

	private bool MatchesParentSection(int lineNumber, string parentSectionName, string currentSectionName)
	{
		KeyValuePair<int, string>[] sections = Editor.FoldingStrategyBaseHelper.lineNumberSectionDic.ToArray();
		for (int index = 0; index < sections.Length; index++)
		{
			KeyValuePair<int, string> section = sections[index];
			if (section.Key >= lineNumber)
			{
				continue;
			}
			if (!string.IsNullOrEmpty(currentSectionName))
			{
				if (section.Value != currentSectionName && section.Value == parentSectionName)
				{
					return true;
				}
				continue;
			}
			if (!(section.Value == parentSectionName))
			{
				break;
			}
			return true;
		}
		return false;
	}

	private string GetContainingSection(DocumentLine line, out int sectionLineNumber)
	{
		sectionLineNumber = line.LineNumber;
		if (Editor == null || Editor.FoldingStrategyBaseHelper == null)
		{
			sectionLineNumber = -1;
			return null;
		}
		KeyValuePair<int, string>[] sections = Editor.FoldingStrategyBaseHelper.lineNumberSectionDic.ToArray();
		for (int index = 0; index < sections.Length; index++)
		{
			KeyValuePair<int, string> section = sections[index];
			if (section.Key < sectionLineNumber)
			{
				sectionLineNumber = section.Key;
				return section.Value;
			}
		}
		sectionLineNumber = -1;
		return null;
	}

	public bool TryFindHighlightedSectionLine(int lineNumber, string sectionName, out int sectionLineNumber)
	{
		sectionLineNumber = -1;
		if (Editor.FoldingStrategyBaseHelper.HigSectionDic != null)
		{
			KeyValuePair<int, KeyValuePair<string, string>>[] highlightedSections = Editor.FoldingStrategyBaseHelper.HigSectionDic.ToArray();
			for (int index = 0; index < highlightedSections.Length; index++)
			{
				KeyValuePair<int, KeyValuePair<string, string>> highlightedSection = highlightedSections[index];
				if (highlightedSection.Key < lineNumber && highlightedSection.Value.Value == sectionName)
				{
					sectionLineNumber = highlightedSection.Key;
					return true;
				}
			}
		}
		return false;
	}

	private void HighlightItemCode(HighlightedSection section)
	{
		ItemCodeBackgroundRenderers backgroundRenderer = new ItemCodeBackgroundRenderers();
		backgroundRenderer.ItemTextSegment = new TextSegment
		{
			StartOffset = section.Offset,
			EndOffset = section.EndOffset,
			Length = section.Length
		};
		Editor.TextArea.TextView.BackgroundRenderers.Add(backgroundRenderer);
	}

	private void CloseTooltip(bool force = false)
	{
		tooltipCloseTimer.Stop();
		if (ToolTip == null || (!force && ((int)Keyboard.Modifiers & 2) == 2))
		{
			return;
		}
		if (Editor != null)
		{
			IBackgroundRenderer[] array = Editor.TextArea.TextView.BackgroundRenderers.ToArray();
			foreach (IBackgroundRenderer backgroundRenderer in array)
			{
				if (backgroundRenderer is ItemCodeBackgroundRenderers)
				{
					Editor.TextArea.TextView.BackgroundRenderers.Remove(backgroundRenderer);
				}
			}
		}
		ToolTip.IsOpen = false;
	}

	public void Dispose()
	{
		CloseTooltip();
		Editor.TextArea.TextView.MouseHover -= OnEditorMouseHover;
		Editor.TextArea.TextView.MouseHoverStopped -= OnEditorMouseHoverStopped;
		Editor.TextArea.TextView.MouseLeftButtonDown -= OnEditorMouseLeftButtonDown;
		Editor.Unloaded -= OnEditorUnloaded;
		ToolTip.MouseLeave -= OnTooltipMouseLeave;
		ToolTip.MouseEnter -= OnTooltipMouseEnter;
		tooltipCloseTimer.Stop();
		tooltipCloseTimer.Tick -= OnTooltipCloseTimerTick;
		Application.Current.MainWindow.Deactivated -= OnMainWindowDeactivated;
		ToolTip.DataContext = null;
		ToolTip = null;
	}
}
