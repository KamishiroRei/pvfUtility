using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass35_0
	{
		public string BDZqqHaBUj;

		public WindowItemCodeSearchViewModel VDnqdMiW0f;

		public IEnumerable<string> VB3qeVMfMZ;

		public _003C_003Ec__DisplayClass35_0()
		{
		}

		internal IEnumerable<string> PVAqLQiimQ()
		{
			return AppCore.ViewModelBase.PVF.ListFileTable.ItemCodesToFilePathsAsync(BDZqqHaBUj, VDnqdMiW0f.SelectedItem.Value.Key);
		}

		internal void KK8qnhpBTP()
		{
			ConcurrentBag<ItemCodeSearchResult> concurrentBag = new ConcurrentBag<ItemCodeSearchResult>();
			foreach (string item in VB3qeVMfMZ)
			{
				concurrentBag.Add(new ItemCodeSearchResult(item));
			}
			VDnqdMiW0f.SearchResult.AddRange(concurrentBag.ToArray());
			VDnqdMiW0f.Count = ((VDnqdMiW0f.SearchResult != null) ? VDnqdMiW0f.SearchResult.Count : 0);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass38_0
	{
		public string H20qbu03iY;

		public _003C_003Ec__DisplayClass38_0()
		{
		}

		internal void EZcqtZKbg6(string it)
		{
			AppCore.ShowExtractLstWindow(H20qbu03iY);
		}
	}

	[CompilerGenerated]
	private SearchItemCodeConfig buhWqqSDe1;

	[CompilerGenerated]
	private TextDocument eRVWdrbN70;

	[CompilerGenerated]
	private IHighlightingDefinition bjDWe382L3;

	private ConcurrentObservableCollection<ItemCodeSearchResult> RN8WtIdonR;

	[CompilerGenerated]
	private List<ItemCodeSearchResult> y9HWbB1nt9;

	public bool RemoveDuplicate
	{
		get
		{
			return GetProperty(() => RemoveDuplicate);
		}
		set
		{
			SetProperty(() => RemoveDuplicate, value, aQcWLP0GAA);
		}
	}

	public SearchItemCodeConfig Config
	{
		[CompilerGenerated]
		get
		{
			return buhWqqSDe1;
		}
		[CompilerGenerated]
		set
		{
			buhWqqSDe1 = value;
		}
	}

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

	public TextDocument Document
	{
		[CompilerGenerated]
		get
		{
			return eRVWdrbN70;
		}
		[CompilerGenerated]
		set
		{
			eRVWdrbN70 = value;
		}
	}

	public IHighlightingDefinition Highlighting
	{
		[CompilerGenerated]
		get
		{
			return bjDWe382L3;
		}
		[CompilerGenerated]
		set
		{
			bjDWe382L3 = value;
		}
	}

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
			if (RN8WtIdonR == null)
			{
				RN8WtIdonR = new ConcurrentObservableCollection<ItemCodeSearchResult>();
			}
			return RN8WtIdonR;
		}
		set
		{
			RN8WtIdonR = value;
			RaisePropertyChanged("SearchResult");
		}
	}

	public List<ItemCodeSearchResult> SelectedItems
	{
		[CompilerGenerated]
		get
		{
			return y9HWbB1nt9;
		}
		[CompilerGenerated]
		set
		{
			y9HWbB1nt9 = value;
		}
	}

	private void aQcWLP0GAA()
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
		Document.TextChanged += kJMWnI8gvd;
	}

	private void kJMWnI8gvd(object? sender, EventArgs P_1)
	{
		Search();
	}

	[Command]
	public async void Search()
	{
		_003C_003Ec__DisplayClass35_0 CS_0024_003C_003E8__locals13 = new _003C_003Ec__DisplayClass35_0();
		CS_0024_003C_003E8__locals13.VDnqdMiW0f = this;
		if (!SelectedItem.HasValue)
		{
			return;
		}
		SearchResult = null;
		SearchResult = new ConcurrentObservableCollection<ItemCodeSearchResult>();
		IsLoading = true;
		CS_0024_003C_003E8__locals13.BDZqqHaBUj = Document.Text;
		CS_0024_003C_003E8__locals13.VB3qeVMfMZ = await Task.Run(() => AppCore.ViewModelBase.PVF.ListFileTable.ItemCodesToFilePathsAsync(CS_0024_003C_003E8__locals13.BDZqqHaBUj, CS_0024_003C_003E8__locals13.VDnqdMiW0f.SelectedItem.Value.Key));
		if (CS_0024_003C_003E8__locals13.VB3qeVMfMZ != null)
		{
			if (RemoveDuplicate)
			{
				CS_0024_003C_003E8__locals13.VB3qeVMfMZ = CS_0024_003C_003E8__locals13.VB3qeVMfMZ.ToHashSet();
			}
			await Task.Run(delegate
			{
				ConcurrentBag<ItemCodeSearchResult> concurrentBag = new ConcurrentBag<ItemCodeSearchResult>();
				foreach (string item in CS_0024_003C_003E8__locals13.VB3qeVMfMZ)
				{
					concurrentBag.Add(new ItemCodeSearchResult(item));
				}
				CS_0024_003C_003E8__locals13.VDnqdMiW0f.SearchResult.AddRange(concurrentBag.ToArray());
				CS_0024_003C_003E8__locals13.VDnqdMiW0f.Count = ((CS_0024_003C_003E8__locals13.VDnqdMiW0f.SearchResult != null) ? CS_0024_003C_003E8__locals13.VDnqdMiW0f.SearchResult.Count : 0);
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
		_003C_003Ec__DisplayClass38_0 CS_0024_003C_003E8__locals4 = new _003C_003Ec__DisplayClass38_0();
		if (SelectedItems == null)
		{
			return;
		}
		IEnumerable<string> files = SelectedItems.Select((ItemCodeSearchResult it) => it.FullPath);
		CS_0024_003C_003E8__locals4.H20qbu03iY = ServiceItemCodeTable.FilesToLstItemsToString(AppCore.ViewModelBase.PVF, files, out var count);
		if (string.IsNullOrEmpty(CS_0024_003C_003E8__locals4.H20qbu03iY))
		{
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_NoFileCanBeExtractedToLst"), isError: true);
			return;
		}
		AppCore.CopyString(CS_0024_003C_003E8__locals4.H20qbu03iY);
		string text = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_ExtractedLstCount"), count);
		AppCore.Logger.Success(text);
		await AppCore.Logger.ShowNotification(new NotificationViewModel<string>(AppSetting.Instance.AppName, text, Res.Instance.VisualStudioBlendLogo2015Pre_16x, AppSetting.Instance.GetIlogger()?.GetStr("mess_ViewDetails"), new DelegateCommand<string>(delegate
		{
			AppCore.ShowExtractLstWindow(CS_0024_003C_003E8__locals4.H20qbu03iY);
		})));
	}

	[Command]
	public void RowDoubleClick(RowClickArgs e)
	{
		if (e.Item != null && AppCore.ViewModelBase.PVF.PvfIsOpen)
		{
			ItemCodeSearchResult itemCodeSearchResult = (ItemCodeSearchResult)e.Item;
			if (itemCodeSearchResult != null)
			{
				AppCore.ViewModelBase.RootDocument.AddDocument(itemCodeSearchResult.FullPath, gotoNode: true);
			}
		}
	}
}
