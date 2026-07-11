using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using Swordfish.NET.Collections;

namespace PvfCode.ViewModels.NpcShopEditor;

public class NpcShopPageViewModel : ViewModelBase
{
	[CompilerGenerated]
	private ConcurrentObservableCollection<NpcShopItem> tIamqvH1Z3;

	[CompilerGenerated]
	private ObservableCollection<NpcShopItem> nEHmdk0jrt;

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

	public ConcurrentObservableCollection<NpcShopItem> Items
	{
		[CompilerGenerated]
		get
		{
			return tIamqvH1Z3;
		}
		[CompilerGenerated]
		set
		{
			tIamqvH1Z3 = value;
		}
	}

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

	public ObservableCollection<NpcShopItem> SelectedItems
	{
		[CompilerGenerated]
		get
		{
			return nEHmdk0jrt;
		}
		[CompilerGenerated]
		set
		{
			nEHmdk0jrt = value;
		}
	}

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
		nEHmdk0jrt = new ObservableCollection<NpcShopItem>();
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
