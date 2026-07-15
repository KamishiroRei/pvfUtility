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

	private bool Np9yp01jy7;

	private readonly DispatcherTimer tooltipCloseTimer;

	private bool keepOpenWhenEditorIsHovered;

	private TextDocument Document => Editor.Document;

	private PvfGroup Pvf => AppCore.ViewModelBase.PVF;

	public EditorHoverTooltipManager(TextEditorBase P_0, PvfFile P_1)
	{
		File = P_1;
		Editor = P_0;
		Editor.TextArea.TextView.MouseHover += YBWyL20Hlt;
		Editor.TextArea.TextView.MouseHoverStopped += WDoysb9I3D;
		Editor.TextArea.TextView.MouseLeftButtonDown += mKxy18IUB2;
		Editor.Unloaded += Rxyy6hXjyq;
		tooltipCloseTimer = new DispatcherTimer(DispatcherPriority.Input)
		{
			Interval = TimeSpan.FromMilliseconds(500.0)
		};
		tooltipCloseTimer.Tick += TooltipCloseTimer_Tick;
		ToolTip = new GeneralHoverTooltip(null);
		ToolTip.MouseLeave += aENyoWbWyi;
		ToolTip.MouseEnter += wh4ywjdSjh;
		Application.Current.MainWindow.Deactivated += vB8ygbr7b2;
	}

	private void vB8ygbr7b2(object? sender, EventArgs P_1)
	{
		CloseTooltip();
	}

	private void Rxyy6hXjyq(object P_0, RoutedEventArgs P_1)
	{
		CloseTooltip();
	}

	private void mKxy18IUB2(object P_0, MouseButtonEventArgs P_1)
	{
		CloseTooltip();
	}

	private void wh4ywjdSjh(object P_0, MouseEventArgs P_1)
	{
		tooltipCloseTimer.Stop();
		Np9yp01jy7 = true;
	}

	private void aENyoWbWyi(object P_0, MouseEventArgs P_1)
	{
		Np9yp01jy7 = false;
		ScheduleTooltipClose(keepOpenOverEditor: true);
	}

	private void WDoysb9I3D(object P_0, MouseEventArgs P_1)
	{
		if (ToolTip.IsOpen && !Np9yp01jy7)
		{
			VisualLineElement visualLineElementFromPosition = Editor.TextArea.TextView.GetVisualLineElementFromPosition(P_1.GetPosition(Editor.TextArea.TextView) + Editor.TextArea.TextView.ScrollOffset);
			if (ToolTip.hYbiDkDyis != visualLineElementFromPosition)
			{
				ScheduleTooltipClose(keepOpenOverEditor: false);
			}
			P_1.Handled = true;
		}
	}

	private void ScheduleTooltipClose(bool keepOpenOverEditor)
	{
		keepOpenWhenEditorIsHovered = keepOpenOverEditor;
		tooltipCloseTimer.Stop();
		tooltipCloseTimer.Start();
	}

	private void TooltipCloseTimer_Tick(object? sender, EventArgs e)
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

	private void YBWyL20Hlt(object P_0, MouseEventArgs P_1)
	{
		if (ToolTip.IsOpen)
		{
			CloseTooltip();
		}
		VisualLineElement visualLineElementFromPosition = Editor.TextArea.TextView.GetVisualLineElementFromPosition(P_1.GetPosition(Editor.TextArea.TextView) + Editor.TextArea.TextView.ScrollOffset);
		if (visualLineElementFromPosition == null)
		{
			return;
		}
		ToolTip.hYbiDkDyis = visualLineElementFromPosition;
		ToolTipViewModelBase toolTipViewModelBase;
		if (visualLineElementFromPosition is FoldingElementGenerator.FoldingLineElement)
		{
			toolTipViewModelBase = EsryqoIeMa(visualLineElementFromPosition);
			toolTipViewModelBase.TooltipType = EditorTooltipType.Folding;
		}
		else if (visualLineElementFromPosition is ScriptCommentVisualLine)
		{
			toolTipViewModelBase = lFcyd1qmXv(visualLineElementFromPosition);
			if (toolTipViewModelBase != null)
			{
				toolTipViewModelBase.TooltipType = EditorTooltipType.Comment;
			}
		}
		else if (visualLineElementFromPosition is FilePathLinkVisualLine)
		{
			toolTipViewModelBase = new ToolTipViewModel_FilePathHoverTooltip(((FilePathLinkVisualLine)visualLineElementFromPosition).GetFilePath().Replace("`", ""), HXHynMHZCa());
			toolTipViewModelBase.TooltipType = EditorTooltipType.FilePath;
		}
		else
		{
			toolTipViewModelBase = igVyeSTo63();
			if (toolTipViewModelBase != null)
			{
				toolTipViewModelBase.TooltipType = EditorTooltipType.ItemCode;
			}
		}
		if (toolTipViewModelBase != null)
		{
			ToolTip.PlacementTarget = P_0 as UIElement;
			ToolTip.Placement = PlacementMode.RelativePoint;
			Point position = P_1.GetPosition(Editor.TextArea.TextView);
			ToolTip.HorizontalOffset = position.X;
			ToolTip.VerticalOffset = position.Y + 15.0;
			ToolTip.DataContext = toolTipViewModelBase;
			ToolTip.Width = double.NaN;
			ToolTip.Height = double.NaN;
			ToolTip.IsOpen = true;
			toolTipViewModelBase.Loaded();
			P_1.Handled = true;
		}
	}

	private EditorTooltipDataTemplateSelector HXHynMHZCa()
	{
		return (EditorTooltipDataTemplateSelector)ToolTip.TryFindResource("EditorTooltipDataTemplateSelector");
	}

	private ToolTipViewModelBase EsryqoIeMa(VisualLineElement P_0)
	{
		Editor.GetMouseOffset();
		FoldingElementGenerator.FoldingLineElement foldingLineElement = (FoldingElementGenerator.FoldingLineElement)P_0;
		TextSegment seg = new TextSegment
		{
			StartOffset = Editor.Document.GetLineByOffset(foldingLineElement.Mnobn1j3iJ.StartOffset).Offset,
			EndOffset = foldingLineElement.Mnobn1j3iJ.EndOffset
		};
		return new ToolTipViewModel_FoldingTooltip(File, seg, Editor, HXHynMHZCa());
	}

	private ToolTipViewModelBase lFcyd1qmXv(VisualLineElement P_0)
	{
		TextSegment segment = ((ScriptCommentVisualLine)P_0).GetSegment();
		PvfCommentType pvfCommentType = ((Editor.Document.GetCharAt(segment.StartOffset) != '[') ? PvfCommentType.String : PvfCommentType.Section);
		string text = Editor.Document.GetText(segment);
		if (text == null || string.IsNullOrEmpty(text) || text.Length > 60)
		{
			return null;
		}
		if (text.HasChinese())
		{
			return null;
		}
		bool flag = false;
		switch (AppSetting.Instance.EditConfig.EditorCommentHoverTooltipMode)
		{
		case HoverTooltipMode.鼠标移入:
			flag = true;
			break;
		case HoverTooltipMode.按住Ctrl鼠标移入:
			if (((int)Keyboard.Modifiers & 2) == 2)
			{
				flag = true;
			}
			break;
		case HoverTooltipMode.按住Alt鼠标移入:
			if (((int)Keyboard.Modifiers & 1) == 1)
			{
				flag = true;
			}
			break;
		default:
			return null;
		}
		if (!flag)
		{
			return null;
		}
		ToolTipViewModel_SectionComment result = new ToolTipViewModel_SectionComment(new PvfCommentDtoRes
		{
			FileType = File.FileType,
			PvfCommentType = pvfCommentType,
			Section = text
		}, HXHynMHZCa());
		result.OpenEditorRequested = viewModel =>
		{
			CloseTooltip(force: true);
			AppCore.ViewModelBase.RootDocument.OpenPvfTagCommentEditor(viewModel.CommentRequest);
		};
		FlsykUFpxm(new HighlightedSection
		{
			Offset = segment.StartOffset,
			Length = segment.Length
		});
		return result;
	}

	private ToolTipViewModelBase igVyeSTo63()
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
		ToolTipViewModelBase result = null;
		bool flag = ((int)Keyboard.Modifiers & 2) == 2;
		bool flag2 = ((int)Keyboard.Modifiers & 1) == 1;
		switch (AppSetting.Instance.EditConfig.EditorItemCodeHoverTooltipMode)
		{
		case HoverTooltipMode.鼠标移入:
			result = p8cytf6ANW(mouseOffset.Value);
			break;
		case HoverTooltipMode.按住Ctrl鼠标移入:
			if (!flag)
			{
				return null;
			}
			result = p8cytf6ANW(mouseOffset.Value);
			break;
		case HoverTooltipMode.按住Alt鼠标移入:
			if (!flag2)
			{
				return null;
			}
			result = p8cytf6ANW(mouseOffset.Value);
			break;
		}
		return result;
	}

	private ToolTipViewModelBase p8cytf6ANW(int P_0)
	{
		int num = 0;
		try
		{
			DocumentLine lineByOffset = Editor.Document.GetLineByOffset(P_0);
			num++;
			if (lncyIn1qdE())
			{
				return XKSyETmCoP(P_0, lineByOffset);
			}
			num++;
			if (X6VyOg2DWE(lineByOffset.LineNumber))
			{
				return FtByKRZEQV(P_0, lineByOffset);
			}
			num++;
			int index;
			HighlightedSection higSectionAndIndex = Editor.GetHigSectionAndIndex(HighlightingType.Digits, lineByOffset, P_0, out index);
			if (higSectionAndIndex == null)
			{
				return null;
			}
			num++;
			TextDocument document = Editor.Document;
			ItemCodeHoverInfoBase itemCodeHoverInfoBase = null;
			string text = document.GetText(higSectionAndIndex);
			if (text == "-1")
			{
				return null;
			}
			num++;
			if (!int.TryParse(text, out var result))
			{
				return null;
			}
			num++;
			int num2;
			string text2 = ivqyZliCJt(lineByOffset, out num2);
			if (text2 == null)
			{
				return null;
			}
			num++;
			if (File.FileType == PvfFileType.dgn && text2 == "[special passive object item]")
			{
				return hMZy9yAP9Z(P_0, lineByOffset);
			}
			num++;
			List<KeyValuePair<string, ItemCodeHoverInfoBase>> list = AppSetting.Instance.EditConfig.ItemCodeConvertItemNameConfiger.Get(File.FileName, text2, index, result);
			if (list == null)
			{
				return null;
			}
			num++;
			List<KeyValuePair<string, ItemCodeHoverInfoBase>> list2 = new List<KeyValuePair<string, ItemCodeHoverInfoBase>>();
			foreach (KeyValuePair<string, ItemCodeHoverInfoBase> item in list)
			{
				ItemCodeHoverInfoBase value = item.Value;
				if (!string.IsNullOrEmpty(value.ParentSectionName))
				{
					if (yK6yPrZhI7(num2, value.ParentSectionName, text2) && jBcybHZMEM(item.Value.ValidationSectionList, lineByOffset))
					{
						itemCodeHoverInfoBase = item.Value;
						break;
					}
				}
				else
				{
					list2.Add(item);
				}
			}
			num++;
			if (itemCodeHoverInfoBase == null && !list2.Any())
			{
				return null;
			}
			num++;
			if (itemCodeHoverInfoBase == null)
			{
				foreach (KeyValuePair<string, ItemCodeHoverInfoBase> item2 in list2)
				{
					ItemCodeHoverInfoBase value2 = item2.Value;
					if (jBcybHZMEM(value2.ValidationSectionList, lineByOffset))
					{
						itemCodeHoverInfoBase = value2;
					}
				}
			}
			num++;
			if (itemCodeHoverInfoBase == null)
			{
				return null;
			}
			num++;
			itemCodeHoverInfoBase = itemCodeHoverInfoBase.Get(index, result);
			if (itemCodeHoverInfoBase == null)
			{
				return null;
			}
			num++;
			string text3 = null;
			foreach (string lstFileName in itemCodeHoverInfoBase.LstFileNames)
			{
				if (string.IsNullOrEmpty(lstFileName))
				{
					AppCore.Logger.Error(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("EditorHoverTooltip_ConfigFileError_lstNameNull"), itemCodeHoverInfoBase.ParentSectionName));
					return null;
				}
				text3 = Pvf.ListFileTable.ItemCodeConvertFilePath(lstFileName, result);
				if (text3 != null)
				{
					break;
				}
			}
			num++;
			FlsykUFpxm(higSectionAndIndex);
			num++;
			return new ToolTipViewModel_ItemCodeHoverTooltip(text3, result, itemCodeHoverInfoBase, HXHynMHZCa());
		}
		catch (Exception ex)
		{
			string msg = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("EditorHoverTooltip_ConfigFileError"), num, ex.Message);
			AppCore.Logger.Error(msg);
			return null;
		}
	}

	private bool jBcybHZMEM(List<KeyValuePair<string, ValidationSectionData>> list, DocumentLine P_1)
	{
		if (list == null || !list.Any())
		{
			return true;
		}
		Dictionary<string, string> dictionary = null;
		string[] array = null;
		int num = 0;
		foreach (KeyValuePair<string, ValidationSectionData> item in list)
		{
			if (item.Value.CurrentLine.HasValue && item.Value.CurrentLine.Value)
			{
				if (array == null)
				{
					string text = Document.GetText(P_1);
					if (string.IsNullOrEmpty(text))
					{
						return false;
					}
					array = text.Split("\t", StringSplitOptions.RemoveEmptyEntries);
					if (array == null || array.Length == 0)
					{
						return false;
					}
				}
				int num2 = (item.Value.Index.HasValue ? item.Value.Index.Value : 0);
				if (array.Length < num2)
				{
					return false;
				}
				if (array[num2] == item.Value.Value)
				{
					num++;
					continue;
				}
				return false;
			}
			if (dictionary == null)
			{
				ScriptFileCompilerOl scriptFileCompilerOl = new ScriptFileCompilerOl(AppCore.ViewModelBase.PVF);
				PvfFile pvfFile = new PvfFile(File.FileName);
				byte[] array2 = scriptFileCompilerOl.Compile(new PvfFile(File.FileName), Document.Text);
				if (array2 == null)
				{
					return false;
				}
				pvfFile.WriteFileData(array2);
				dictionary = scriptFileCompilerOl.DecompileDic(pvfFile);
				if (dictionary == null)
				{
					return false;
				}
			}
			if (dictionary.TryGetValue(item.Key, out var value))
			{
				if (item.Value.Index.HasValue)
				{
					if (!string.IsNullOrEmpty(value))
					{
						string[] array3 = value.Split("\t", StringSplitOptions.RemoveEmptyEntries);
						if (array3 != null && array3.Any() && item.Value.Index.Value < array3.Count() && array3[item.Value.Index.Value] == item.Value.Value)
						{
							num++;
						}
					}
				}
				else if (item.Value.Value == value.Replace("\t", string.Empty))
				{
					num++;
				}
				continue;
			}
			return false;
		}
		return list.Count == num;
	}

	private bool lncyIn1qdE()
	{
		return File.FileName == "etc/independent_drop.etc";
	}

	private ToolTipViewModel_ItemCodeHoverTooltip XKSyETmCoP(int P_0, DocumentLine P_1)
	{
		int index;
		HighlightedSection higSectionAndIndex = Editor.GetHigSectionAndIndex(HighlightingType.Digits, P_1, P_0, out index);
		if (higSectionAndIndex == null)
		{
			return null;
		}
		if (!yK6yPrZhI7(P_1.LineNumber, "[dungeon drop rate balance]", null))
		{
			string text = Document.GetText(higSectionAndIndex);
			if (text == "-1")
			{
				return null;
			}
			if (!int.TryParse(text, out var result))
			{
				return null;
			}
			string[] array = Document.GetText(P_1).Split("\t", StringSplitOptions.RemoveEmptyEntries);
			if (array.Count() == 17)
			{
				switch (index)
				{
				case 1:
					if (array[0] == "0")
					{
						FlsykUFpxm(higSectionAndIndex);
						return new ToolTipViewModel_ItemCodeHoverTooltip(Pvf.ListFileTable.ItemCodeConvertFilePath("monster", result), result, new ItemCodeHoverInfoDefault
						{
							Description = AppSetting.Instance.GetIlogger().GetStr("Luang_MonsterName")
						}, HXHynMHZCa());
					}
					if (array[0] == "1")
					{
						FlsykUFpxm(higSectionAndIndex);
						return new ToolTipViewModel_ItemCodeHoverTooltip(Pvf.ListFileTable.ItemCodeConvertFilePath("aicharacter", result), result, new ItemCodeHoverInfoDefault
						{
							Description = "APC"
						}, HXHynMHZCa());
					}
					break;
				case 2:
					if (result == 0)
					{
						break;
					}
					if (array[16] == "0")
					{
						FlsykUFpxm(higSectionAndIndex);
						string text2 = Pvf.ListFileTable.ItemCodeConvertFilePath("equipment", result);
						if (text2 == null)
						{
							text2 = Pvf.ListFileTable.ItemCodeConvertFilePath("stackable", result);
						}
						return new ToolTipViewModel_ItemCodeHoverTooltip(text2, result, new ItemCodeHoverInfoDefault
						{
							Description = AppSetting.Instance.GetIlogger()?.GetStr("EditorHoverTooltip_Description_DropItemName"),
							LstFileNames = new List<string>
							{
								"equipment",
								"stackable"
							}
						}, HXHynMHZCa());
					}
					return new ToolTipViewModel_ItemCodeHoverTooltip(AppSetting.Instance.GetIlogger()?.GetStr("mess_Independent_drop_OnlyZeroCanBeInputErr"), new ItemCodeHoverInfoDefault
					{
						Description = "错误信息"
					}, HXHynMHZCa());
				}
			}
			else
			{
				if (array.Count() != 2)
				{
					if (array.Count() == 1)
					{
						FlsykUFpxm(higSectionAndIndex);
						return new ToolTipViewModel_ItemCodeHoverTooltip(Pvf.ListFileTable.ItemCodeConvertFilePath("independentdrop", result), result, new ItemCodeHoverInfoDefault
						{
							Description = AppSetting.Instance.GetIlogger()?.GetStr("EditorHoverTooltip_Description_DropListName")
						}, HXHynMHZCa());
					}
					return null;
				}
				if (index == 0)
				{
					FlsykUFpxm(higSectionAndIndex);
					string text3 = Pvf.ListFileTable.ItemCodeConvertFilePath("equipment", result);
					if (text3 == null)
					{
						text3 = Pvf.ListFileTable.ItemCodeConvertFilePath("stackable", result);
					}
					return new ToolTipViewModel_ItemCodeHoverTooltip(text3, result, new ItemCodeHoverInfoDefault
					{
						Description = AppSetting.Instance.GetIlogger()?.GetStr("EditorHoverTooltip_Description_DropItemName"),
						LstFileNames = new List<string>
						{
							"equipment",
							"stackable"
						}
					}, HXHynMHZCa());
				}
			}
		}
		return null;
	}

	private bool X6VyOg2DWE(int P_0)
	{
		if (File.FileType == PvfFileType.ui && iukyJZJqp1(P_0, "[ui controls]", out var num) && Document.LineCount > num)
		{
			string text = Document.GetText(Document.GetLineByNumber(num + 1));
			if (!string.IsNullOrEmpty(text))
			{
				text.Replace(" ", "").Replace("\t", "");
				string text2 = text.Replace(" ", "").Replace("\t", "");
				if (text2 == "`[switchbox]`" || text2 == "`[balloon]`")
				{
					return true;
				}
			}
		}
		return false;
	}

	private ToolTipViewModelBase FtByKRZEQV(int P_0, DocumentLine P_1)
	{
		int index;
		HighlightedSection higSectionAndIndex = Editor.GetHigSectionAndIndex(HighlightingType.Digits, P_1, P_0, out index);
		if (higSectionAndIndex == null)
		{
			return null;
		}
		if (index != 4)
		{
			return null;
		}
		string text = Editor.Document.GetText(higSectionAndIndex);
		if (text == "-1")
		{
			return null;
		}
		if (!int.TryParse(text, out var result))
		{
			return null;
		}
		string? filePath = Pvf.ListFileTable.ItemCodeConvertFilePath("dungeon", result);
		FlsykUFpxm(higSectionAndIndex);
		return new ToolTipViewModel_ItemCodeHoverTooltip(info: new ItemCodeHoverInfoDefault
		{
			Description = AppSetting.Instance.GetIlogger().GetStr("EditorHoverTooltip_Description_DungeonInterface")
		}, filePath: filePath, itemCode: result, editorTooltipDataTemplateSelector: HXHynMHZCa());
	}

	private ToolTipViewModelBase hMZy9yAP9Z(int P_0, DocumentLine P_1)
	{
		int index;
		HighlightedSection higSectionAndIndex = Editor.GetHigSectionAndIndex(HighlightingType.Digits, P_1, P_0, out index);
		if (higSectionAndIndex == null)
		{
			return null;
		}
		if (index <= 2)
		{
			return null;
		}
		if (index % 2 == 0)
		{
			return null;
		}
		string text = Editor.Document.GetText(higSectionAndIndex);
		if (text == "-1")
		{
			return null;
		}
		if (!int.TryParse(text, out var result))
		{
			return null;
		}
		string text2 = Pvf.ListFileTable.ItemCodeConvertFilePath("equipment", result);
		if (string.IsNullOrEmpty(text2))
		{
			text2 = Pvf.ListFileTable.ItemCodeConvertFilePath("stackable", result);
		}
		FlsykUFpxm(higSectionAndIndex);
		ItemCodeHoverInfoBase info = new ItemCodeHoverInfoDefault
		{
			Description = AppSetting.Instance.GetIlogger()?.GetStr("EditorHoverTooltip_Description_SpecialPassiveObjectItem")
		};
		return new ToolTipViewModel_ItemCodeHoverTooltip(text2, result, info, HXHynMHZCa());
	}

	private bool yK6yPrZhI7(int P_0, string P_1, string P_2)
	{
		KeyValuePair<int, string>[] array = Editor.FoldingStrategyBaseHelper.lineNumberSectionDic.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			KeyValuePair<int, string> keyValuePair = array[i];
			if (keyValuePair.Key >= P_0)
			{
				continue;
			}
			if (!string.IsNullOrEmpty(P_2))
			{
				if (keyValuePair.Value != P_2 && keyValuePair.Value == P_1)
				{
					return true;
				}
				continue;
			}
			if (!(keyValuePair.Value == P_1))
			{
				break;
			}
			return true;
		}
		return false;
	}

	private string ivqyZliCJt(DocumentLine P_0, out int P_1)
	{
		P_1 = P_0.LineNumber;
		if (Editor == null || Editor.FoldingStrategyBaseHelper == null)
		{
			P_1 = -1;
			return null;
		}
		KeyValuePair<int, string>[] array = Editor.FoldingStrategyBaseHelper.lineNumberSectionDic.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			KeyValuePair<int, string> keyValuePair = array[i];
			if (keyValuePair.Key < P_1)
			{
				P_1 = keyValuePair.Key;
				return keyValuePair.Value;
			}
		}
		P_1 = -1;
		return null;
	}

	public bool iukyJZJqp1(int P_0, string P_1, out int P_2)
	{
		P_2 = -1;
		if (Editor.FoldingStrategyBaseHelper.HigSectionDic != null)
		{
			KeyValuePair<int, KeyValuePair<string, string>>[] array = Editor.FoldingStrategyBaseHelper.HigSectionDic.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				KeyValuePair<int, KeyValuePair<string, string>> keyValuePair = array[i];
				if (keyValuePair.Key < P_0 && keyValuePair.Value.Value == P_1)
				{
					P_2 = keyValuePair.Key;
					return true;
				}
			}
		}
		return false;
	}

	private void FlsykUFpxm(HighlightedSection P_0)
	{
		ItemCodeBackgroundRenderers itemCodeBackgroundRenderers = new ItemCodeBackgroundRenderers();
		itemCodeBackgroundRenderers.ItemTextSegment = new TextSegment
		{
			StartOffset = P_0.Offset,
			EndOffset = P_0.EndOffset,
			Length = P_0.Length
		};
		Editor.TextArea.TextView.BackgroundRenderers.Add(itemCodeBackgroundRenderers);
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
		Editor.TextArea.TextView.MouseHover -= YBWyL20Hlt;
		Editor.TextArea.TextView.MouseHoverStopped -= WDoysb9I3D;
		Editor.TextArea.TextView.MouseLeftButtonDown -= mKxy18IUB2;
		Editor.Unloaded -= Rxyy6hXjyq;
		ToolTip.MouseLeave -= aENyoWbWyi;
		ToolTip.MouseEnter -= wh4ywjdSjh;
		tooltipCloseTimer.Stop();
		tooltipCloseTimer.Tick -= TooltipCloseTimer_Tick;
		Application.Current.MainWindow.Deactivated -= vB8ygbr7b2;
		ToolTip.DataContext = null;
		ToolTip = null;
	}
}
