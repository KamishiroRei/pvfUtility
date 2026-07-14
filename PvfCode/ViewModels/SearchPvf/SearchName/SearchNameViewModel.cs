using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Collections.Pooled;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Xpf;
using PvfCodeViewModels.SearchPvf.SearchName;
using Swordfish.NET.Collections;

namespace PvfCode.ViewModels.SearchPvf.SearchName;

public class SearchNameViewModel<TItem> : SearchNameViewModelBase<TItem> where TItem : ItemNameSearchResultBase, new()
{
	public bool ShowGroupPanel { get; set; }

	public bool ShowSelectYesOrCancelPanel { get; set; }

	public SearchNameViewModelType Type { get; set; }

	public string Title { get; set; }

	public ConcurrentObservableCollection<TItem> GroupItems
	{
		get
		{
			return GetProperty(() => GroupItems);
		}
		set
		{
			SetProperty<ConcurrentObservableCollection<TItem>>(() => GroupItems, value);
		}
	}

	public TItem GroupSelectedItem
	{
		get
		{
			return GetProperty(() => GroupSelectedItem);
		}
		set
		{
			SetProperty<TItem>(() => GroupSelectedItem, value);
		}
	}

	public int GroupItemsCount => GroupItems.Count;

	public TItem GroupSelectedItem2
	{
		get
		{
			return GetProperty(() => GroupSelectedItem2);
		}
		set
		{
			SetProperty<TItem>(() => GroupSelectedItem2, value);
		}
	}

	public List<TItem> GroupSelectedItems { get; set; }

	public bool AddGroupCheckRepeat
	{
		get
		{
			return GetProperty(() => AddGroupCheckRepeat);
		}
		set
		{
			SetProperty(() => AddGroupCheckRepeat, value);
		}
	}

	public SearchNameViewModel()
	{
		ShowGroupPanel = true;
		GroupSelectedItems = new List<TItem>();
		GroupItems = new ConcurrentObservableCollection<TItem>();
		Title = AppSetting.Instance.GetIlogger().GetStr("mainWin_bar_Main_subItem_Search_NameSearcher");
		Type = SearchNameViewModelType.ItemNameSearchTool;
	}

	public SearchNameViewModel(string title, bool showGroupPanel, IEnumerable<string>? pathNames = null, bool initItems = false)
	{
		ShowGroupPanel = showGroupPanel;
		ShowSelectYesOrCancelPanel = true;
		Type = SearchNameViewModelType.ItemCodeSelectTool;
		Title = title;
		if (pathNames != null)
		{
			InitLstName(pathNames, initItems);
		}
	}

	[Command]
	public virtual void OnAddRightGroup()
	{
		if (base.SelectedItems.Count <= 0)
		{
			return;
		}
		if (AddGroupCheckRepeat)
		{
			List<TItem> itemsToAdd = new List<TItem>();
			HashSet<string> existingPaths = GroupItems.Select((TItem item) => item.FilePath).ToHashSet();
			foreach (TItem selectedItem in base.SelectedItems)
			{
				if (!existingPaths.Contains(selectedItem.FilePath))
				{
					itemsToAdd.Add(selectedItem);
				}
			}
			GroupItems.AddRange(itemsToAdd);
		}
		else
		{
			GroupItems.AddRange(base.SelectedItems);
		}
		UpdateGroupItemsCount();
	}

	[Command]
	public void OnGroupDeleteSelectedItems()
	{
		if (GroupSelectedItems.Count > 0)
		{
			GroupItems.RemoveRange(GroupSelectedItems.ToArray());
			UpdateGroupItemsCount();
		}
	}

	[Command]
	public void RowDoubleClickGroupItem(RowClickArgs e)
	{
		if (e.Item != null && AppCore.ViewModelBase.PVF.PvfIsOpen)
		{
			ItemNameSearchResultBase item = (ItemNameSearchResultBase)e.Item;
			if (item != null)
			{
				AppCore.ViewModelBase.RootDocument.AddDocument(item.FilePath, gotoNode: true);
			}
		}
	}

	[Command]
	public void SelectedItemsAddFileListSearchResultPanel()
	{
		if (GroupSelectedItems != null)
		{
			IEnumerable<string> items = GroupSelectedItems.Select((TItem it) => it.FilePath);
			string key = AppSetting.Instance.GetIlogger()?.GetStr("Title_NewNameSearch");
			if (AppCore.ViewModelBase.SearchResultViewModel.SearchReusltData.ContainsKey(key))
			{
				AppCore.ViewModelBase.SearchResultViewModel.SearchReusltData.Remove(key);
			}
			AppCore.ViewModelBase.SearchResultViewModel.AddSearchResult(items.ToPooledList(), key);
		}
	}

	[Command]
	public virtual void OnRemoveDuplicate()
	{
		IEnumerable<string> filePaths = GroupItems.Select((TItem item) => item.FilePath);
		if (filePaths == null || !filePaths.Any())
		{
			return;
		}
		List<TItem> uniqueItems = new List<TItem>();
		foreach (string filePath in filePaths.ToHashSet())
		{
			uniqueItems.Add(new TItem
			{
				FilePath = filePath
			});
		}
		GroupItems.Clear();
		GroupItems.AddRange(uniqueItems);
		UpdateGroupItemsCount();
	}

	[Command]
	public void OnRemoveItemCodeIsNull()
	{
		TItem[] items = GroupItems.ToArray();
		GroupItems.Clear();
		List<TItem> itemsWithCode = new List<TItem>();
		foreach (TItem item in items)
		{
			if (item.ItemCode.HasValue)
			{
				itemsWithCode.Add(item);
			}
		}
		GroupItems.AddRange(itemsWithCode);
		UpdateGroupItemsCount();
	}

	[Command]
	public void OnCopyGroupSelectedItemName()
	{
		if (GroupSelectedItems.Count > 0)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (TItem item in GroupSelectedItems.ToArray())
			{
				stringBuilder.AppendLine(item.ItemName);
			}
			AppCore.CopyString(stringBuilder.ToString());
		}
	}

	[Command]
	public void OnCopyGroupSelectedItemCode()
	{
		if (GroupSelectedItems.Count <= 0)
		{
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		foreach (TItem item in GroupSelectedItems.ToArray())
		{
			if (item.ItemCode.HasValue)
			{
				stringBuilder.AppendLine($"{item.ItemCode}\t");
			}
		}
		AppCore.CopyString(stringBuilder.ToString());
	}

	[Command]
	public virtual void OnYes(Window win)
	{
		if (ShowGroupPanel)
		{
			if (!GroupItems.Any() && !base.SelectedItems.Any())
			{
				AppCore.ShowMsg(MessageBoxService, AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseSelectCodeOrAddToContainer"));
				return;
			}
		}
		else if (base.SelectedItem == null)
		{
			AppCore.ShowMsg(MessageBoxService, AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseSelectCode"));
			return;
		}
		win.DialogResult = true;
	}

	[Command]
	public void OnCancel(Window win)
	{
		win.DialogResult = false;
	}

	public void UpdateGroupItemsCount()
	{
		RaisePropertyChanged("GroupItemsCount");
	}
}
