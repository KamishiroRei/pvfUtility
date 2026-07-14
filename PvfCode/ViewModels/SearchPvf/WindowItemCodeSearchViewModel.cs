using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Collections.Pooled;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Xpf;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using PvfCode.LoggerBase;
using PvfCode.Models.Pvf;
using PvfCode.Models.Pvf.SearchItemCodeModels;
using PvfCode.Services;
using Swordfish.NET.Collections;

namespace PvfCode.ViewModels.SearchPvf;

public class WindowItemCodeSearchViewModel : ViewModelBase
{
	private ConcurrentObservableCollection<ItemCodeSearchResult> searchResult;

	public bool RemoveDuplicate
	{
		get
		{
			return GetProperty(() => RemoveDuplicate);
		}
		set
		{
			SetProperty(() => RemoveDuplicate, value, OnRemoveDuplicateChanged);
		}
	}

	public SearchItemCodeConfig Config { get; set; }

	public KeyValuePair<string, Dictionary<int, LstItem>>? SelectedItem
	{
		get
		{
			return GetProperty(() => SelectedItem);
		}
		set
		{
			SetProperty<KeyValuePair<string, Dictionary<int, LstItem>>?>(() => SelectedItem, value, Search);
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

	public TextDocument Document { get; set; }

	public IHighlightingDefinition Highlighting { get; set; }

	public int Count
	{
		get
		{
			return GetProperty(() => Count);
		}
		set
		{
			SetProperty(() => Count, value);
		}
	}

	public ConcurrentObservableCollection<ItemCodeSearchResult> SearchResult
	{
		get
		{
			if (searchResult == null)
			{
				searchResult = new ConcurrentObservableCollection<ItemCodeSearchResult>();
			}
			return searchResult;
		}
		set
		{
			searchResult = value;
			RaisePropertyChanged("SearchResult");
		}
	}

	public List<ItemCodeSearchResult> SelectedItems { get; set; }

	private void OnRemoveDuplicateChanged()
	{
		Search();
	}

	public WindowItemCodeSearchViewModel()
	{
		SelectedItems = new List<ItemCodeSearchResult>();
		Config = new SearchItemCodeConfig();
		Highlighting = ThemeSwitcher.Instance.GetHighlightingDefinition(PvfFileType.equ);
		Document = new TextDocument
		{
			Text = ""
		};
		Document.TextChanged += OnDocumentTextChanged;
	}

	private void OnDocumentTextChanged(object? sender, EventArgs e)
	{
		Search();
	}

	[Command]
	public async void Search()
	{
		if (!SelectedItem.HasValue)
		{
			return;
		}
		SearchResult = null;
		SearchResult = new ConcurrentObservableCollection<ItemCodeSearchResult>();
		IsLoading = true;
		string itemCodes = Document.Text;
		IEnumerable<string> filePaths = await Task.Run(() => AppCore.ViewModelBase.PVF.ListFileTable.ItemCodesToFilePathsAsync(itemCodes, SelectedItem.Value.Key));
		if (filePaths != null)
		{
			if (RemoveDuplicate)
			{
				filePaths = filePaths.ToHashSet();
			}
			await Task.Run(delegate
			{
				ConcurrentBag<ItemCodeSearchResult> results = new ConcurrentBag<ItemCodeSearchResult>();
				foreach (string filePath in filePaths)
				{
					results.Add(new ItemCodeSearchResult(filePath));
				}
				SearchResult.AddRange(results.ToArray());
				Count = ((SearchResult != null) ? SearchResult.Count : 0);
			});
		}
		IsLoading = false;
	}

	[Command]
	public void Clear()
	{
		SearchResult = null;
		SearchResult = new ConcurrentObservableCollection<ItemCodeSearchResult>();
		Count = ((SearchResult != null) ? SearchResult.Count : 0);
	}

	[Command]
	public void SelectedItemsAddFileListSearchResultPanel()
	{
		if (SelectedItems != null && SelectedItem.Value.Key != null)
		{
			IEnumerable<string> items = SelectedItems.Select((ItemCodeSearchResult it) => it.FullPath);
			if (AppCore.ViewModelBase.SearchResultViewModel.SearchReusltData.ContainsKey(SelectedItem.Value.Key))
			{
				AppCore.ViewModelBase.SearchResultViewModel.SearchReusltData.Remove(SelectedItem.Value.Key);
			}
			AppCore.ViewModelBase.SearchResultViewModel.AddSearchResult(items.ToPooledList(), SelectedItem.Value.Key);
		}
	}

	[Command]
	public async void ExtractSelectedItemsToLst()
	{
		if (SelectedItems == null)
		{
			return;
		}
		IEnumerable<string> files = SelectedItems.Select((ItemCodeSearchResult it) => it.FullPath);
		string lstText = ServiceItemCodeTable.FilesToLstItemsToString(AppCore.ViewModelBase.PVF, files, out var count);
		if (string.IsNullOrEmpty(lstText))
		{
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_NoFileCanBeExtractedToLst"), isError: true);
			return;
		}
		AppCore.CopyString(lstText);
		string message = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_ExtractedLstCount"), count);
		AppCore.Logger.Success(message);
		await AppCore.Logger.ShowNotification(new NotificationViewModel<string>(AppSetting.Instance.AppName, message, Res.Instance.VisualStudioBlendLogo2015Pre_16x, AppSetting.Instance.GetIlogger()?.GetStr("mess_ViewDetails"), new DelegateCommand<string>(delegate
		{
			AppCore.ShowExtractLstWindow(lstText);
		})));
	}

	[Command]
	public void RowDoubleClick(RowClickArgs e)
	{
		if (e.Item != null && AppCore.ViewModelBase.PVF.PvfIsOpen)
		{
			ItemCodeSearchResult item = (ItemCodeSearchResult)e.Item;
			if (item != null)
			{
				AppCore.ViewModelBase.RootDocument.AddDocument(item.FullPath, gotoNode: true);
			}
		}
	}
}
