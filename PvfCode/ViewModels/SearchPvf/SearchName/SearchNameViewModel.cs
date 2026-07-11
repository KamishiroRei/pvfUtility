using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
	[CompilerGenerated]
	private bool JfkWIfTIeQ;

	[CompilerGenerated]
	private bool PWvWE6mgop;

	[CompilerGenerated]
	private SearchNameViewModelType xrZWOBPifN;

	[CompilerGenerated]
	private string wx1WKr66kQ;

	[CompilerGenerated]
	private List<TItem> u1dW9mgVSI;

	public bool ShowGroupPanel
	{
		[CompilerGenerated]
		get
		{
			return JfkWIfTIeQ;
		}
		[CompilerGenerated]
		set
		{
			JfkWIfTIeQ = value;
		}
	}

	public bool ShowSelectYesOrCancelPanel
	{
		[CompilerGenerated]
		get
		{
			return PWvWE6mgop;
		}
		[CompilerGenerated]
		set
		{
			PWvWE6mgop = value;
		}
	}

	public SearchNameViewModelType Type
	{
		[CompilerGenerated]
		get
		{
			return xrZWOBPifN;
		}
		[CompilerGenerated]
		set
		{
			xrZWOBPifN = value;
		}
	}

	public string Title
	{
		[CompilerGenerated]
		get
		{
			return wx1WKr66kQ;
		}
		[CompilerGenerated]
		set
		{
			wx1WKr66kQ = value;
		}
	}

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

	public List<TItem> GroupSelectedItems
	{
		[CompilerGenerated]
		get
		{
			return u1dW9mgVSI;
		}
		[CompilerGenerated]
		set
		{
			u1dW9mgVSI = value;
		}
	}

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
			List<TItem> list = new List<TItem>();
			HashSet<string> hashSet = GroupItems.Select((TItem it) => it.FilePath).ToHashSet();
			foreach (TItem selectedItem in base.SelectedItems)
			{
				if (!hashSet.Contains(selectedItem.FilePath))
				{
					list.Add(selectedItem);
				}
			}
			GroupItems.AddRange(list);
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
			ItemNameSearchResultBase itemNameSearchResultBase = (ItemNameSearchResultBase)e.Item;
			if (itemNameSearchResultBase != null)
			{
				AppCore.ViewModelBase.RootDocument.AddDocument(itemNameSearchResultBase.FilePath, gotoNode: true);
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
		IEnumerable<string> enumerable = GroupItems.Select((TItem it) => it.FilePath);
		if (enumerable == null || !enumerable.Any())
		{
			return;
		}
		List<TItem> list = new List<TItem>();
		foreach (string item in enumerable.ToHashSet())
		{
			list.Add(new TItem
			{
				FilePath = item
			});
		}
		GroupItems.Clear();
		GroupItems.AddRange(list);
		UpdateGroupItemsCount();
	}

	[Command]
	public void OnRemoveItemCodeIsNull()
	{
		TItem[] array = GroupItems.ToArray();
		GroupItems.Clear();
		List<TItem> list = new List<TItem>();
		TItem[] array2 = array;
		foreach (TItem val in array2)
		{
			if (val.ItemCode.HasValue)
			{
				list.Add(val);
			}
		}
		GroupItems.AddRange(list);
		UpdateGroupItemsCount();
	}

	[Command]
	public void OnCopyGroupSelectedItemName()
	{
		if (GroupSelectedItems.Count > 0)
		{
			StringBuilder stringBuilder = new StringBuilder();
			TItem[] array = GroupSelectedItems.ToArray();
			foreach (TItem val in array)
			{
				stringBuilder.AppendLine(val.ItemName);
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
		TItem[] array = GroupSelectedItems.ToArray();
		foreach (TItem val in array)
		{
			if (val.ItemCode.HasValue)
			{
				StringBuilder stringBuilder2 = stringBuilder;
				StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(1, 1, stringBuilder2);
				handler.AppendFormatted(val.ItemCode);
				handler.AppendLiteral("\t");
				stringBuilder2.AppendLine(ref handler);
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
