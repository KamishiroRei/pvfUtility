using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Xpf;
using PvfCode.Dot;
using PvfCode.Services.SearchModel;
using Swordfish.NET.Collections;

namespace PvfCode.ViewModels.SearchPvf.SearchName;

public class SearchNameViewModelBase<TItem> : ViewModelBase where TItem : ItemNameSearchResultBase, new()
{
	public IEnumerable<string> SourceFileList;

	protected virtual IMessageBoxService MessageBoxService => GetService<IMessageBoxService>(ServiceSearchMode.PreferParents);

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

	public List<TItem> SelectedItems { get; set; }

	public TItem SelectedItem
	{
		get
		{
			return GetProperty(() => SelectedItem);
		}
		set
		{
			SetProperty<TItem>(() => SelectedItem, value);
		}
	}

	public TItem SelectedItem2
	{
		get
		{
			return GetProperty(() => SelectedItem2);
		}
		set
		{
			SetProperty<TItem>(() => SelectedItem2, value);
		}
	}

	public ConcurrentObservableCollection<TItem> Items
	{
		get
		{
			return GetProperty(() => Items);
		}
		set
		{
			SetProperty<ConcurrentObservableCollection<TItem>>(() => Items, value);
		}
	}

	public int Count => Items.Count;

	public string SearchKeyword
	{
		get
		{
			return GetProperty(() => SearchKeyword);
		}
		set
		{
			SetProperty<string>(() => SearchKeyword, value);
		}
	}

	public bool WholeWordMatch
	{
		get
		{
			return GetProperty(() => WholeWordMatch);
		}
		set
		{
			SetProperty(() => WholeWordMatch, value);
		}
	}

	public SearchNameViewModelBase()
	{
		SelectedItems = new List<TItem>();
		Items = new ConcurrentObservableCollection<TItem>();
	}

	[Command]
	public async void OnSearch()
	{
		await Task.Run((Func<Task?>)SearchStart);
		UpdateCount();
	}

	[Command]
	public void OnDeleteSelected()
	{
		if (SelectedItems != null && SelectedItems.Count > 0)
		{
			Items.RemoveRange(SelectedItems.ToArray());
			UpdateCount();
		}
	}

	[Command]
	public void RowDoubleClick(RowClickArgs e)
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
	public void OnCopySelectedItemName()
	{
		if (SelectedItems.Count > 0)
		{
			StringBuilder stringBuilder = new StringBuilder();
			TItem[] array = SelectedItems.ToArray();
			foreach (TItem val in array)
			{
				stringBuilder.AppendLine(val.ItemName);
			}
			AppCore.CopyString(stringBuilder.ToString());
		}
	}

	[Command]
	public void OnCopySelectedItemCode()
	{
		if (SelectedItems.Count <= 0)
		{
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		TItem[] array = SelectedItems.ToArray();
		foreach (TItem val in array)
		{
			if (val.ItemCode.HasValue)
			{
				stringBuilder.AppendLine($"{val.ItemCode}\t");
			}
		}
		AppCore.CopyString(stringBuilder.ToString());
	}

	public async Task SearchStart()
	{
		Items.Clear();
		if (string.IsNullOrEmpty(SearchKeyword))
		{
			return;
		}
		ResultData<HashSet<string>> resultData = await new SearchService(new SearchConfig
		{
			Keyword = SearchKeyword,
			NormalUsing = ((!WholeWordMatch) ? SearchNormalUsing.Like : SearchNormalUsing.None),
			WholeWordMatch = WholeWordMatch,
			Type = SearchType.Name,
			SearchResult = ((SourceFileList != null) ? SourceFileList.ToHashSet() : new HashSet<string>()),
			SourceType = ((SourceFileList != null) ? SearchSourceType.InSearchResultFind : SearchSourceType.AllFiles)
		}, AppCore.ViewModelBase.PVF).Search();
		if (resultData == null)
		{
			return;
		}
		List<TItem> list = new List<TItem>();
		foreach (string datum in resultData.Data)
		{
			list.Add(new TItem
			{
				FilePath = datum
			});
		}
		Items.AddRange(list);
	}

	public void InitLstName(IEnumerable<string> lstNames, bool initItems)
	{
		SourceFileList = AppCore.ViewModelBase.PVF.ListFileTable.GetLstFileList(lstNames);
		List<TItem> list = new List<TItem>();
		if (!initItems)
		{
			return;
		}
		foreach (string sourceFile in SourceFileList)
		{
			list.Add(new TItem
			{
				FilePath = sourceFile
			});
		}
		Items.AddRange(list);
	}

	public void UpdateCount()
	{
		RaisePropertyChanged("Count");
	}
}
