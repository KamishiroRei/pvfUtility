using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Media;
using DevExpress.Mvvm;
using PvfCode.Dot;
using PvfCode.ViewModels.independent_drop.DropList;
using PvfCode.ViewModels.independent_drop.Enums;
using Swordfish.NET.Collections;
using Utools;

namespace PvfCode.ViewModels.independent_drop;

public class DropListRowData : ViewModelBase
{
	[CompilerGenerated]
	private List<string> Fjim8MFabO;

	private DropList_independentdrop QVamMPmvwt;

	private DropListList XoamVAnUg2;

	public List<string> Datas
	{
		[CompilerGenerated]
		get
		{
			return Fjim8MFabO;
		}
		[CompilerGenerated]
		set
		{
			Fjim8MFabO = value;
		}
	}

	public MonsterType MonsterType
	{
		get
		{
			int num = Datas[0].ToInt();
			if (num > 1)
			{
				num = 0;
			}
			return (MonsterType)num;
		}
		set
		{
			List<string> datas = Datas;
			int num = (int)value;
			datas[0] = num.ToString();
			RaisePropertyChanged("MonsterType");
			RaisePropertyChanged("MosterOrApcName");
		}
	}

	public DropType DropType
	{
		get
		{
			int num = Datas[16].ToInt();
			if (num > 2)
			{
				num = 0;
			}
			return (DropType)num;
		}
		set
		{
			List<string> datas = Datas;
			int num = (int)value;
			datas[16] = num.ToString();
			RaisePropertyChanged("DropType");
			RaisePropertyChanged("DropListBase");
		}
	}

	public int MosterOrApcId
	{
		get
		{
			return Datas[1].ToInt();
		}
		set
		{
			Datas[1] = value.ToString();
			RaisePropertyChanged("MosterOrApcId");
			RaisePropertyChanged("MosterOrApcName");
		}
	}

	public ImageSource MonsterImage
	{
		get
		{
			PvfGroup pVF = AppCore.ViewModelBase.PVF;
			string lstName = ((MonsterType == MonsterType.怪物) ? "monster" : "aicharacter");
			string text = pVF.ListFileTable.ItemCodeConvertFilePath(lstName, MosterOrApcId);
			if (text == null)
			{
				return null;
			}
			if (pVF.FileList.TryGetValue(text, out PvfFile value))
			{
				if (!ImagePack2Service.Instance.TreeGetIcon(pVF, value, out ImageSource imageSource))
				{
					return null;
				}
				return imageSource;
			}
			return null;
		}
	}

	public string MosterOrApcName
	{
		get
		{
			PvfGroup pVF = AppCore.ViewModelBase.PVF;
			string lstName = ((MonsterType == MonsterType.怪物) ? "monster" : "aicharacter");
			string text = pVF.ListFileTable.ItemCodeConvertFilePath(lstName, MosterOrApcId);
			if (text == null)
			{
				return string.Format(AppSetting.Instance.GetIlogger()?.GetStrNoReplace("mess_CannotFindCode"), MosterOrApcId);
			}
			string itemName = AppCore.ViewModelBase.PVF.GetItemName(text);
			if (!string.IsNullOrEmpty(itemName))
			{
				return itemName.Replace("\r\n", string.Empty);
			}
			return AppSetting.Instance.GetIlogger()?.GetStrNoReplace("CannotFindItemName");
		}
	}

	public DropList_independentdrop DropList_independentdrop
	{
		get
		{
			return QVamMPmvwt;
		}
		set
		{
			QVamMPmvwt = value;
			RaisePropertyChanged("DropList_independentdrop");
		}
	}

	public DropListList DropListList
	{
		get
		{
			if (XoamVAnUg2 == null)
			{
				XoamVAnUg2 = new DropListList(new ConcurrentObservableCollection<ListItem>());
			}
			return XoamVAnUg2;
		}
		set
		{
			XoamVAnUg2 = value;
			RaisePropertyChanged("DropListList");
		}
	}

	public DropListBase DropListBase
	{
		get
		{
			if (DropType == DropType.List)
			{
				return DropListList;
			}
			return DropList_independentdrop;
		}
	}

	public int ItemCode
	{
		get
		{
			return Datas[2].ToInt();
		}
		set
		{
			Datas[2] = ((DropType != DropType.Default) ? "0" : value.ToString());
			RaisePropertyChanged("ItemCode");
			RaisePropertyChanged("ItemName");
		}
	}

	public string ItemName
	{
		get
		{
			if (DropType != DropType.Default)
			{
				return string.Empty;
			}
			string text = AppCore.ViewModelBase.PVF.ListFileTable.ItemCodeConvertFilePath(new List<string>
			{
				"stackable",
				"equipment"
			}, ItemCode);
			if (text == null)
			{
				return string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_CannotFindCode"), ItemCode);
			}
			string itemName = AppCore.ViewModelBase.PVF.GetItemName(text);
			if (!string.IsNullOrEmpty(itemName))
			{
				return itemName.Replace("\r\n", string.Empty);
			}
			return AppSetting.Instance.GetIlogger()?.GetStr("CannotFindItemName");
		}
	}

	public float DropGrade1
	{
		get
		{
			float num = Datas[3].ToFloat();
			if (num > 1000000f)
			{
				num = 1000000f;
			}
			return num;
		}
		set
		{
			Datas[3] = value.ToString();
			RaisePropertyChanged("DropGrade1");
			RaisePropertyChanged("PercentageDropGrade1");
		}
	}

	public float DropGrade2
	{
		get
		{
			float num = Datas[4].ToFloat();
			if (num > 1000000f)
			{
				num = 1000000f;
			}
			return num;
		}
		set
		{
			Datas[4] = value.ToString();
			RaisePropertyChanged("DropGrade2");
			RaisePropertyChanged("PercentageDropGrade2");
		}
	}

	public float DropGrade3
	{
		get
		{
			float num = Datas[5].ToFloat();
			if (num > 1000000f)
			{
				num = 1000000f;
			}
			return num;
		}
		set
		{
			Datas[5] = value.ToString();
			RaisePropertyChanged("DropGrade3");
			RaisePropertyChanged("PercentageDropGrade3");
		}
	}

	public float DropGrade4
	{
		get
		{
			float num = Datas[6].ToFloat();
			if (num > 1000000f)
			{
				num = 1000000f;
			}
			return num;
		}
		set
		{
			Datas[6] = value.ToString();
			RaisePropertyChanged("DropGrade4");
			RaisePropertyChanged("PercentageDropGrade4");
		}
	}

	public float DropGrade5
	{
		get
		{
			float num = Datas[7].ToFloat();
			if (num > 1000000f)
			{
				num = 1000000f;
			}
			return num;
		}
		set
		{
			Datas[7] = value.ToString();
			RaisePropertyChanged("DropGrade5");
			RaisePropertyChanged("PercentageDropGrade5");
		}
	}

	public float PercentageDropGrade1
	{
		get
		{
			return DropGrade1 / 10000f;
		}
		set
		{
			if (value > 100f)
			{
				value = 100f;
			}
			DropGrade1 = value * 10000f;
		}
	}

	public float PercentageDropGrade2
	{
		get
		{
			return DropGrade2 / 10000f;
		}
		set
		{
			if (value > 100f)
			{
				value = 100f;
			}
			DropGrade2 = value * 10000f;
		}
	}

	public float PercentageDropGrade3
	{
		get
		{
			return DropGrade3 / 10000f;
		}
		set
		{
			if (value > 100f)
			{
				value = 100f;
			}
			DropGrade3 = value * 10000f;
		}
	}

	public float PercentageDropGrade4
	{
		get
		{
			return DropGrade4 / 10000f;
		}
		set
		{
			if (value > 100f)
			{
				value = 100f;
			}
			DropGrade4 = value * 10000f;
		}
	}

	public float PercentageDropGrade5
	{
		get
		{
			return DropGrade5 / 10000f;
		}
		set
		{
			if (value > 100f)
			{
				value = 100f;
			}
			DropGrade5 = value * 10000f;
		}
	}

	public int DropGrade1Number
	{
		get
		{
			return Datas[8].ToInt();
		}
		set
		{
			Datas[8] = value.ToString();
			RaisePropertyChanged("DropGrade1Number");
		}
	}

	public int DropGrade2Number
	{
		get
		{
			return Datas[9].ToInt();
		}
		set
		{
			Datas[9] = value.ToString();
			RaisePropertyChanged("DropGrade2Number");
		}
	}

	public int DropGrade3Number
	{
		get
		{
			return Datas[10].ToInt();
		}
		set
		{
			Datas[10] = value.ToString();
			RaisePropertyChanged("DropGrade3Number");
		}
	}

	public int DropGrade4Number
	{
		get
		{
			return Datas[11].ToInt();
		}
		set
		{
			Datas[11] = value.ToString();
			RaisePropertyChanged("DropGrade4Number");
		}
	}

	public int DropGrade5Number
	{
		get
		{
			return Datas[12].ToInt();
		}
		set
		{
			Datas[12] = value.ToString();
			RaisePropertyChanged("DropGrade5Number");
		}
	}

	public int LevelMin
	{
		get
		{
			return (int)Datas[13].ToFloat();
		}
		set
		{
			Datas[13] = value.ToString();
			RaisePropertyChanged("LevelMin");
		}
	}

	public int LevelMax
	{
		get
		{
			return (int)Datas[14].ToFloat();
		}
		set
		{
			Datas[14] = value.ToString();
			RaisePropertyChanged("LevelMax");
		}
	}

	public int CharacType
	{
		get
		{
			return Datas[15].ToInt();
		}
		set
		{
			Datas[15] = value.ToString();
			RaisePropertyChanged("CharacType");
			RaisePropertyChanged("CharacTypeName");
		}
	}

	public string CharacTypeName
	{
		get
		{
			if (CharacType == -1)
			{
				return AppSetting.Instance.GetIlogger()?.GetStr("mess_NoLimit");
			}
			PvfGroup pVF = AppCore.ViewModelBase.PVF;
			string lstName = "character";
			string text = pVF.ListFileTable.ItemCodeConvertFilePath(lstName, CharacType);
			if (text == null)
			{
				return string.Format(AppSetting.Instance.GetIlogger()?.GetStrNoReplace("mess_CannotFindCode"), CharacType);
			}
			string itemName = AppCore.ViewModelBase.PVF.GetItemName(text);
			if (!string.IsNullOrEmpty(itemName))
			{
				return itemName.Replace("\r\n", string.Empty);
			}
			return AppSetting.Instance.GetIlogger()?.GetStr("CannotFindItemName");
		}
	}

	public DropListRowData(List<string> datas, DropList_independentdrop dropList_Independentdrop)
	{
		DropList_independentdrop = dropList_Independentdrop;
		Datas = datas;
	}

	public string GetText()
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (DropType != DropType.Default)
		{
			ItemCode = 0;
			stringBuilder.Append(string.Join("\t", Datas));
			stringBuilder.Append("\r\n");
			stringBuilder.AppendLine("[list]");
			ResultData<string> resultData = DropListBase.ToText();
			stringBuilder.AppendLine(resultData.Data);
			stringBuilder.AppendLine("[/list]");
		}
		else
		{
			stringBuilder.Append(string.Join("\t", Datas));
		}
		return stringBuilder.ToString();
	}

	public bool Search(SearchConfig config)
	{
		if (config.WholeWordMatch)
		{
			switch (config.SearchType)
			{
			case SearchType.怪物ID:
				if (config.KeywordCodes.Count > 0)
				{
					return config.KeywordCodes.Contains(MosterOrApcId);
				}
				return MosterOrApcId.ToString() == config.SearchKeyword;
			case SearchType.怪物名称:
				return MosterOrApcName.Equals(config.SearchKeyword);
			case SearchType.物品ID:
				if (DropType == DropType.Default)
				{
					if (config.KeywordCodes.Count > 0)
					{
						return config.KeywordCodes.Contains(ItemCode);
					}
					return ItemCode.ToString() == config.SearchKeyword;
				}
				return DropListBase.SearchItemCode(config);
			case SearchType.物品名称:
				if (DropType == DropType.Default)
				{
					return ItemName.Equals(config.SearchKeyword);
				}
				return DropListBase.SearchItemName(config.WholeWordMatch, config.SearchKeyword);
			}
		}
		else
		{
			switch (config.SearchType)
			{
			case SearchType.怪物ID:
				if (config.KeywordCodes.Count > 0)
				{
					return config.KeywordCodes.Contains(MosterOrApcId);
				}
				return MosterOrApcId.ToString().Contains(config.SearchKeyword);
			case SearchType.怪物名称:
				return MosterOrApcName.Contains(config.SearchKeyword);
			case SearchType.物品ID:
				if (DropType == DropType.Default)
				{
					if (config.KeywordCodes.Count > 0)
					{
						return config.KeywordCodes.Contains(ItemCode);
					}
					return ItemCode.ToString().Contains(config.SearchKeyword);
				}
				return DropListBase.SearchItemCode(config);
			case SearchType.物品名称:
				if (DropType == DropType.Default)
				{
					return ItemName.Contains(config.SearchKeyword);
				}
				return DropListBase.SearchItemName(config.WholeWordMatch, config.SearchKeyword);
			}
		}
		return false;
	}
}
