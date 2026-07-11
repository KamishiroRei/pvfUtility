using System.Collections.Generic;
using PvfCode.ViewModels.SearchPvf.SearchName;

namespace PvfCode.ViewModels.independent_drop.DropList;

public class ListItem : ItemNameSearchResultBase
{
	private int wpT2xIvqmh;

	private int lPe2QuCLy2;

	public override int? ItemCode
	{
		get
		{
			return wpT2xIvqmh;
		}
		set
		{
			if (!value.HasValue)
			{
				value = -1;
			}
			wpT2xIvqmh = value.Value;
			RaisePropertyChanged("ItemCode");
			RaisePropertyChanged("ItemName");
		}
	}

	public override string ItemName
	{
		get
		{
			string text = AppCore.ViewModelBase.PVF.ListFileTable.ItemCodeConvertFilePath(new List<string>
			{
				"stackable",
				"equipment"
			}, ItemCode.Value);
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

	public int DropWeight
	{
		get
		{
			return lPe2QuCLy2;
		}
		set
		{
			lPe2QuCLy2 = value;
			RaisePropertiesChanged("DropWeight");
		}
	}

	public ListItem()
	{
	}
}
