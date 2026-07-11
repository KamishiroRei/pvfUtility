using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
	[CompilerGenerated]
	private List<int> g7wj1LeFQy;

	[CompilerGenerated]
	private PvfFile sqojyJ9lfB;

	[CompilerGenerated]
	private int JrhjHaMxrY;

	[CompilerGenerated]
	private string SlUjAqw7tV;

	[CompilerGenerated]
	private List<EquGroupItemAttributes> cChjavxp4F;

	[CompilerGenerated]
	private string? QAijKCsA6Q;

	[CompilerGenerated]
	private EquPartsetFile VbOjxfa3dX;

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

	public List<int> GroupItemCodeList
	{
		[CompilerGenerated]
		get
		{
			return g7wj1LeFQy;
		}
		[CompilerGenerated]
		set
		{
			g7wj1LeFQy = value;
		}
	}

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

	private PvfFile tEdjO29qtn
	{
		[CompilerGenerated]
		get
		{
			return sqojyJ9lfB;
		}
		[CompilerGenerated]
		set
		{
			sqojyJ9lfB = value;
		}
	}

	public string EquGroupName
	{
		[CompilerGenerated]
		get
		{
			return SlUjAqw7tV;
		}
		[CompilerGenerated]
		set
		{
			SlUjAqw7tV = value;
		}
	}

	public List<EquGroupItemAttributes> EquGroupItemInfos
	{
		[CompilerGenerated]
		get
		{
			return cChjavxp4F;
		}
		[CompilerGenerated]
		set
		{
			cChjavxp4F = value;
		}
	}

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
					if (tEdjO29qtn == null)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(9, 1);
						defaultInterpolatedStringHandler.AppendLiteral("找不到套装主文件：");
						defaultInterpolatedStringHandler.AppendFormatted(eCGj0wdnQG());
						return defaultInterpolatedStringHandler.ToStringAndClear();
					}
					scriptFileParserNew = new ScriptFileParserNew(tEdjO29qtn, base.Pvf);
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
					if (tEdjO29qtn == null)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(9, 1);
						defaultInterpolatedStringHandler.AppendLiteral("找不到套装主文件：");
						defaultInterpolatedStringHandler.AppendFormatted(eCGj0wdnQG());
						return defaultInterpolatedStringHandler.ToStringAndClear();
					}
					scriptFileParserNew = new ScriptFileParserNew(tEdjO29qtn, base.Pvf);
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

	public string? FullsetBasicExplain
	{
		[CompilerGenerated]
		get
		{
			return QAijKCsA6Q;
		}
		[CompilerGenerated]
		set
		{
			QAijKCsA6Q = value;
		}
	}

	public EquPartsetFile EquPartsetFile
	{
		[CompilerGenerated]
		get
		{
			return VbOjxfa3dX;
		}
		[CompilerGenerated]
		set
		{
			VbOjxfa3dX = value;
		}
	}

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
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(12, 1);
						defaultInterpolatedStringHandler.AppendLiteral("装备代码：");
						defaultInterpolatedStringHandler.AppendFormatted(item);
						defaultInterpolatedStringHandler.AppendLiteral("对应文件不存在");
						equPartsetFile.Name = defaultInterpolatedStringHandler.ToStringAndClear();
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
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(35, 1);
			defaultInterpolatedStringHandler2.AppendLiteral("套装索引：");
			defaultInterpolatedStringHandler2.AppendFormatted(val);
			defaultInterpolatedStringHandler2.AppendLiteral(" 未注册到：etc/equipmentpartset.etc");
			equPartsetFile2.Name = defaultInterpolatedStringHandler2.ToStringAndClear();
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
					cvaj3LBaj3(base.File.ItemCode.Value);
					tEdjO29qtn = base.File;
				}
			}
			else
			{
				EquGroupName = "套装主文件未设定 ：[set item]";
			}
		}
		else if (base.File.GetSectionIntValue("[set item master]", base.Pvf, out val2))
		{
			cvaj3LBaj3(val2);
			EquGroupType = EquGroupType.Children;
		}
		if (EquGroupType == EquGroupType.None)
		{
			return;
		}
		eCGj0wdnQG();
		if (tEdjO29qtn == null)
		{
			string text2 = base.Pvf.ListFileTable.ItemCodeConvertFilePath("equipment", eCGj0wdnQG());
			if (string.IsNullOrEmpty(text2))
			{
				EquGroupName = "套装主代码文件不存在：" + eCGj0wdnQG();
			}
			else
			{
				tEdjO29qtn = base.Pvf.GetFile(text2);
			}
		}
		string name;
		if (tEdjO29qtn == null)
		{
			EquGroupName = "套装主代码文件不存在：" + eCGj0wdnQG();
		}
		else if (tEdjO29qtn.GetNameText(base.Pvf, "[set name]", out name))
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
		Sgaj8IGgfE();
		if (tEdjO29qtn != null && tEdjO29qtn.GetNameText(base.Pvf, "[fullset basic explain]", out string name2))
		{
			FullsetBasicExplain = name2?.Replace("\\n", "\r\n");
		}
	}

	private void Sgaj8IGgfE()
	{
		if (tEdjO29qtn == null)
		{
			return;
		}
		if (EquGroupType == EquGroupType.Children)
		{
			if (!tEdjO29qtn.GetSectionIntArray(base.Pvf, "[set item]", out List<int> items) || items.Count == 0)
			{
				EquGroupName = "套装主文件未设定 [set item]";
				return;
			}
			GroupItemCodeList = items;
		}
		if (GroupItemCodeList != null && GroupItemCodeList.Count != 0)
		{
			EquGroupItemInfos = new List<EquGroupItemAttributes>();
			EquGroupItemInfos.Add(new EquGroupItemAttributes(tEdjO29qtn, isRoot: true, base.Pvf, eCGj0wdnQG()));
			for (int i = 1; i < GroupItemCodeList.Count; i++)
			{
				PvfFile file = base.Pvf.GetFile(base.Pvf.ListFileTable.ItemCodeConvertFilePath("equipment", GroupItemCodeList[i]));
				EquGroupItemInfos.Add(new EquGroupItemAttributes(file, isRoot: false, base.Pvf, GroupItemCodeList[i]));
			}
		}
	}

	[SpecialName]
	[CompilerGenerated]
	private int eCGj0wdnQG()
	{
		return JrhjHaMxrY;
	}

	[SpecialName]
	[CompilerGenerated]
	private void cvaj3LBaj3(int P_0)
	{
		JrhjHaMxrY = P_0;
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
				if (item.ItemCode == eCGj0wdnQG())
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
