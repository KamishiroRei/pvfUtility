using System.Collections.ObjectModel;
using System.Linq;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using Swordfish.NET.Collections;

namespace PvfCode.ViewModels.NpcShopEditor;

public class NpcShopPageViewModel : ViewModelBase
{
	public string Title
	{
		get
		{
			return GetProperty(() => Title);
		}
		set
		{
			SetProperty<string>(() => Title, value);
		}
	}

	public ConcurrentObservableCollection<NpcShopItem> Items { get; set; }

	public NpcShopItem CurrentItem
	{
		get
		{
			return GetProperty(() => CurrentItem);
		}
		set
		{
			SetProperty<NpcShopItem>(() => CurrentItem, value);
		}
	}

	public NpcShopItem SelectedItem
	{
		get
		{
			return GetProperty(() => SelectedItem);
		}
		set
		{
			SetProperty<NpcShopItem>(() => SelectedItem, value);
		}
	}

	public ObservableCollection<NpcShopItem> SelectedItems { get; set; }

	public bool IsSelected
	{
		get
		{
			return GetProperty(() => IsSelected);
		}
		set
		{
			SetProperty(() => IsSelected, value);
		}
	}

	public NpcShopPageViewModel(string title, ConcurrentObservableCollection<NpcShopItem> items)
	{
		SelectedItems = new ObservableCollection<NpcShopItem>();
		Title = title;
		Items = items;
		if (Items == null)
		{
			Items = new ConcurrentObservableCollection<NpcShopItem>();
		}
	}

	[Command]
	public void OnRemoveItems()
	{
		if (SelectedItems.Count <= 0)
		{
			return;
		}
		if (SelectedItems.Any(item => item.IsPurchaseDataModified))
		{
			AppCore.ShowMsg("选中商品包含尚未保存的购买属性，请先编译保存。", isError: true);
			return;
		}
		foreach (NpcShopItem selectedItem in SelectedItems)
		{
			selectedItem.ItemCode = -1;
		}
		SelectedItems.Clear();
	}

	[Command]
	public void OnDeleteItems()
	{
		if (SelectedItems.Count > 0)
		{
			if (SelectedItems.Any(item => item.IsPurchaseDataModified))
			{
				AppCore.ShowMsg("选中商品包含尚未保存的购买属性，请先编译保存。", isError: true);
				return;
			}
			Items.RemoveRange(SelectedItems);
			SelectedItems.Clear();
			CheckItems();
		}
	}

	[Command]
	public void OnAddPageLine()
	{
		int count = Items.Count;
		if (count % 7 != 0)
		{
			for (int i = 0; i < 7 - count % 7; i++)
			{
				Items.Add(new NpcShopItem());
			}
		}
		for (int j = 0; j < 7; j++)
		{
			Items.Add(new NpcShopItem());
		}
		SelectedItems.Clear();
		SelectedItems.Add(Items.Last());
		CurrentItem = Items.Last();
		SelectedItem = Items.Last();
	}

	public override string ToString()
	{
		ConcurrentObservableCollection<NpcShopItem> items = Items;
		if (items != null && !items.Any())
		{
			for (int i = 0; i < 7; i++)
			{
				Items.Add(new NpcShopItem());
			}
		}
		int count = Items.Count;
		if (count % 7 != 0)
		{
			for (int j = 0; j < 7 - count % 7; j++)
			{
				Items.Add(new NpcShopItem());
			}
		}
		return string.Join("\t", Items.Select((NpcShopItem it) => it.ItemCode));
	}

	public void CheckItems()
	{
		int count = Items.Count;
		if (count % 7 != 0)
		{
			for (int i = 0; i < 7 - count % 7; i++)
			{
				Items.Add(new NpcShopItem());
			}
		}
	}
}
