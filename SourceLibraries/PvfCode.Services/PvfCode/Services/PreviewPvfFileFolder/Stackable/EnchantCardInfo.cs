using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
	[CompilerGenerated]
	private string Qtnjw3kMpk;

	[CompilerGenerated]
	private int xDrj6fjZDb;

	[CompilerGenerated]
	private List<string> forj22lOBW;

	[CompilerGenerated]
	private int fFDjBs3ShM;

	[CompilerGenerated]
	private MonsterCategoryType? SbKjU0twip;

	private readonly PvfFile eRUjvb1MLg;

	private readonly PvfGroup hZMjWVfBVu;

	private readonly ScriptFileParserNew pjXjiYfgjN;

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

	public string BackImgPath
	{
		[CompilerGenerated]
		get
		{
			return Qtnjw3kMpk;
		}
		[CompilerGenerated]
		set
		{
			Qtnjw3kMpk = value;
		}
	}

	public int BackImgIndex
	{
		[CompilerGenerated]
		get
		{
			return xDrj6fjZDb;
		}
		[CompilerGenerated]
		set
		{
			xDrj6fjZDb = value;
		}
	}

	public List<string> EquipParts
	{
		[CompilerGenerated]
		get
		{
			return forj22lOBW;
		}
		[CompilerGenerated]
		set
		{
			forj22lOBW = value;
		}
	}

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
			return GetProperty(() => MonsterLevelMini);
		}
		set
		{
			SetProperty(() => MonsterLevelMini, value);
		}
	}

	public int MonsterId
	{
		[CompilerGenerated]
		get
		{
			return fFDjBs3ShM;
		}
		[CompilerGenerated]
		set
		{
			fFDjBs3ShM = value;
		}
	}

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

	public MonsterCategoryType? MonsterType
	{
		[CompilerGenerated]
		get
		{
			return SbKjU0twip;
		}
		[CompilerGenerated]
		set
		{
			SbKjU0twip = value;
		}
	}

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
			if (pjXjiYfgjN.Sections != null)
			{
				SectionBase sectionBase = pjXjiYfgjN.Sections.Where((SectionBase it) => it.GetSectionName() == "[enchant]").FirstOrDefault();
				if (sectionBase == null)
				{
					return null;
				}
				string? equWhiteAttributes = PvfFilePreviewHelper.GetEquWhiteAttributes(pjXjiYfgjN, sectionBase.Children, hZMjWVfBVu);
				string text = PvfFilePreviewHelper.EquBlueAttributes(pjXjiYfgjN, sectionBase.Children, hZMjWVfBVu);
				if (eRUjvb1MLg.GetNameText(hZMjWVfBVu, "[stat desc]", out string name) && !string.IsNullOrEmpty(name))
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
		eRUjvb1MLg = file;
		hZMjWVfBVu = pvf;
		pjXjiYfgjN = scriptFileParserNew;
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
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(15, 2);
				defaultInterpolatedStringHandler.AppendLiteral("在NPK中找不到卡片背景图：");
				defaultInterpolatedStringHandler.AppendFormatted(BackImgPath);
				defaultInterpolatedStringHandler.AppendLiteral(",");
				defaultInterpolatedStringHandler.AppendFormatted(BackImgIndex);
				ilogger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			else
			{
				BackImageSource = image.Data;
				((Freezable)BackImageSource).Freeze();
			}
		}
		string text = hZMjWVfBVu.ListFileTable.ItemCodeConvertFilePath("monster", MonsterId);
		if (text == null || !hZMjWVfBVu.FileList.TryGetValue(text, out PvfFile value) || value == null)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(13, 1);
			defaultInterpolatedStringHandler2.AppendLiteral("找不到卡片怪物文件 ID：");
			defaultInterpolatedStringHandler2.AppendFormatted(MonsterId);
			ilogger.Error(defaultInterpolatedStringHandler2.ToStringAndClear());
			return;
		}
		if (value.GetMonsterType(hZMjWVfBVu, out var monsterCategoryType))
		{
			MonsterType = monsterCategoryType;
			RaisePropertyChanged("MonsterTypeImageSource");
		}
		if (value.GetSectionIntArray(hZMjWVfBVu, "[level]", out List<int> items) && items.Count == 2)
		{
			MonsterLevelMini = items[0];
			MonsterLevelMax = items[1];
			eRUjvb1MLg.GetRarity((PvfPack)hZMjWVfBVu, out RarityType? rarityType);
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
