using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using PvfCode.Dot;
using PvfCode.LoggerBase;
using PvfCode.Models.Pvf;
using PvfCode.Models.Pvf.Enums;
using PvfCode.Services.PvfParsingNew;

namespace PvfCode.Services.PreviewPvfFileFolder.Stackable;

public class EnchantCardInfo : ViewModelBase
{
	private readonly PvfFile file;

	private readonly PvfGroup pvf;

	private readonly ScriptFileParserNew scriptFileParser;

	public ImageSource BackImageSource
	{
		get
		{
			return GetProperty(() => BackImageSource);
		}
		set
		{
			SetProperty<ImageSource>(() => BackImageSource, value);
		}
	}

	public EnchantCardInfoLevelIconInfo LevelImgLeft
	{
		get
		{
			return GetProperty(() => LevelImgLeft);
		}
		set
		{
			SetProperty<EnchantCardInfoLevelIconInfo>(() => LevelImgLeft, value);
		}
	}

	public EnchantCardInfoLevelIconInfo LevelImgRight
	{
		get
		{
			return GetProperty(() => LevelImgRight);
		}
		set
		{
			SetProperty<EnchantCardInfoLevelIconInfo>(() => LevelImgRight, value);
		}
	}

	public string BackImgPath { get; set; }

	public int BackImgIndex { get; set; }

	public List<string> EquipParts { get; set; }

	public string? EquipPartsText
	{
		get
		{
			if (EquipParts == null || EquipParts.Count == 0)
			{
				return null;
			}
			List<string> list = new List<string>();
			foreach (string equipPart in EquipParts)
			{
				if (PvfFileHelper.StrConvertEquipmentType(equipPart, out var re))
				{
					if (re.HasValue)
					{
						list.Add(re.ToString());
					}
					else
					{
						list.Add("未识别");
					}
				}
				else
				{
					list.Add("未识别");
				}
			}
			return string.Join("，", list);
		}
	}

	public int MonsterLevelMini
	{
		get
		{
			return GetProperty(() => MonsterLevelMini);
		}
		set
		{
			SetProperty(() => MonsterLevelMini, value);
		}
	}

	public int MonsterLevelMax
	{
		get
		{
			return GetProperty(() => MonsterLevelMax);
		}
		set
		{
			SetProperty(() => MonsterLevelMax, value);
		}
	}

	public int MonsterId { get; set; }

	public string EnchantPropertiesString
	{
		get
		{
			return GetProperty(() => EnchantPropertiesString);
		}
		set
		{
			SetProperty<string>(() => EnchantPropertiesString, value);
		}
	}

	public MonsterCategoryType? MonsterType { get; set; }

	public ImageSource? MonsterTypeImageSource
	{
		get
		{
			if (MonsterType.HasValue && ImagePack2Service.Instance.MonsterTypeIcons != null && ImagePack2Service.Instance.MonsterTypeIcons.TryGetValue(MonsterType.Value, out ImageSource value))
			{
				return value;
			}
			return null;
		}
	}

	public bool IsMonsterLevelMaxMoreThan9 => MonsterLevelMini > 9;

	public string? EquBlueAttributes
	{
		get
		{
			if (scriptFileParser.Sections != null)
			{
				SectionBase sectionBase = scriptFileParser.Sections.Where((SectionBase it) => it.GetSectionName() == "[enchant]").FirstOrDefault();
				if (sectionBase == null)
				{
					return null;
				}
				string? equWhiteAttributes = PvfFilePreviewHelper.GetEquWhiteAttributes(scriptFileParser, sectionBase.Children, pvf);
				string text = PvfFilePreviewHelper.EquBlueAttributes(scriptFileParser, sectionBase.Children, pvf);
				if (file.GetNameText(pvf, "[stat desc]", out string name) && !string.IsNullOrEmpty(name))
				{
					name = name.Replace("\\n", "");
				}
				return equWhiteAttributes + text + name;
			}
			return null;
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

	public EnchantCardInfo(PvfFile file, PvfGroup pvf, ScriptFileParserNew scriptFileParserNew)
	{
		this.file = file;
		this.pvf = pvf;
		scriptFileParser = scriptFileParserNew;
	}

	[Command]
	public async void OnLoaded()
	{
		IsLoading = true;
		await Task.Run((Action)Init);
		IsLoading = false;
	}

	public void Init()
	{
		Ilogger ilogger = AppSetting.Instance.GetIlogger();
		if (BackImgPath != null)
		{
			ResultData<ImageSource> image = ImagePack2Service.Instance.GetImage(BackImgPath.ToLower(), BackImgIndex);
			if (image.IsError || image.Data == null)
			{
				ilogger.Error($"在NPK中找不到卡片背景图：{BackImgPath},{BackImgIndex}");
			}
			else
			{
				BackImageSource = image.Data;
				((Freezable)BackImageSource).Freeze();
			}
		}
		string text = pvf.ListFileTable.ItemCodeConvertFilePath("monster", MonsterId);
		if (text == null || !pvf.FileList.TryGetValue(text, out PvfFile value) || value == null)
		{
			ilogger.Error($"找不到卡片怪物文件 ID：{MonsterId}");
			return;
		}
		if (value.GetMonsterType(pvf, out var monsterCategoryType))
		{
			MonsterType = monsterCategoryType;
			RaisePropertyChanged("MonsterTypeImageSource");
		}
		if (value.GetSectionIntArray(pvf, "[level]", out List<int> items) && items.Count == 2)
		{
			MonsterLevelMini = items[0];
			MonsterLevelMax = items[1];
			file.GetRarity((PvfPack)pvf, out RarityType? rarityType);
			if (!rarityType.HasValue)
			{
				rarityType = RarityType.普通;
			}
			KeyValuePair<EnchantCardInfoLevelIconInfo, EnchantCardInfoLevelIconInfo>? enchantCardInfoLevelIcon = ImagePack2Service.Instance.GetEnchantCardInfoLevelIcon(MonsterLevelMini, rarityType.Value);
			if (enchantCardInfoLevelIcon.HasValue)
			{
				LevelImgLeft = enchantCardInfoLevelIcon.Value.Key;
				LevelImgRight = enchantCardInfoLevelIcon.Value.Value;
			}
			RaisePropertyChanged("IsMonsterLevelMaxMoreThan9");
		}
	}
}
