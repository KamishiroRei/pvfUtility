using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Mvvm.DataAnnotations;
using PvfCode.LoggerBase;
using PvfCode.Models.Pvf;
using PvfCode.Models.Pvf.Enums;
using PvfCode.Services.PvfParsingNew;

namespace PvfCode.Services.PreviewPvfFileFolder;

public class FilePreviewData_Equ : StackableOrEquPreviewBase
{
	private PvfFile equGroupMasterFile;

	private int EquGroupMasterItemCode { get; set; }

	public EquipmentType? EquipmentTypeEnum
	{
		get
		{
			if (base.File.GetEquipmentType(base.Pvf, out var re))
			{
				return re;
			}
			return null;
		}
	}

	public string? EquArmorType
	{
		get
		{
			if (base.File.GetEquipmentType(base.Pvf, out var re))
			{
				EquipmentType value = re.Value;
				if ((uint)(value - 2) <= 4u)
				{
					if (base.File.GetEquArmorType(base.Pvf, out var equArmorType))
					{
						return equArmorType.ToString();
					}
					return null;
				}
			}
			return null;
		}
	}

	public string? Durability
	{
		get
		{
			if (base.File.GetDurability(base.Pvf, out string durability))
			{
				return "耐久度 " + durability + "/" + durability;
			}
			return null;
		}
	}

	public bool IsAvatar
	{
		get
		{
			EquipmentType? equipmentTypeEnum = EquipmentTypeEnum;
			if (!equipmentTypeEnum.HasValue)
			{
				return false;
			}
			EquipmentType value = equipmentTypeEnum.Value;
			if ((uint)(value - 17) <= 9u)
			{
				return true;
			}
			return false;
		}
	}

	public string? ItemGroupNameStr
	{
		get
		{
			if (base.File.GetItemGroupName(base.Pvf, out var itemGroupName))
			{
				return itemGroupName?.ToString();
			}
			return null;
		}
	}

	public string? avatar_select_ability
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (base.Pvf.Getavatar_select_abilityItems(base.File, out List<avatar_select_ability_Base> items))
			{
				foreach (avatar_select_ability_Base item in items)
				{
					stringBuilder.AppendLine(item.Text);
				}
			}
			if (stringBuilder.Length <= 0)
			{
				return null;
			}
			return stringBuilder.ToString();
		}
	}

	public List<int> GroupItemCodeList { get; set; }

	public EquGroupType EquGroupType
	{
		get
		{
			return GetProperty(() => EquGroupType);
		}
		set
		{
			SetProperty(() => EquGroupType, value);
		}
	}

	public string EquGroupName { get; set; }

	public List<EquGroupItemAttributes> EquGroupItemInfos { get; set; }

	public string? EquWhiteAttributes => PvfFilePreviewHelper.GetEquWhiteAttributes(base.ScriptFileParser, base.ScriptFileParser.Sections, base.Pvf);

	public string? EquBlueAttributes => PvfFilePreviewHelper.EquBlueAttributes(base.ScriptFileParser, base.ScriptFileParser.Sections, base.Pvf);

	public string? EquGrouphiteAttributes
	{
		get
		{
			if (EquGroupType == EquGroupType.Root || EquGroupType == EquGroupType.Children)
			{
				ScriptFileParserNew scriptFileParserNew = base.ScriptFileParser;
				if (EquGroupType == EquGroupType.Children)
				{
					if (equGroupMasterFile == null)
					{
						return $"找不到套装主文件：{EquGroupMasterItemCode}";
					}
					scriptFileParserNew = new ScriptFileParserNew(equGroupMasterFile, base.Pvf);
					scriptFileParserNew.PraseStructureMain();
				}
				if (scriptFileParserNew != null && scriptFileParserNew.Sections != null && scriptFileParserNew.Sections.Any())
				{
					SectionBase sectionBase = scriptFileParserNew.Sections.Where((SectionBase it) => it.GetSectionName() == "[set ability]").FirstOrDefault();
					if (sectionBase != null)
					{
						return PvfFilePreviewHelper.GetEquWhiteAttributes(scriptFileParserNew, sectionBase.Children, base.Pvf);
					}
				}
			}
			return null;
		}
	}

	public string? EquGroupBlueAttributes
	{
		get
		{
			if (EquGroupType == EquGroupType.Root || EquGroupType == EquGroupType.Children)
			{
				ScriptFileParserNew scriptFileParserNew = base.ScriptFileParser;
				if (EquGroupType == EquGroupType.Children)
				{
					if (equGroupMasterFile == null)
					{
						return $"找不到套装主文件：{EquGroupMasterItemCode}";
					}
					scriptFileParserNew = new ScriptFileParserNew(equGroupMasterFile, base.Pvf);
					scriptFileParserNew.PraseStructureMain();
				}
				if (scriptFileParserNew != null && scriptFileParserNew.Sections != null && scriptFileParserNew.Sections.Any())
				{
					SectionBase sectionBase = scriptFileParserNew.Sections.Where((SectionBase it) => it.GetSectionName() == "[set ability]").FirstOrDefault();
					if (sectionBase != null)
					{
						return PvfFilePreviewHelper.EquBlueAttributes(scriptFileParserNew, sectionBase.Children, base.Pvf);
					}
				}
			}
			return null;
		}
	}

	public string? EquGroupAttributesText
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder();
			string equGrouphiteAttributes = EquGrouphiteAttributes;
			if (!string.IsNullOrEmpty(equGrouphiteAttributes))
			{
				stringBuilder.Append(equGrouphiteAttributes);
			}
			string equGroupBlueAttributes = EquGroupBlueAttributes;
			if (!string.IsNullOrEmpty(equGroupBlueAttributes))
			{
				stringBuilder.Append(equGroupBlueAttributes);
			}
			if (stringBuilder.Length <= 0)
			{
				return null;
			}
			return stringBuilder.ToString();
		}
	}

	public string? FullsetBasicExplain { get; set; }

	public EquPartsetFile EquPartsetFile { get; set; }

	public FilePreviewData_Equ(PvfGroup pvf, PvfFile file, ImageSource? imageSource = null)
		: base(pvf, file, imageSource)
	{
	}

	public override void InitEquGroup()
	{
		int stringTableId = base.Pvf.Strtable.GetStringTableId("[piece set ability]");
		if (stringTableId != -1 && base.File.FindSectionIndex(stringTableId, out var _))
		{
			EquGroupType = EquGroupType.GroupPart;
			Dictionary<string, EquipmentPartSet> value2;
			if (base.File.GetSectionIntArray(base.Pvf, "[set item]", out List<int> items))
			{
				EquGroupType = EquGroupType.GroupPart2;
				Dictionary<string, EquipmentPartSet> dictionary = new Dictionary<string, EquipmentPartSet>();
				int num = 0;
				foreach (int item in items)
				{
					string text = base.Pvf.ListFileTable.ItemCodeConvertFilePath("equipment", item);
					if (string.IsNullOrEmpty(text) || !base.Pvf.FileList.TryGetValue(text, out PvfFile value))
					{
						EquPartsetFile equPartsetFile = new EquPartsetFile(null, base.Pvf, null);
						equPartsetFile.Name = $"装备代码：{item}对应文件不存在";
						EquPartsetFile = equPartsetFile;
					}
					else
					{
						num++;
						EquipmentPartSet equipmentPartSet = new EquipmentPartSet
						{
							ParFile = base.File,
							Name = base.Pvf.GetItemName(value)
						};
						equipmentPartSet.ReferencesFiles.Add(new EquipmentPartSet.ReferencesRowViewModel(value));
						dictionary.Add(num.ToString(), equipmentPartSet);
					}
				}
				if (dictionary.Count == 0)
				{
					EquPartsetFile = new EquPartsetFile(null, base.Pvf, null)
					{
						Name = "装备代码：" + string.Join(",", items) + "对应文件不存在"
					};
				}
				else
				{
					EquPartsetFile = new EquPartsetFile(base.File, base.Pvf, dictionary);
				}
			}
			else if (base.Pvf.EquipmentPartSetTable.PathItems.TryGetValue(base.File.FileName, out value2))
			{
				EquPartsetFile = new EquPartsetFile(base.File, base.Pvf, value2);
			}
			return;
		}
		if (base.File.GetSectionIntValue("[part set index]", base.Pvf, out var val))
		{
			EquGroupType = EquGroupType.Link;
			if (base.Pvf.EquipmentPartSetTable.Items.TryGetValue(val, out Dictionary<string, EquipmentPartSet> value3))
			{
				EquPartsetFile = new EquPartsetFile(value3.FirstOrDefault().Value?.ParFile, base.Pvf, value3);
				return;
			}
			EquPartsetFile equPartsetFile2 = new EquPartsetFile(null, base.Pvf, null);
			equPartsetFile2.Name = $"套装索引：{val} 未注册到：etc/equipmentpartset.etc";
			EquPartsetFile = equPartsetFile2;
			return;
		}
		int val2;
		if (base.File.GetSectionIntArray(base.Pvf, "[set item]", out List<int> items2))
		{
			EquGroupType = EquGroupType.Root;
			if (items2.Count > 0)
			{
				GroupItemCodeList = items2;
				if (base.File.ItemCode.HasValue)
				{
					EquGroupMasterItemCode = base.File.ItemCode.Value;
					equGroupMasterFile = base.File;
				}
			}
			else
			{
				EquGroupName = "套装主文件未设定 ：[set item]";
			}
		}
		else if (base.File.GetSectionIntValue("[set item master]", base.Pvf, out val2))
		{
			EquGroupMasterItemCode = val2;
			EquGroupType = EquGroupType.Children;
		}
		if (EquGroupType == EquGroupType.None)
		{
			return;
		}
		if (equGroupMasterFile == null)
		{
			string text2 = base.Pvf.ListFileTable.ItemCodeConvertFilePath("equipment", EquGroupMasterItemCode);
			if (string.IsNullOrEmpty(text2))
			{
				EquGroupName = "套装主代码文件不存在：" + EquGroupMasterItemCode;
			}
			else
			{
				equGroupMasterFile = base.Pvf.GetFile(text2);
			}
		}
		string name;
		if (equGroupMasterFile == null)
		{
			EquGroupName = "套装主代码文件不存在：" + EquGroupMasterItemCode;
		}
		else if (equGroupMasterFile.GetNameText(base.Pvf, "[set name]", out name))
		{
			if (!string.IsNullOrEmpty(name))
			{
				EquGroupName = name;
			}
			else
			{
				EquGroupName = "未设定 套装名称";
			}
		}
		else
		{
			EquGroupName = "未设定 套装名称";
		}
		LoadEquGroupItems();
		if (equGroupMasterFile != null && equGroupMasterFile.GetNameText(base.Pvf, "[fullset basic explain]", out string name2))
		{
			FullsetBasicExplain = name2?.Replace("\\n", "\r\n");
		}
	}

	private void LoadEquGroupItems()
	{
		if (equGroupMasterFile == null)
		{
			return;
		}
		if (EquGroupType == EquGroupType.Children)
		{
			if (!equGroupMasterFile.GetSectionIntArray(base.Pvf, "[set item]", out List<int> items) || items.Count == 0)
			{
				EquGroupName = "套装主文件未设定 [set item]";
				return;
			}
			GroupItemCodeList = items;
		}
		if (GroupItemCodeList != null && GroupItemCodeList.Count != 0)
		{
			EquGroupItemInfos = new List<EquGroupItemAttributes>();
			EquGroupItemInfos.Add(new EquGroupItemAttributes(equGroupMasterFile, isRoot: true, base.Pvf, EquGroupMasterItemCode));
			for (int i = 1; i < GroupItemCodeList.Count; i++)
			{
				PvfFile file = base.Pvf.GetFile(base.Pvf.ListFileTable.ItemCodeConvertFilePath("equipment", GroupItemCodeList[i]));
				EquGroupItemInfos.Add(new EquGroupItemAttributes(file, isRoot: false, base.Pvf, GroupItemCodeList[i]));
			}
		}
	}

	[Command]
	public async void OnOpenEquGroupFile(EquGroupItemAttributes? equGroupItemInfo)
	{
		List<EquGroupItemAttributes> list = ((equGroupItemInfo == null) ? EquGroupItemInfos : new List<EquGroupItemAttributes> { equGroupItemInfo });
		bool flag = (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
		Ilogger ilogger = AppSetting.Instance.GetIlogger();
		if (list == null)
		{
			return;
		}
		if (flag)
		{
			if (equGroupItemInfo != null)
			{
				await ilogger.AddFileListToCurrentSearchPanel(from it in list
					where it.File != null
					select it.File.FileName);
			}
			else
			{
				ilogger.AddFileListToNewSearchPanel(from it in list
					where it.File != null
					select it.File.FileName, EquGroupName);
			}
			return;
		}
		if (equGroupItemInfo != null)
		{
			if (equGroupItemInfo.File != null)
			{
				ilogger.OpenPvfFileDocument(equGroupItemInfo.File.FileName);
			}
			else
			{
				ilogger.Error("套装文件不存在无法打开 ： " + equGroupItemInfo.ItemCode);
			}
			return;
		}
		string text = null;
		foreach (EquGroupItemAttributes item in list)
		{
			if (item.File != null)
			{
				if (item.ItemCode == EquGroupMasterItemCode)
				{
					text = item.File.FileName;
				}
				else
				{
					ilogger.OpenPvfFileDocument(item.File.FileName);
				}
			}
			else
			{
				ilogger.Error("套装文件不存在无法打开 ： " + item.ItemCode);
			}
		}
		if (text != null)
		{
			ilogger.SetDocumentFocused(text);
		}
	}
}
