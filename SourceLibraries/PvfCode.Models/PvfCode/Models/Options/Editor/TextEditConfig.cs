using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
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
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass50_0
	{
		public CodeCompletionData Upw5df1MtG;

		public _003C_003Ec__DisplayClass50_0()
		{
		}

		internal bool QIi5Jce35m(CodeCompletionData it)
		{
			if (it.Text == Upw5df1MtG.Text)
			{
				return it.CodeCompletScriptType == Upw5df1MtG.CodeCompletScriptType;
			}
			return false;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass51_0
	{
		public CodeCompletionData uU75O3pq8C;

		public _003C_003Ec__DisplayClass51_0()
		{
		}

		internal bool JOv51NLUd9(CodeCompletionData it)
		{
			if (it.Text == uU75O3pq8C.Text)
			{
				return it.CodeCompletScriptType == uU75O3pq8C.CodeCompletScriptType;
			}
			return false;
		}

		internal bool cPk5GO4XBa(CodeCompletionData it)
		{
			if (it.Text == uU75O3pq8C.Text)
			{
				return it.CodeCompletScriptType == uU75O3pq8C.CodeCompletScriptType;
			}
			return false;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass52_0
	{
		public CodeCompletionData TXh5jqKZ02;

		public _003C_003Ec__DisplayClass52_0()
		{
		}

		internal bool YiF5rE6ijf(CodeCompletionData it)
		{
			if (it.HighlightingType == TXh5jqKZ02.HighlightingType && it.Text == TXh5jqKZ02.Text)
			{
				return it.CodeCompletScriptType == TXh5jqKZ02.CodeCompletScriptType;
			}
			return false;
		}

		internal bool Bhf5eRdPky(CodeCompletionData it)
		{
			if (it.HighlightingType == TXh5jqKZ02.HighlightingType && it.Text == TXh5jqKZ02.Text)
			{
				return it.CodeCompletScriptType == TXh5jqKZ02.CodeCompletScriptType;
			}
			return false;
		}
	}

	private EncodingType? TcZEwPsTmd;

	private bool? FZvEoySkNO;

	private bool jj7E2gQDhr;

	private bool? CRPEtX0lwY;

	private bool? IejE9IZcZh;

	private double? BagERYg9wV;

	private double? LZ4EW0s9qU;

	private bool? Jx5Eq7Y4hX;

	private ItemCodeConvertItemNameConfiger JFTEzWMfiJ;

	private HashSet<string>? hSdZA2nvPg;

	private List<CodeCompletionData> hjrZnTcx7S;

	private List<CodeCompletionData> zdPZkJSD5S;

	private HoverTooltipMode? q51ZL9Qp5i;

	private HoverTooltipMode? YsYZEu5dln;

	private bool jWDZZMlSCy;

	private IUnitViewModel HjoZ84NhEg;

	private int? YYSZuLeyRN;

	private HashSet<string> gjxZ573ZTP;

	private bool IjrZpvK9nV;

	private bool? i3hZDCHvTA;

	private bool FEeZ3FyP8S;

	private bool vWRZHVx5XQ;

	private bool? noGZ7g41Qq;

	private bool? BiMZcLY5UQ;

	private bool fbBZgy7RXy;

	private Dictionary<int, SolidColorBrush> wKDZKmTjBK;

	private bool? K2pZY3ddc7;

	private bool IglZJdtSj9;

	private bool TjTZdJPXAP;

	private bool? IZhZ1lfvwn;

	private Dictionary<ThemeType, PvfEdiorHighlightingColorOptions>? m2vZG9pInd;

	public EncodingType NutDefaultEncodingType
	{
		get
		{
			EncodingType? tcZEwPsTmd = TcZEwPsTmd;
			if (tcZEwPsTmd.HasValue)
			{
				return TcZEwPsTmd.Value;
			}
			return EncodingType.KR;
		}
		set
		{
			TcZEwPsTmd = value;
			DoNotify("NutDefaultEncodingType");
		}
	}

	public bool OpenKorStrQuote
	{
		get
		{
			if (!FZvEoySkNO.HasValue)
			{
				FZvEoySkNO = true;
			}
			return FZvEoySkNO.Value;
		}
		set
		{
			FZvEoySkNO = value;
			DoNotify("OpenKorStrQuote");
		}
	}

	public bool AddEndTab
	{
		get
		{
			return jj7E2gQDhr;
		}
		set
		{
			jj7E2gQDhr = value;
			DoNotify("AddEndTab");
		}
	}

	public bool LstDocumentNameGoFileNeedCtrl
	{
		get
		{
			if (CRPEtX0lwY.HasValue)
			{
				return CRPEtX0lwY.Value;
			}
			return true;
		}
		set
		{
			CRPEtX0lwY = value;
			DoNotify("LstDocumentNameGoFileNeedCtrl");
		}
	}

	public bool ShowFirstColumnTab
	{
		get
		{
			if (!IejE9IZcZh.HasValue)
			{
				IejE9IZcZh = true;
			}
			return IejE9IZcZh.Value;
		}
		set
		{
			IejE9IZcZh = value;
			DoNotify("ShowFirstColumnTab");
		}
	}

	public int FirstColumnTab => ShowFirstColumnTab ? 1 : 0;

	public double PreviewAniPanelWidht
	{
		get
		{
			if (!BagERYg9wV.HasValue)
			{
				BagERYg9wV = 200.0;
			}
			return BagERYg9wV.Value;
		}
		set
		{
			BagERYg9wV = value;
			DoNotify("PreviewAniPanelWidht");
		}
	}

	public double PreviewAniPanelHeight
	{
		get
		{
			if (!LZ4EW0s9qU.HasValue)
			{
				LZ4EW0s9qU = 200.0;
			}
			return LZ4EW0s9qU.Value;
		}
		set
		{
			LZ4EW0s9qU = value;
			DoNotify("PreviewAniPanelHeight");
		}
	}

	[JsonIgnore]
	public bool ShowAniPreviewPanel
	{
		get
		{
			if (Jx5Eq7Y4hX.HasValue)
			{
				return Jx5Eq7Y4hX.Value;
			}
			return true;
		}
		set
		{
			Jx5Eq7Y4hX = value;
			DoNotify("ShowAniPreviewPanel");
		}
	}

	[JsonIgnore]
	public ItemCodeConvertItemNameConfiger ItemCodeConvertItemNameConfiger
	{
		get
		{
			if (JFTEzWMfiJ == null)
			{
				JFTEzWMfiJ = new ItemCodeConvertItemNameConfiger();
			}
			return JFTEzWMfiJ;
		}
		set
		{
			JFTEzWMfiJ = value;
			DoNotify("ItemCodeConvertItemNameConfiger");
		}
	}

	[JsonIgnore]
	public HashSet<string> NotUseFileListTooltip
	{
		get
		{
			if (hSdZA2nvPg == null)
			{
				hSdZA2nvPg = new HashSet<string>();
				hSdZA2nvPg.Add("aicharactername.lst");
				hSdZA2nvPg.Add("itemname.lst");
				hSdZA2nvPg.Add("monstername.lst");
				hSdZA2nvPg.Add("npcname.lst");
				hSdZA2nvPg.Add("passiveobjectname.lst");
				hSdZA2nvPg.Add("skillname0.lst");
				hSdZA2nvPg.Add("skillname1.lst");
				hSdZA2nvPg.Add("skillname2.lst");
				hSdZA2nvPg.Add("skillname3.lst");
				hSdZA2nvPg.Add("skillname4.lst");
				hSdZA2nvPg.Add("skillname5.lst");
				hSdZA2nvPg.Add("skillname6.lst");
				hSdZA2nvPg.Add("skillname7.lst");
				hSdZA2nvPg.Add("skillname8.lst");
				hSdZA2nvPg.Add("skillname9.lst");
				hSdZA2nvPg.Add("n_quest/epicquest.lst");
			}
			return hSdZA2nvPg;
		}
		set
		{
			hSdZA2nvPg = value;
		}
	}

	[JsonIgnore]
	public List<CodeCompletionData> CompletionDatas
	{
		get
		{
			if (hjrZnTcx7S == null)
			{
				hjrZnTcx7S = new List<CodeCompletionData>();
			}
			return hjrZnTcx7S;
		}
		set
		{
			hjrZnTcx7S = value;
		}
	}

	public List<CodeCompletionData> CompletionDatasDisk
	{
		get
		{
			if (zdPZkJSD5S == null)
			{
				zdPZkJSD5S = new List<CodeCompletionData>();
			}
			return zdPZkJSD5S;
		}
		set
		{
			zdPZkJSD5S = value;
		}
	}

	public HoverTooltipMode EditorCommentHoverTooltipMode
	{
		get
		{
			if (!q51ZL9Qp5i.HasValue)
			{
				q51ZL9Qp5i = HoverTooltipMode.鼠标移入;
			}
			return q51ZL9Qp5i.Value;
		}
		set
		{
			q51ZL9Qp5i = value;
			DoNotify("EditorCommentHoverTooltipMode");
		}
	}

	public HoverTooltipMode EditorItemCodeHoverTooltipMode
	{
		get
		{
			if (!YsYZEu5dln.HasValue)
			{
				YsYZEu5dln = HoverTooltipMode.鼠标移入;
			}
			return YsYZEu5dln.Value;
		}
		set
		{
			YsYZEu5dln = value;
			DoNotify("EditorItemCodeHoverTooltipMode");
		}
	}

	public bool SaveBeautifyNutCode
	{
		get
		{
			return jWDZZMlSCy;
		}
		set
		{
			jWDZZMlSCy = value;
		}
	}

	public IUnitViewModel SizeUnitLabel
	{
		get
		{
			if (HjoZ84NhEg == null)
			{
				ObservableCollection<ListItem> list = new ObservableCollection<ListItem>(GenerateScreenUnitList());
				HjoZ84NhEg = UnitViewModeService.CreateInstance(list, new ScreenConverter(), 0, 100.0, "#####");
			}
			_ = HjoZ84NhEg.SelectedItem.DefaultValues;
			return HjoZ84NhEg;
		}
		set
		{
			HjoZ84NhEg = value;
			DoNotify("SizeUnitLabel");
		}
	}

	public int TruncateLongLineLength
	{
		get
		{
			if (!YYSZuLeyRN.HasValue)
			{
				YYSZuLeyRN = 2000;
			}
			return YYSZuLeyRN.Value;
		}
		set
		{
			YYSZuLeyRN = value;
			DoNotify("TruncateLongLineLength");
		}
	}

	[JsonIgnore]
	public HashSet<string> AllowLinkHighlightedTheName
	{
		get
		{
			if (gjxZ573ZTP == null)
			{
				gjxZ573ZTP = new HashSet<string>
				{
					"String",
					"Section",
					"SectionEnd"
				};
			}
			return gjxZ573ZTP;
		}
	}

	public bool ShowSpaces
	{
		get
		{
			return IjrZpvK9nV;
		}
		set
		{
			IjrZpvK9nV = value;
			DoNotify("ShowSpaces");
		}
	}

	public bool ShowTabs
	{
		get
		{
			if (i3hZDCHvTA.HasValue)
			{
				return i3hZDCHvTA.Value;
			}
			return true;
		}
		set
		{
			i3hZDCHvTA = value;
			DoNotify("ShowTabs");
		}
	}

	public bool ShowEndOfLine
	{
		get
		{
			return FEeZ3FyP8S;
		}
		set
		{
			FEeZ3FyP8S = value;
			DoNotify("ShowEndOfLine");
		}
	}

	public bool WordWrap
	{
		get
		{
			return vWRZHVx5XQ;
		}
		set
		{
			vWRZHVx5XQ = value;
			DoNotify("WordWrap");
		}
	}

	public bool EnableTextDragDrop
	{
		get
		{
			if (!noGZ7g41Qq.HasValue)
			{
				noGZ7g41Qq = true;
			}
			return noGZ7g41Qq.Value;
		}
		set
		{
			noGZ7g41Qq = value;
			DoNotify("EnableTextDragDrop");
		}
	}

	public bool EnableVirtualSpace
	{
		get
		{
			if (!BiMZcLY5UQ.HasValue)
			{
				BiMZcLY5UQ = false;
			}
			return BiMZcLY5UQ.Value;
		}
		set
		{
			BiMZcLY5UQ = value;
			DoNotify("EnableVirtualSpace");
		}
	}

	public bool UseAutoCodeFolding
	{
		get
		{
			return fbBZgy7RXy;
		}
		set
		{
			fbBZgy7RXy = value;
			DoNotify("UseAutoCodeFolding");
		}
	}

	public bool UseFoldingGuideLines
	{
		get
		{
			if (!K2pZY3ddc7.HasValue)
			{
				K2pZY3ddc7 = true;
			}
			return K2pZY3ddc7.Value;
		}
		set
		{
			K2pZY3ddc7 = value;
			DoNotify("UseFoldingGuideLines");
		}
	}

	[JsonIgnore]
	public Dictionary<int, SolidColorBrush> FoldingGuideLineBrushs
	{
		get
		{
			if (wKDZKmTjBK == null)
			{
				wKDZKmTjBK = new Dictionary<int, SolidColorBrush>();
			}
			return wKDZKmTjBK;
		}
		set
		{
			wKDZKmTjBK = value;
		}
	}

	public bool SearchPanelKeywordConvertTW
	{
		get
		{
			return IglZJdtSj9;
		}
		set
		{
			IglZJdtSj9 = value;
			DoNotify("_SearchPanelKeywordConvertTW");
		}
	}

	public bool SearchPanelNameConvertCode
	{
		get
		{
			return TjTZdJPXAP;
		}
		set
		{
			TjTZdJPXAP = value;
		}
	}

	public bool SearchPanelFindNotFoundAllowMessageBox
	{
		get
		{
			if (!IZhZ1lfvwn.HasValue)
			{
				IZhZ1lfvwn = true;
			}
			return IZhZ1lfvwn.Value;
		}
		set
		{
			IZhZ1lfvwn = value;
			DoNotify("SearchPanelFindNotFoundAllowMessageBox");
		}
	}

	public Dictionary<ThemeType, PvfEdiorHighlightingColorOptions> PvfEditorColorOptionDic
	{
		get
		{
			if (m2vZG9pInd == null)
			{
				m2vZG9pInd = new Dictionary<ThemeType, PvfEdiorHighlightingColorOptions>();
			}
			foreach (object value in Enum.GetValues(typeof(ThemeType)))
			{
				ThemeType themeType = (ThemeType)Enum.Parse(typeof(ThemeType), value.ToString());
				if (!m2vZG9pInd.ContainsKey(themeType))
				{
					m2vZG9pInd.Add(themeType, new PvfEdiorHighlightingColorOptions(themeType));
				}
			}
			return m2vZG9pInd;
		}
		set
		{
			m2vZG9pInd = value;
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
		using List<CodeCompletionData>.Enumerator enumerator = items.GetEnumerator();
		while (enumerator.MoveNext())
		{
			_003C_003Ec__DisplayClass50_0 CS_0024_003C_003E8__locals4 = new _003C_003Ec__DisplayClass50_0();
			CS_0024_003C_003E8__locals4.Upw5df1MtG = enumerator.Current;
			if (CompletionDatas.FindIndex((CodeCompletionData it) => it.Text == CS_0024_003C_003E8__locals4.Upw5df1MtG.Text && it.CodeCompletScriptType == CS_0024_003C_003E8__locals4.Upw5df1MtG.CodeCompletScriptType) == -1)
			{
				CompletionDatas.Add(CS_0024_003C_003E8__locals4.Upw5df1MtG);
			}
		}
	}

	public void SaveCompletionDatas(CodeCompletionData data)
	{
		_003C_003Ec__DisplayClass51_0 CS_0024_003C_003E8__locals7 = new _003C_003Ec__DisplayClass51_0();
		CS_0024_003C_003E8__locals7.uU75O3pq8C = data;
		int num = CompletionDatasDisk.FindIndex((CodeCompletionData it) => it.Text == CS_0024_003C_003E8__locals7.uU75O3pq8C.Text && it.CodeCompletScriptType == CS_0024_003C_003E8__locals7.uU75O3pq8C.CodeCompletScriptType);
		if (num != -1)
		{
			CompletionDatasDisk.RemoveAt(num);
		}
		CompletionDatasDisk.Add(CS_0024_003C_003E8__locals7.uU75O3pq8C);
		num = CompletionDatas.FindIndex((CodeCompletionData it) => it.Text == CS_0024_003C_003E8__locals7.uU75O3pq8C.Text && it.CodeCompletScriptType == CS_0024_003C_003E8__locals7.uU75O3pq8C.CodeCompletScriptType);
		if (num != -1)
		{
			CompletionDatas.RemoveAt(num);
		}
		CompletionDatas.Add(CS_0024_003C_003E8__locals7.uU75O3pq8C);
	}

	public bool DeleteCompletionData(CodeCompletionData data)
	{
		_003C_003Ec__DisplayClass52_0 CS_0024_003C_003E8__locals7 = new _003C_003Ec__DisplayClass52_0();
		CS_0024_003C_003E8__locals7.TXh5jqKZ02 = data;
		int num = CompletionDatasDisk.FindIndex((CodeCompletionData it) => it.HighlightingType == CS_0024_003C_003E8__locals7.TXh5jqKZ02.HighlightingType && it.Text == CS_0024_003C_003E8__locals7.TXh5jqKZ02.Text && it.CodeCompletScriptType == CS_0024_003C_003E8__locals7.TXh5jqKZ02.CodeCompletScriptType);
		if (num != -1)
		{
			CompletionDatasDisk.RemoveAt(num);
			num = CompletionDatas.FindIndex((CodeCompletionData it) => it.HighlightingType == CS_0024_003C_003E8__locals7.TXh5jqKZ02.HighlightingType && it.Text == CS_0024_003C_003E8__locals7.TXh5jqKZ02.Text && it.CodeCompletScriptType == CS_0024_003C_003E8__locals7.TXh5jqKZ02.CodeCompletScriptType);
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
