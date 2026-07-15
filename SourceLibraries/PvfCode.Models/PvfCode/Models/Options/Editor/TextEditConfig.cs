using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Newtonsoft.Json;
using PvfCode.Models.CodeCompletionModels;
using PvfCode.Models.Enums;
using PvfCode.Models.Options.Enums;
using PvfCode.Models.Pvf.Enums;
using UnitComboLib;
using UnitComboLib.Models;
using UnitComboLib.Models.Unit;
using UnitComboLib.Models.Unit.Screen;
using UnitComboLib.ViewModels;

namespace PvfCode.Models.Options.Editor;

[JsonObject(MemberSerialization.OptOut)]
public class TextEditConfig : ModelBase
{
	private EncodingType? nutDefaultEncodingType;

	private bool? openKorStrQuote;

	private bool addEndTab;

	private bool? lstDocumentNameGoFileNeedCtrl;

	private bool? showFirstColumnTab;

	private double? previewAniPanelWidth;

	private double? previewAniPanelHeight;

	private bool? showAniPreviewPanel;

	private ItemCodeConvertItemNameConfiger itemCodeConvertItemNameConfiger;

	private HashSet<string>? notUseFileListTooltip;

	private List<CodeCompletionData> completionDatas;

	private List<CodeCompletionData> completionDatasDisk;

	private HoverTooltipMode? editorCommentHoverTooltipMode;

	private HoverTooltipMode? editorItemCodeHoverTooltipMode;

	private bool saveBeautifyNutCode;

	private IUnitViewModel sizeUnitLabel;

	private int? truncateLongLineLength;

	private HashSet<string> allowLinkHighlightedTheName;

	private bool showSpaces;

	private bool? showTabs;

	private bool showEndOfLine;

	private bool wordWrap;

	private bool? enableTextDragDrop;

	private bool? enableVirtualSpace;

	private bool useAutoCodeFolding;

	private Dictionary<int, SolidColorBrush> foldingGuideLineBrushes;

	private bool? useFoldingGuideLines;

	private bool searchPanelKeywordConvertTw;

	private bool searchPanelNameConvertCode;

	private bool? searchPanelFindNotFoundAllowMessageBox;

	private Dictionary<ThemeType, PvfEdiorHighlightingColorOptions>? pvfEditorColorOptionDictionary;

	public EncodingType NutDefaultEncodingType
	{
		get
		{
			EncodingType? encodingType = nutDefaultEncodingType;
			if (encodingType.HasValue)
			{
				return nutDefaultEncodingType.Value;
			}
			return EncodingType.KR;
		}
		set
		{
			nutDefaultEncodingType = value;
			DoNotify("NutDefaultEncodingType");
		}
	}

	public bool OpenKorStrQuote
	{
		get
		{
			if (!openKorStrQuote.HasValue)
			{
				openKorStrQuote = true;
			}
			return openKorStrQuote.Value;
		}
		set
		{
			openKorStrQuote = value;
			DoNotify("OpenKorStrQuote");
		}
	}

	public bool AddEndTab
	{
		get
		{
			return addEndTab;
		}
		set
		{
			addEndTab = value;
			DoNotify("AddEndTab");
		}
	}

	public bool LstDocumentNameGoFileNeedCtrl
	{
		get
		{
			if (lstDocumentNameGoFileNeedCtrl.HasValue)
			{
				return lstDocumentNameGoFileNeedCtrl.Value;
			}
			return true;
		}
		set
		{
			lstDocumentNameGoFileNeedCtrl = value;
			DoNotify("LstDocumentNameGoFileNeedCtrl");
		}
	}

	public bool ShowFirstColumnTab
	{
		get
		{
			if (!showFirstColumnTab.HasValue)
			{
				showFirstColumnTab = true;
			}
			return showFirstColumnTab.Value;
		}
		set
		{
			showFirstColumnTab = value;
			DoNotify("ShowFirstColumnTab");
		}
	}

	public int FirstColumnTab => ShowFirstColumnTab ? 1 : 0;

	public double PreviewAniPanelWidht
	{
		get
		{
			if (!previewAniPanelWidth.HasValue)
			{
				previewAniPanelWidth = 200.0;
			}
			return previewAniPanelWidth.Value;
		}
		set
		{
			previewAniPanelWidth = value;
			DoNotify("PreviewAniPanelWidht");
		}
	}

	public double PreviewAniPanelHeight
	{
		get
		{
			if (!previewAniPanelHeight.HasValue)
			{
				previewAniPanelHeight = 200.0;
			}
			return previewAniPanelHeight.Value;
		}
		set
		{
			previewAniPanelHeight = value;
			DoNotify("PreviewAniPanelHeight");
		}
	}

	[JsonIgnore]
	public bool ShowAniPreviewPanel
	{
		get
		{
			if (showAniPreviewPanel.HasValue)
			{
				return showAniPreviewPanel.Value;
			}
			return true;
		}
		set
		{
			showAniPreviewPanel = value;
			DoNotify("ShowAniPreviewPanel");
		}
	}

	[JsonIgnore]
	public ItemCodeConvertItemNameConfiger ItemCodeConvertItemNameConfiger
	{
		get
		{
			if (itemCodeConvertItemNameConfiger == null)
			{
				itemCodeConvertItemNameConfiger = new ItemCodeConvertItemNameConfiger();
			}
			return itemCodeConvertItemNameConfiger;
		}
		set
		{
			itemCodeConvertItemNameConfiger = value;
			DoNotify("ItemCodeConvertItemNameConfiger");
		}
	}

	[JsonIgnore]
	public HashSet<string> NotUseFileListTooltip
	{
		get
		{
			if (notUseFileListTooltip == null)
			{
				notUseFileListTooltip = new HashSet<string>();
				notUseFileListTooltip.Add("aicharactername.lst");
				notUseFileListTooltip.Add("itemname.lst");
				notUseFileListTooltip.Add("monstername.lst");
				notUseFileListTooltip.Add("npcname.lst");
				notUseFileListTooltip.Add("passiveobjectname.lst");
				notUseFileListTooltip.Add("skillname0.lst");
				notUseFileListTooltip.Add("skillname1.lst");
				notUseFileListTooltip.Add("skillname2.lst");
				notUseFileListTooltip.Add("skillname3.lst");
				notUseFileListTooltip.Add("skillname4.lst");
				notUseFileListTooltip.Add("skillname5.lst");
				notUseFileListTooltip.Add("skillname6.lst");
				notUseFileListTooltip.Add("skillname7.lst");
				notUseFileListTooltip.Add("skillname8.lst");
				notUseFileListTooltip.Add("skillname9.lst");
				notUseFileListTooltip.Add("n_quest/epicquest.lst");
			}
			return notUseFileListTooltip;
		}
		set
		{
			notUseFileListTooltip = value;
		}
	}

	[JsonIgnore]
	public List<CodeCompletionData> CompletionDatas
	{
		get
		{
			if (completionDatas == null)
			{
				completionDatas = new List<CodeCompletionData>();
			}
			return completionDatas;
		}
		set
		{
			completionDatas = value;
		}
	}

	public List<CodeCompletionData> CompletionDatasDisk
	{
		get
		{
			if (completionDatasDisk == null)
			{
				completionDatasDisk = new List<CodeCompletionData>();
			}
			return completionDatasDisk;
		}
		set
		{
			completionDatasDisk = value;
		}
	}

	public HoverTooltipMode EditorCommentHoverTooltipMode
	{
		get
		{
			if (!editorCommentHoverTooltipMode.HasValue)
			{
				editorCommentHoverTooltipMode = HoverTooltipMode.鼠标移入;
			}
			return editorCommentHoverTooltipMode.Value;
		}
		set
		{
			editorCommentHoverTooltipMode = value;
			DoNotify("EditorCommentHoverTooltipMode");
		}
	}

	public HoverTooltipMode EditorItemCodeHoverTooltipMode
	{
		get
		{
			if (!editorItemCodeHoverTooltipMode.HasValue)
			{
				editorItemCodeHoverTooltipMode = HoverTooltipMode.鼠标移入;
			}
			return editorItemCodeHoverTooltipMode.Value;
		}
		set
		{
			editorItemCodeHoverTooltipMode = value;
			DoNotify("EditorItemCodeHoverTooltipMode");
		}
	}

	public bool SaveBeautifyNutCode
	{
		get
		{
			return saveBeautifyNutCode;
		}
		set
		{
			saveBeautifyNutCode = value;
		}
	}

	public IUnitViewModel SizeUnitLabel
	{
		get
		{
			if (sizeUnitLabel == null)
			{
				ObservableCollection<ListItem> list = new ObservableCollection<ListItem>(GenerateScreenUnitList());
				sizeUnitLabel = UnitViewModeService.CreateInstance(list, new ScreenConverter(), 0, 100.0, "#####");
			}
			_ = sizeUnitLabel.SelectedItem.DefaultValues;
			return sizeUnitLabel;
		}
		set
		{
			sizeUnitLabel = value;
			DoNotify("SizeUnitLabel");
		}
	}

	public int TruncateLongLineLength
	{
		get
		{
			if (!truncateLongLineLength.HasValue)
			{
				truncateLongLineLength = 2000;
			}
			return truncateLongLineLength.Value;
		}
		set
		{
			truncateLongLineLength = value;
			DoNotify("TruncateLongLineLength");
		}
	}

	[JsonIgnore]
	public HashSet<string> AllowLinkHighlightedTheName
	{
		get
		{
			if (allowLinkHighlightedTheName == null)
			{
				allowLinkHighlightedTheName = new HashSet<string>
				{
					"String",
					"Section",
					"SectionEnd"
				};
			}
			return allowLinkHighlightedTheName;
		}
	}

	public bool ShowSpaces
	{
		get
		{
			return showSpaces;
		}
		set
		{
			showSpaces = value;
			DoNotify("ShowSpaces");
		}
	}

	public bool ShowTabs
	{
		get
		{
			if (showTabs.HasValue)
			{
				return showTabs.Value;
			}
			return true;
		}
		set
		{
			showTabs = value;
			DoNotify("ShowTabs");
		}
	}

	public bool ShowEndOfLine
	{
		get
		{
			return showEndOfLine;
		}
		set
		{
			showEndOfLine = value;
			DoNotify("ShowEndOfLine");
		}
	}

	public bool WordWrap
	{
		get
		{
			return wordWrap;
		}
		set
		{
			wordWrap = value;
			DoNotify("WordWrap");
		}
	}

	public bool EnableTextDragDrop
	{
		get
		{
			if (!enableTextDragDrop.HasValue)
			{
				enableTextDragDrop = true;
			}
			return enableTextDragDrop.Value;
		}
		set
		{
			enableTextDragDrop = value;
			DoNotify("EnableTextDragDrop");
		}
	}

	public bool EnableVirtualSpace
	{
		get
		{
			if (!enableVirtualSpace.HasValue)
			{
				enableVirtualSpace = false;
			}
			return enableVirtualSpace.Value;
		}
		set
		{
			enableVirtualSpace = value;
			DoNotify("EnableVirtualSpace");
		}
	}

	public bool UseAutoCodeFolding
	{
		get
		{
			return useAutoCodeFolding;
		}
		set
		{
			useAutoCodeFolding = value;
			DoNotify("UseAutoCodeFolding");
		}
	}

	public bool UseFoldingGuideLines
	{
		get
		{
			if (!useFoldingGuideLines.HasValue)
			{
				useFoldingGuideLines = true;
			}
			return useFoldingGuideLines.Value;
		}
		set
		{
			useFoldingGuideLines = value;
			DoNotify("UseFoldingGuideLines");
		}
	}

	[JsonIgnore]
	public Dictionary<int, SolidColorBrush> FoldingGuideLineBrushs
	{
		get
		{
			if (foldingGuideLineBrushes == null)
			{
				foldingGuideLineBrushes = new Dictionary<int, SolidColorBrush>();
			}
			return foldingGuideLineBrushes;
		}
		set
		{
			foldingGuideLineBrushes = value;
		}
	}

	public bool SearchPanelKeywordConvertTW
	{
		get
		{
			return searchPanelKeywordConvertTw;
		}
		set
		{
			searchPanelKeywordConvertTw = value;
			DoNotify("_SearchPanelKeywordConvertTW");
		}
	}

	public bool SearchPanelNameConvertCode
	{
		get
		{
			return searchPanelNameConvertCode;
		}
		set
		{
			searchPanelNameConvertCode = value;
		}
	}

	public bool SearchPanelFindNotFoundAllowMessageBox
	{
		get
		{
			if (!searchPanelFindNotFoundAllowMessageBox.HasValue)
			{
				searchPanelFindNotFoundAllowMessageBox = true;
			}
			return searchPanelFindNotFoundAllowMessageBox.Value;
		}
		set
		{
			searchPanelFindNotFoundAllowMessageBox = value;
			DoNotify("SearchPanelFindNotFoundAllowMessageBox");
		}
	}

	public Dictionary<ThemeType, PvfEdiorHighlightingColorOptions> PvfEditorColorOptionDic
	{
		get
		{
			if (pvfEditorColorOptionDictionary == null)
			{
				pvfEditorColorOptionDictionary = new Dictionary<ThemeType, PvfEdiorHighlightingColorOptions>();
			}
			foreach (object value in Enum.GetValues(typeof(ThemeType)))
			{
				ThemeType themeType = (ThemeType)Enum.Parse(typeof(ThemeType), value.ToString());
				if (!pvfEditorColorOptionDictionary.ContainsKey(themeType))
				{
					pvfEditorColorOptionDictionary.Add(themeType, new PvfEdiorHighlightingColorOptions(themeType));
				}
			}
			return pvfEditorColorOptionDictionary;
		}
		set
		{
			pvfEditorColorOptionDictionary = value;
			DoNotify("PvfEditorColorOptionDic");
		}
	}

	[JsonIgnore]
	public PvfEdiorHighlightingColorOptions NowEditorHighlightingColorOption
	{
		get
		{
			if (PvfEditorColorOptionDic.TryGetValue(AppSetting.Instance.NowThemeType, out PvfEdiorHighlightingColorOptions value))
			{
				return value;
			}
			return null;
		}
	}

	public void InitCompletionDatas(List<CodeCompletionData> items)
	{
		CompletionDatas.AddRange(CompletionDatasDisk);
		foreach (CodeCompletionData item in items)
		{
			if (CompletionDatas.FindIndex(existing => existing.Text == item.Text && existing.CodeCompletScriptType == item.CodeCompletScriptType) == -1)
			{
				CompletionDatas.Add(item);
			}
		}
	}

	public void SaveCompletionDatas(CodeCompletionData data)
	{
		int num = CompletionDatasDisk.FindIndex(item => item.Text == data.Text && item.CodeCompletScriptType == data.CodeCompletScriptType);
		if (num != -1)
		{
			CompletionDatasDisk.RemoveAt(num);
		}
		CompletionDatasDisk.Add(data);
		num = CompletionDatas.FindIndex(item => item.Text == data.Text && item.CodeCompletScriptType == data.CodeCompletScriptType);
		if (num != -1)
		{
			CompletionDatas.RemoveAt(num);
		}
		CompletionDatas.Add(data);
	}

	public bool DeleteCompletionData(CodeCompletionData data)
	{
		int num = CompletionDatasDisk.FindIndex(item => item.HighlightingType == data.HighlightingType && item.Text == data.Text && item.CodeCompletScriptType == data.CodeCompletScriptType);
		if (num != -1)
		{
			CompletionDatasDisk.RemoveAt(num);
			num = CompletionDatas.FindIndex(item => item.HighlightingType == data.HighlightingType && item.Text == data.Text && item.CodeCompletScriptType == data.CodeCompletScriptType);
			if (num != -1)
			{
				CompletionDatas.RemoveAt(num);
			}
			return true;
		}
		return false;
	}

	public IEnumerable<ListItem> GenerateScreenUnitList()
	{
		return new List<ListItem>
		{
			new ListItem(defaultValues: new ObservableCollection<string>
			{
				"25",
				"50",
				"75",
				"100",
				"125",
				"150",
				"175",
				"200",
				"300",
				"400",
				"500"
			}, key: Itemkey.ScreenPercent, displayNameLong: "Percent", displayNameShort: "%")
		};
	}

	public void InitFoldingGuideLineBurshs(Control control)
	{
		FoldingGuideLineBrushs = new Dictionary<int, SolidColorBrush>();
		SolidColorBrush solidColorBrush = (SolidColorBrush)control.FindResource("FoldingGuideLineBrush0");
		if (solidColorBrush != null)
		{
			((Freezable)solidColorBrush).Freeze();
		}
		FoldingGuideLineBrushs.Add(0, solidColorBrush);
		SolidColorBrush solidColorBrush2 = (SolidColorBrush)control.FindResource("FoldingGuideLineBrush1");
		if (solidColorBrush2 != null)
		{
			((Freezable)solidColorBrush2).Freeze();
		}
		FoldingGuideLineBrushs.Add(1, solidColorBrush2);
		SolidColorBrush solidColorBrush3 = (SolidColorBrush)control.FindResource("FoldingGuideLineBrush2");
		if (solidColorBrush3 != null)
		{
			((Freezable)solidColorBrush3).Freeze();
		}
		FoldingGuideLineBrushs.Add(2, solidColorBrush3);
		SolidColorBrush solidColorBrush4 = (SolidColorBrush)control.FindResource("FoldingGuideLineBrush3");
		if (solidColorBrush4 != null)
		{
			((Freezable)solidColorBrush4).Freeze();
		}
		FoldingGuideLineBrushs.Add(3, solidColorBrush4);
		SolidColorBrush solidColorBrush5 = (SolidColorBrush)control.FindResource("FoldingGuideLineBrush4");
		if (solidColorBrush5 != null)
		{
			((Freezable)solidColorBrush5).Freeze();
		}
		FoldingGuideLineBrushs.Add(4, solidColorBrush5);
	}

	public SolidColorBrush GetFoldingGuideLineBrush(int level)
	{
		if (level > 4)
		{
			level = 4;
		}
		if (!FoldingGuideLineBrushs.TryGetValue(level, out SolidColorBrush value))
		{
			return AppSetting.Instance.ToColor("red");
		}
		return value;
	}

	public void ChangedPvfEdiorHighlightingColorOptions()
	{
		DoNotify("NowEditorHighlightingColorOption");
	}

	public TextEditConfig()
	{
	}
}
