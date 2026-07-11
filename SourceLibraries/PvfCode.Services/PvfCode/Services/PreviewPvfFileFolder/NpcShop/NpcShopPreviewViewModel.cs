using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using DevExpress.Mvvm.DataAnnotations;
using PvfCode.LoggerBase;
using Swordfish.NET.Collections;

namespace PvfCode.Services.PreviewPvfFileFolder.NpcShop;

public class NpcShopPreviewViewModel : FilePreviewDataBase
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass13_0
	{
		public ConcurrentObservableCollection<NpcShopPreviewItem> nkry9DLiHg;

		public int G44yqHbwvm;

		public List<NpcTabItemViewModel> KaNygkcLm5;

		public List<string> uwqyzON891;

		public NpcShopPreviewViewModel xMEHuXWi65;

		public _003C_003Ec__DisplayClass13_0()
		{
		}

		internal void zQjydtoER6(int code)
		{
			if (code == -2)
			{
				nkry9DLiHg = new ConcurrentObservableCollection<NpcShopPreviewItem>();
				G44yqHbwvm++;
				KaNygkcLm5.Add(new NpcTabItemViewModel(lPLMYWeuja(uwqyzON891, G44yqHbwvm), nkry9DLiHg));
			}
			else
			{
				nkry9DLiHg.Add(new NpcShopPreviewItem(code, xMEHuXWi65.Pvf));
			}
		}
	}

	[CompilerGenerated]
	private int UQDM8PeMv4;

	[CompilerGenerated]
	private ConcurrentObservableCollection<NpcTabItemViewModel> IxRMjs78DY;

	public int NpcId
	{
		[CompilerGenerated]
		get
		{
			return UQDM8PeMv4;
		}
		[CompilerGenerated]
		set
		{
			UQDM8PeMv4 = value;
		}
	}

	public string NpcName
	{
		get
		{
			if (NpcId == -1)
			{
				return "未设定NPC";
			}
			PvfFile pvfFile = base.Pvf.ListFileTable.ItemCodeConvertPvfFile(base.Pvf, NpcId, new string[1] { "npc" });
			if (pvfFile == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(6, 1);
				defaultInterpolatedStringHandler.AppendLiteral("未知NPC：");
				defaultInterpolatedStringHandler.AppendFormatted(NpcId);
				return defaultInterpolatedStringHandler.ToStringAndClear();
			}
			return base.Pvf.GetItemName(pvfFile);
		}
	}

	public ConcurrentObservableCollection<NpcTabItemViewModel> Items
	{
		[CompilerGenerated]
		get
		{
			return IxRMjs78DY;
		}
		[CompilerGenerated]
		set
		{
			IxRMjs78DY = value;
		}
	}

	public NpcShopPreviewViewModel(PvfGroup pvf, PvfFile file, ImageSource? imageSource = null)
		: base(pvf, file, imageSource)
	{
		NpcId = -1;
		Items = new ConcurrentObservableCollection<NpcTabItemViewModel>();
	}

	[Command]
	public void OnPreviewLoaded()
	{
		Task.Run((Action)XCKMl91ujh);
	}

	[Command]
	public void OnOpenNpcShopEditor()
	{
		Ilogger ilogger = AppSetting.Instance.GetIlogger();
		if (base.File != null)
		{
			ilogger?.OpenNpcShopEditor(base.File.FileName);
		}
	}

	private void XCKMl91ujh()
	{
		_003C_003Ec__DisplayClass13_0 CS_0024_003C_003E8__locals19 = new _003C_003Ec__DisplayClass13_0();
		CS_0024_003C_003E8__locals19.xMEHuXWi65 = this;
		ConcurrentObservableCollection<NpcTabItemViewModel> items = Items;
		if (items != null && items.Any())
		{
			return;
		}
		Ilogger ilogger = AppSetting.Instance.GetIlogger();
		if (base.File == null)
		{
			ilogger.Error("NPC商店预览创建失败 文件不能为NULL");
			return;
		}
		int stringTableId = base.Pvf.Strtable.GetStringTableId("[sell item]");
		if (stringTableId == -1 || !base.File.FindSectionIndex(stringTableId, out var indexOut))
		{
			Items = new ConcurrentObservableCollection<NpcTabItemViewModel>();
			return;
		}
		byte[] data = base.File.Data;
		List<int> list = new List<int>();
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = indexOut + 5; i < base.File.DataLen; i += 5)
		{
			byte b = data[i];
			if (data[i] == 5)
			{
				break;
			}
			if (b == 2 && i < base.File.DataLen)
			{
				list.Add(BitConverter.ToInt32(base.File.Data, i + 1));
				continue;
			}
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(31, 2, stringBuilder2);
			handler.AppendLiteral("商店物品数据类型只能是int 已忽略该值类型：");
			handler.AppendFormatted(b);
			handler.AppendLiteral(" file://");
			handler.AppendFormatted(base.File.FileName);
			stringBuilder2.AppendLine(ref handler);
		}
		if (stringBuilder.Length > 0)
		{
			ilogger.Error(stringBuilder.ToString());
		}
		if (base.File.GetNpcId(base.Pvf, out var npcId))
		{
			NpcId = npcId;
			RaisePropertyChanged("NpcName");
		}
		if (!base.File.GetSectionTypeIsStrArray(base.Pvf, "[tab name]", out CS_0024_003C_003E8__locals19.uwqyzON891))
		{
			CS_0024_003C_003E8__locals19.uwqyzON891 = new List<string>();
		}
		CS_0024_003C_003E8__locals19.G44yqHbwvm = 0;
		CS_0024_003C_003E8__locals19.nkry9DLiHg = new ConcurrentObservableCollection<NpcShopPreviewItem>();
		CS_0024_003C_003E8__locals19.KaNygkcLm5 = new List<NpcTabItemViewModel>();
		CS_0024_003C_003E8__locals19.KaNygkcLm5.Add(new NpcTabItemViewModel(lPLMYWeuja(CS_0024_003C_003E8__locals19.uwqyzON891, CS_0024_003C_003E8__locals19.G44yqHbwvm), CS_0024_003C_003E8__locals19.nkry9DLiHg)
		{
			IsSelected = true
		});
		list.ForEach(delegate(int code)
		{
			if (code == -2)
			{
				CS_0024_003C_003E8__locals19.nkry9DLiHg = new ConcurrentObservableCollection<NpcShopPreviewItem>();
				CS_0024_003C_003E8__locals19.G44yqHbwvm++;
				CS_0024_003C_003E8__locals19.KaNygkcLm5.Add(new NpcTabItemViewModel(lPLMYWeuja(CS_0024_003C_003E8__locals19.uwqyzON891, CS_0024_003C_003E8__locals19.G44yqHbwvm), CS_0024_003C_003E8__locals19.nkry9DLiHg));
			}
			else
			{
				CS_0024_003C_003E8__locals19.nkry9DLiHg.Add(new NpcShopPreviewItem(code, CS_0024_003C_003E8__locals19.xMEHuXWi65.Pvf));
			}
		});
		Items.AddRange(CS_0024_003C_003E8__locals19.KaNygkcLm5);
	}

	public static bool Create(PvfFile file, PvfGroup pvf, out NpcShopPreviewViewModel? npcShopPreviewViewModel)
	{
		npcShopPreviewViewModel = null;
		if (file == null)
		{
			return false;
		}
		npcShopPreviewViewModel = new NpcShopPreviewViewModel(pvf, file);
		return true;
	}

	private static string lPLMYWeuja(List<string> P_0, int P_1)
	{
		string text = "商店";
		if (P_0 != null && P_0.Any() && P_1 < P_0.Count)
		{
			text = P_0[P_1];
			if (string.IsNullOrEmpty(text))
			{
				text = "商店";
			}
		}
		return text;
	}
}
