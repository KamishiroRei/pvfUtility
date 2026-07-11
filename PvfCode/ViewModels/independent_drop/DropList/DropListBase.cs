using System.Collections.Generic;
using DevExpress.Mvvm;
using PvfCode.Dot;
using PvfCode.Models.Options.Enums;
using Swordfish.NET.Collections;

namespace PvfCode.ViewModels.independent_drop.DropList;

public abstract class DropListBase : ViewModelBase
{
	public string SearchKeyWord
	{
		get
		{
			return GetProperty(() => SearchKeyWord);
		}
		set
		{
			SetProperty<string>(() => SearchKeyWord, value);
		}
	}

	public int Count
	{
		get
		{
			if (Items != null)
			{
				return Items.Count;
			}
			return 0;
		}
	}

	public ConcurrentObservableCollection<ListItem> Items
	{
		get
		{
			return GetProperty(() => Items);
		}
		set
		{
			SetProperty<ConcurrentObservableCollection<ListItem>>(() => Items, value);
		}
	}

	public abstract ResultData<string> ToText();

	public void UpdateCount()
	{
		RaisePropertyChanged("Count");
	}

	public ListItem Add()
	{
		ListItem listItem = new ListItem
		{
			DropWeight = 1000,
			ItemCode = -1
		};
		if (AppSetting.Instance.InsertIndependent_drop_ListOrder == InsertListOrder.首行插入)
		{
			Items.Insert(0, listItem);
		}
		else
		{
			Items.Add(listItem);
		}
		return listItem;
	}

	public void AddRange(IList<ListItem> items)
	{
		if (AppSetting.Instance.InsertIndependent_drop_ListOrder == InsertListOrder.首行插入)
		{
			Items.InsertRange(0, items);
		}
		else
		{
			Items.AddRange(items);
		}
	}

	public bool SearchItemCode(SearchConfig config)
	{
		if (Items == null)
		{
			return false;
		}
		foreach (ListItem item in Items)
		{
			if (config.KeywordCodes.Count > 1)
			{
				if (config.KeywordCodes.Contains(item.ItemCode.Value))
				{
					return true;
				}
			}
			else if (config.WholeWordMatch)
			{
				if (item.ItemCode.ToString() == config.SearchKeyword)
				{
					return true;
				}
			}
			else if (item.ItemCode.ToString().Contains(config.SearchKeyword))
			{
				return true;
			}
		}
		return false;
	}

	public bool SearchItemName(bool WholeWordMatch, string keyword)
	{
		if (Items == null)
		{
			return false;
		}
		foreach (ListItem item in Items)
		{
			if (WholeWordMatch)
			{
				if (item.ItemName == keyword)
				{
					return true;
				}
			}
			else if (item.ItemName.Contains(keyword))
			{
				return true;
			}
		}
		return false;
	}

	protected DropListBase()
	{
	}
}
