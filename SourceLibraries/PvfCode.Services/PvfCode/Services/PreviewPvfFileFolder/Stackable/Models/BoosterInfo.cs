using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using PvfCode.Models.Pvf;
using PvfCode.Services.PvfParsingNew;
using Swordfish.NET.Collections;

namespace PvfCode.Services.PreviewPvfFileFolder.Stackable.Models;

public class BoosterInfo : ViewModelBase
{
	public class Avatar : BoosterInfoItemBase
	{
		public int Value1 { get; set; }

		public int Value2 { get; set; }

		public Avatar(int itemCode, int weigh, int itemNumber, int value1, int value2)
			: base(itemCode, weigh, itemNumber)
		{
			Value1 = value1;
			Value2 = value2;
		}

		public new static bool Create(BoosterType boosterType, List<SectionBase> datas, out BoosterInfoItemRoot? result, out string? error)
		{
			result = null;
			if ((datas.Count - 1) % 5 != 0)
			{
				error = "数据长度不正确！";
				return false;
			}
			if (datas[0].Item.Type != ScriptType.Int)
			{
				error = $"起始数据类型不正确 必须为 int 类型：{datas[0].Item.Type} pos:0";
				return false;
			}
			StringBuilder stringBuilder = new StringBuilder();
			List<BoosterInfoItemBase> list = new List<BoosterInfoItemBase>();
			for (int i = 1; i < datas.Count; i += 5)
			{
				if (datas[i].Item.Type != ScriptType.Int)
				{
					stringBuilder.AppendLine($"数据类型不正确 必须为int 类型：{datas[i].Item.Type} pos:{i + 2}");
				}
				if (datas[i + 1].Item.Type != ScriptType.Int)
				{
					stringBuilder.AppendLine($"数据类型不正确 必须为int 类型：{datas[i].Item.Type} pos:{i + 3}");
				}
				if (datas[i + 2].Item.Type != ScriptType.Int)
				{
					stringBuilder.AppendLine($"数据类型不正确 必须为int 类型：{datas[i].Item.Type} pos:{i + 4}");
				}
				if (datas[i + 3].Item.Type != ScriptType.Int)
				{
					stringBuilder.AppendLine($"数据类型不正确 必须为int 类型：{datas[i].Item.Type} pos:{i + 5}");
				}
				if (datas[i + 4].Item.Type != ScriptType.Int)
				{
					stringBuilder.AppendLine($"数据类型不正确 必须为int 类型：{datas[i].Item.Type} pos:{i + 6}");
				}
				Special_Avatar item = new Special_Avatar(datas[i].Item.Data, datas[i + 1].Item.Data, datas[i + 2].Item.Data, datas[i + 3].Item.Data, datas[i + 4].Item.Data);
				list.Add(item);
			}
			error = ((stringBuilder.Length > 0) ? stringBuilder.ToString() : null);
			if (list.Count > 0)
			{
				result = new BoosterInfoItemRoot(datas[0].Item.Data, list)
				{
					Type = boosterType
				};
				return true;
			}
			return false;
		}
	}

	public class Special_Avatar : Avatar
	{
		public Special_Avatar(int itemCode, int weigh, int itemNumber, int value1, int value2)
			: base(itemCode, weigh, itemNumber, value1, value2)
		{
		}
	}

	public class BoosterInfoItemBase
	{
		public int GainCount { get; set; }

		public int ItemCode { get; set; }

		public int Weigh { get; set; }

		public int ItemNumber { get; set; }

		public virtual string ItemName { get; set; }

		public BoosterInfoItemBase(int itemCode, int weigh, int itemNumber)
		{
			ItemCode = itemCode;
			Weigh = weigh;
			ItemNumber = itemNumber;
		}

		public static bool Create(BoosterType boosterType, List<SectionBase> datas, out BoosterInfoItemRoot? result, out string? error)
		{
			result = null;
			if ((datas.Count - 1) % 3 != 0)
			{
				error = "数据长度不正确！";
				return false;
			}
			if (datas[0].Item.Type != ScriptType.Int)
			{
				error = $"起始数据类型不正确 必须为 int 类型：{datas[0].Item.Type} pos:0";
				return false;
			}
			StringBuilder stringBuilder = new StringBuilder();
			List<BoosterInfoItemBase> list = new List<BoosterInfoItemBase>();
			for (int i = 1; i < datas.Count; i += 3)
			{
				if (datas[i].Item.Type != ScriptType.Int)
				{
					stringBuilder.AppendLine($"数据类型不正确 必须为int 类型：{datas[i].Item.Type} pos:{i + 2}");
				}
				if (datas[i + 1].Item.Type != ScriptType.Int)
				{
					stringBuilder.AppendLine($"数据类型不正确 必须为int 类型：{datas[i].Item.Type} pos:{i + 3}");
				}
				if (datas[i + 2].Item.Type != ScriptType.Int)
				{
					stringBuilder.AppendLine($"数据类型不正确 必须为int 类型：{datas[i].Item.Type} pos:{i + 4}");
				}
				BoosterInfoItemBase item = new BoosterInfoItemBase(datas[i].Item.Data, datas[i + 1].Item.Data, datas[i + 2].Item.Data);
				list.Add(item);
			}
			error = ((stringBuilder.Length > 0) ? stringBuilder.ToString() : null);
			if (list.Count > 0)
			{
				result = new BoosterInfoItemRoot(datas[0].Item.Data, list)
				{
					Type = boosterType
				};
				return true;
			}
			return false;
		}
	}

	public class BoosterInfoItemRoot
	{
		public BoosterType Type { get; set; }

		public string? Title { get; set; }

		public int? PrimaryCategoryIndex { get; set; }

		public int? SecondaryCategoryIndex { get; set; }

		public int GainCount { get; set; }

		public List<BoosterInfoItemBase> Items { get; set; }

		public BoosterInfoItemRoot(int gainCount, List<BoosterInfoItemBase> items)
		{
			GainCount = gainCount;
			Items = items;
		}

		public List<KeyValuePair<int, PvfFile>>? GetFiles(PvfGroup pvf)
		{
			if (Items == null || Items.Count == 0)
			{
				return null;
			}
			List<KeyValuePair<int, PvfFile>> list = new List<KeyValuePair<int, PvfFile>>();
			ListFileTable listFileTable = pvf.ListFileTable;
			foreach (BoosterInfoItemBase item in Items)
			{
				string text = listFileTable.ItemCodeConvertFilePath(item.ItemCode);
				if (string.IsNullOrEmpty(text))
				{
					list.Add(new KeyValuePair<int, PvfFile>(item.ItemNumber, null));
				}
				else
				{
					list.Add(new KeyValuePair<int, PvfFile>(item.ItemNumber, pvf.GetFile(text)));
				}
			}
			if (list.Count <= 0)
			{
				return null;
			}
			return list;
		}
	}

	public class PreviewItemListVm
	{
		public string Title { get; set; }

		public ConcurrentObservableCollection<FileItemIconPreviewViewModel> FilePreviewItems { get; set; }

		public FileItemIconPreviewViewModel CurrentItem { get; set; }

		public PreviewItemListVm(List<KeyValuePair<int, PvfFile>> files, PvfGroup pvf, string title)
		{
			Title = title;
			FilePreviewItems = new ConcurrentObservableCollection<FileItemIconPreviewViewModel>();
			List<FileItemIconPreviewViewModel> previewItems = new List<FileItemIconPreviewViewModel>();
			files?.ForEach(file =>
			{
				previewItems.Add(new FileItemIconPreviewViewModel(pvf, file.Value, file.Key));
			});
			FilePreviewItems.AddRange(previewItems);
		}
	}

	private readonly PvfGroup pvf;

	private readonly PvfFile file;

	public int SelectionMenuLevel { get; set; }

	public int PrimaryCategoryCount { get; set; }

	public int SecondaryCategoryCount { get; set; }

	public string? SelectionPrompt { get; set; }

	public string? SecondarySelectionPrompt { get; set; }

	public List<BoosterInfoItemRoot> Items { get; set; }

	public ObservableCollection<PreviewItemListVm> PreviewItemListVmItems
	{
		get
		{
			return GetProperty(() => PreviewItemListVmItems);
		}
		set
		{
			SetProperty<ObservableCollection<PreviewItemListVm>>(() => PreviewItemListVmItems, value);
		}
	}

	public bool IsLoading
	{
		get
		{
			return GetProperty(() => IsLoading);
		}
		set
		{
			SetProperty(() => IsLoading, value);
		}
	}

	public bool ShowPreviewItemListVmItems
	{
		get
		{
			return GetProperty(() => ShowPreviewItemListVmItems);
		}
		set
		{
			SetProperty(() => ShowPreviewItemListVmItems, value);
		}
	}

	public string Text => GetText();

	public BoosterInfo(PvfGroup pvf, PvfFile file)
	{
		this.pvf = pvf;
		this.file = file;
		Items = new List<BoosterInfoItemRoot>();
	}

	public string GetText()
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (SelectionMenuLevel > 0)
		{
			stringBuilder.Append($"{SelectionMenuLevel}级自选菜单：{PrimaryCategoryCount}项");
			if (SelectionMenuLevel == 2)
			{
				stringBuilder.Append($" x {SecondaryCategoryCount}项");
			}
			stringBuilder.AppendLine();
		}
		Dictionary<BoosterType, int> dictionary = new Dictionary<BoosterType, int>();
		foreach (BoosterInfoItemRoot item in Items)
		{
			if (dictionary.ContainsKey(item.Type))
			{
				dictionary[item.Type]++;
			}
			else
			{
				dictionary.Add(item.Type, 1);
			}
		}
		foreach (KeyValuePair<BoosterType, int> boosterCount in dictionary)
		{
			int itemCount = Items.Where(it => it.Type == boosterCount.Key).Sum(it => it.Items.Count);
			stringBuilder.AppendLine($"{BoosterTypeToName(boosterCount.Key)} [{boosterCount.Value}]种 {itemCount}个物品");
		}
		return stringBuilder.ToString();
	}

	[Command]
	public void OnAddAllFilesToSearchPanel()
	{
		if ((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.Control)
		{
			OnShowFileItems();
		}
		else
		{
			if (Items == null || Items.Count == 0)
			{
				return;
			}
			HashSet<int> itemCodes = new HashSet<int>();
			Items.ForEach(item =>
			{
				item.Items.ForEach(itemData =>
				{
					itemCodes.Add(itemData.ItemCode);
				});
			});
			List<string> list = pvf.ListFileTable.ItemCodeConvertFilePath(itemCodes);
			if (list != null)
			{
				list.Add(file.FileName);
				string itemName = pvf.GetItemName(file);
				AppSetting.Instance.GetIlogger()?.AddFileListToNewSearchPanel(list, string.IsNullOrEmpty(itemName) ? file.FileName : itemName);
			}
		}
	}

	[Command]
	public async void OnShowFileItems()
	{
		ShowPreviewItemListVmItems = true;
		if (PreviewItemListVmItems != null)
		{
			ShowPreviewItemListVmItems = true;
			return;
		}
		IsLoading = true;
		ObservableCollection<PreviewItemListVm> previewItems = new ObservableCollection<PreviewItemListVm>();
		await Task.Run(() =>
		{
			Items.ForEach(item =>
			{
				List<KeyValuePair<int, PvfFile>> files = item.GetFiles(pvf);
				previewItems.Add(new PreviewItemListVm(files, pvf, item.Title ?? BoosterTypeToName(item.Type)));
			});
		});
		IsLoading = false;
		PreviewItemListVmItems = previewItems;
	}

	[Command]
	public void OnCloseFileItemsPanel()
	{
		ShowPreviewItemListVmItems = false;
	}

	public static bool StrConvertBoosterType(string str, out BoosterType? type)
	{
		if (str != null)
		{
			switch (str.Length)
			{
			case 11:
				switch (str[1])
				{
				case 'e':
					if (!(str == "[equipment]"))
					{
						break;
					}
					type = BoosterType.equipment;
					return true;
				case 's':
					if (!(str == "[stackable]"))
					{
						break;
					}
					type = BoosterType.Stackable;
					return true;
				}
				break;
			case 8:
				switch (str[1])
				{
				case 'a':
					if (!(str == "[avatar]"))
					{
						break;
					}
					type = BoosterType.Avatar;
					return true;
				case 'e':
					if (!(str == "[emblem]"))
					{
						break;
					}
					type = BoosterType.Emblem;
					return true;
				}
				break;
			case 10:
				if (!(str == "[creature]"))
				{
					break;
				}
				type = BoosterType.Creature;
				return true;
			case 5:
				if (!(str == "[etc]"))
				{
					break;
				}
				type = BoosterType.ETC;
				return true;
			case 6:
				if (!(str == "[cera]"))
				{
					break;
				}
				type = BoosterType.Cera;
				return true;
			case 16:
				if (!(str == "[special avatar]"))
				{
					break;
				}
				type = BoosterType.Special_Avatar;
				return true;
			}
		}
		type = null;
		return false;
	}

	public static string BoosterTypeToName(BoosterType boosterType)
	{
		return boosterType switch
		{
			BoosterType.Creature => "宠物", 
			BoosterType.ETC => "其他", 
			BoosterType.equipment => "装备", 
			BoosterType.Cera => "商城", 
			BoosterType.Special_Avatar => "特殊装扮", 
			BoosterType.Avatar => "装扮", 
			BoosterType.Stackable => "道具", 
			BoosterType.Emblem => "徽章", 
			_ => "未知BoosterType", 
		};
	}

	public static bool CreateSelectionItemRoot(BoosterType boosterType, IReadOnlyList<int> values, int groupLength, string title, int primaryCategoryIndex, int secondaryCategoryIndex, out BoosterInfoItemRoot? result, out string? error)
	{
		result = null;
		error = null;
		if (groupLength < 2)
		{
			error = "自选礼盒项目列数不能小于2。";
			return false;
		}
		if (values.Count < 2)
		{
			error = "自选礼盒项目缺少物品ID或数量。";
			return false;
		}

		List<BoosterInfoItemBase> items = new List<BoosterInfoItemBase>();
		for (int i = 0; i + groupLength <= values.Count; i += groupLength)
		{
			items.Add(new BoosterInfoItemBase(values[i], 0, values[i + 1]));
		}
		if (items.Count == 0)
		{
			error = "自选礼盒项目中没有可预览的物品。";
			return false;
		}

		result = new BoosterInfoItemRoot(1, items)
		{
			Type = boosterType,
			Title = title,
			PrimaryCategoryIndex = primaryCategoryIndex,
			SecondaryCategoryIndex = secondaryCategoryIndex
		};
		if (values.Count % groupLength != 0)
		{
			error = $"自选礼盒项目数据长度 {values.Count} 不是每组 {groupLength} 列的整数倍，末尾数据已忽略。";
		}
		return true;
	}
}
