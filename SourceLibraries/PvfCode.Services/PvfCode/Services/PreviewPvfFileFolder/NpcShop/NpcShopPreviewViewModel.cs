using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using DevExpress.Mvvm.DataAnnotations;
using PvfCode.LoggerBase;
using Swordfish.NET.Collections;

namespace PvfCode.Services.PreviewPvfFileFolder.NpcShop;

public class NpcShopPreviewViewModel : FilePreviewDataBase
{
	public int NpcId { get; set; }

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
				return $"未知NPC：{NpcId}";
			}
			return base.Pvf.GetItemName(pvfFile);
		}
	}

	public ConcurrentObservableCollection<NpcTabItemViewModel> Items { get; set; }

	public NpcShopPreviewViewModel(PvfGroup pvf, PvfFile file, ImageSource? imageSource = null)
		: base(pvf, file, imageSource)
	{
		NpcId = -1;
		Items = new ConcurrentObservableCollection<NpcTabItemViewModel>();
	}

	[Command]
	public void OnPreviewLoaded()
	{
		Task.Run((Action)LoadItems);
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

	private void LoadItems()
	{
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
			stringBuilder.AppendLine($"商店物品数据类型只能是int 已忽略该值类型：{b} file://{base.File.FileName}");
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
		if (!base.File.GetSectionTypeIsStrArray(base.Pvf, "[tab name]", out List<string> tabNames))
		{
			tabNames = new List<string>();
		}
		int tabIndex = 0;
		ConcurrentObservableCollection<NpcShopPreviewItem> tabItems = new ConcurrentObservableCollection<NpcShopPreviewItem>();
		List<NpcTabItemViewModel> tabs = new List<NpcTabItemViewModel>();
		tabs.Add(new NpcTabItemViewModel(GetTabTitle(tabNames, tabIndex), tabItems)
		{
			IsSelected = true
		});
		foreach (int code in list)
		{
			if (code == -2)
			{
				tabItems = new ConcurrentObservableCollection<NpcShopPreviewItem>();
				tabIndex++;
				tabs.Add(new NpcTabItemViewModel(GetTabTitle(tabNames, tabIndex), tabItems));
			}
			else
			{
				tabItems.Add(new NpcShopPreviewItem(code, Pvf));
			}
		}
		Items.AddRange(tabs);
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

	private static string GetTabTitle(List<string> tabNames, int tabIndex)
	{
		string text = "商店";
		if (tabNames != null && tabNames.Any() && tabIndex < tabNames.Count)
		{
			text = tabNames[tabIndex];
			if (string.IsNullOrEmpty(text))
			{
				text = "商店";
			}
		}
		return text;
	}
}
