using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Collections.Pooled;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using PvfCode.Services.SearchModel;
using PvfCode.ViewModels.SearchPvf;
using PvfCode.ViewModels.TreeFolder;
using PvfCode.Views.SearchPvf;
using Swordfish.NET.Collections;
using Utools;

namespace PvfCode.ViewModels;

public class SearchResultTreeViewModel : ViewModelBase
{
	[CompilerGenerated]
	private ViewSearchPvfViewModel gXWFusTP67;

	[CompilerGenerated]
	private PvfTreeViewModel u9IFGp73RG;

	public ViewSearchPvfViewModel SearchUiViewModel
	{
		[CompilerGenerated]
		get
		{
			return gXWFusTP67;
		}
		[CompilerGenerated]
		set
		{
			gXWFusTP67 = value;
		}
	}

	public ConcurrentObservableDictionary<string, SearchResultGroup> SearchReusltData
	{
		get
		{
			return GetProperty(() => SearchReusltData);
		}
		set
		{
			SetProperty<ConcurrentObservableDictionary<string, SearchResultGroup>>(() => SearchReusltData, value);
		}
	}

	public int ResultCount
	{
		get
		{
			return GetProperty(() => ResultCount);
		}
		set
		{
			SetProperty(() => ResultCount, value);
		}
	}

	public string SelectedItem
	{
		get
		{
			return GetProperty(() => SelectedItem);
		}
		set
		{
			SetProperty<string>(() => SelectedItem, value, s3nF4sc8cR);
		}
	}

	public PvfTreeViewModel TreeViewModel
	{
		[CompilerGenerated]
		get
		{
			return u9IFGp73RG;
		}
		[CompilerGenerated]
		set
		{
			u9IFGp73RG = value;
		}
	}

	public PooledList<string> GetSelectedFileList()
	{
		if (SelectedItem == null)
		{
			return new PooledList<string>();
		}
		return SearchReusltData[SelectedItem].FileList;
	}

	public PooledList<string> GetFileList(string key)
	{
		if (!SearchReusltData.ContainsKey(key))
		{
			return new PooledList<string>();
		}
		if (SearchReusltData.TryGetValue(key, out SearchResultGroup value))
		{
			return value.FileList;
		}
		return null;
	}

	public SearchResultTreeViewModel()
	{
		SearchUiViewModel = new ViewSearchPvfViewModel();
		SearchReusltData = new ConcurrentObservableDictionary<string, SearchResultGroup>();
		TreeViewModel = new PvfTreeViewModel(TreeViewType.SearchResult);
		SearchReusltData.Add("未命名", new SearchResultGroup());
		SelectedItem = SearchReusltData.Keys[0];
		TreeViewModel.RemoveSelectedItemsEvent += GOEFyvhfYL;
	}

	public void AddSearchResult(PooledList<string> fileList, string? key = null)
	{
		if (key == null)
		{
			key = SearchUiViewModel.Config.GetKey();
		}
		if (key == "未命名")
		{
			SearchReusltData[key].FileList = fileList;
		}
		else
		{
			RemoveSearchResult(key);
			SearchReusltData.Add(key, new SearchResultGroup
			{
				Config = SearchUiViewModel.Config,
				FileList = fileList
			});
		}
		if (SelectedItem == key)
		{
			s3nF4sc8cR();
			return;
		}
		SelectedItem = key;
		TreeViewModel.GoToFirstOrDefault();
	}

	public async Task AddSearchToCurrent(PooledList<string> fileList)
	{
		SearchReusltData[AppCore.ViewModelBase.SearchResultViewModel.SelectedItem].FileList.AddRange(fileList);
		await TreeViewModel.TreeGroupData.CreateTrees(new PooledList<string>(fileList));
	}

	[Command]
	public void OnShowAddNewSearchResultView()
	{
		WindowAddNewSearchResult windowAddNewSearchResult = new WindowAddNewSearchResult
		{
			Owner = Application.Current.MainWindow,
			WindowStartupLocation = WindowStartupLocation.CenterOwner
		};
		if (windowAddNewSearchResult.ShowDialog().Value)
		{
			string text = windowAddNewSearchResult.input.Text;
			AddSearchResult(new PooledList<string>(), text);
		}
	}

	private async void s3nF4sc8cR()
	{
		TreeViewModel.TreeGroupData.Trees = null;
		if (SelectedItem == null || !SearchReusltData.ContainsKey(SelectedItem))
		{
			return;
		}
		SearchResultGroup item = SearchReusltData[SelectedItem];
		TreeGroup treeGroupData = TreeViewModel.TreeGroupData;
		treeGroupData.Trees = await new TreeGroup(TreeViewType.SelectFolder).CreateTrees(new PooledList<string>(item.FileList));
		ResultCount = item.FileList.Count;
		if (TreeViewModel.TreeGroupData.FileCount < 5000)
		{
			((DispatcherObject)Application.Current).Dispatcher.BeginInvoke((Delegate)(Action)delegate
			{
				TreeViewModel.Service.ExpandAllNodes();
			}, Array.Empty<object>());
		}
		TreeViewModel.GoToFirstOrDefault();
	}

	public string GetCurrentTitle()
	{
		return SelectedItem;
	}

	[Command]
	public void RemoveSelectedSearchResult()
	{
		if (SelectedItem != null)
		{
			IList<string> keys = SearchReusltData.Keys;
			TreeViewModel.TreeGroupData.Trees = null;
			if (keys[0] == SelectedItem)
			{
				SearchReusltData[SelectedItem].Clear();
			}
			else
			{
				SearchReusltData.Remove(SelectedItem);
			}
			SelectedItem = SearchReusltData.Keys[0];
		}
	}

	public void RemoveSearchResult(string key)
	{
		if (string.IsNullOrEmpty(key) || SearchReusltData.ContainsKey(key))
		{
			SearchReusltData[key].Clear();
			SearchReusltData.Remove(key);
		}
	}

	[Command]
	public async void Clear(bool isUi = false)
	{
		SelectedItem = null;
		SearchReusltData.Clear();
		SearchUiViewModel.Clear();
		SearchReusltData = new ConcurrentObservableDictionary<string, SearchResultGroup>();
		TreeViewModel.Clear();
		TreeViewModel.TreeGroupData.Trees = new ObservableConcurrentDictionaryEx<string, PvfTreeFileBase>();
		SearchReusltData = null;
		SearchReusltData = new ConcurrentObservableDictionary<string, SearchResultGroup>();
		SearchReusltData.Add("未命名", new SearchResultGroup());
		SelectedItem = "未命名";
		if (isUi)
		{
			await Task.Run(delegate
			{
				WindowsEx.ClearMemorySilent(Process.GetCurrentProcess());
			});
			await Task.Run((Action)GC.Collect);
		}
	}

	[Command]
	public void ExtractAll()
	{
		if (SelectedItem != null)
		{
			PooledList<string> fileList = GetFileList(SelectedItem);
			AppCore.ViewModelBase.BarsVm.OnExtractFiles(fileList.ToList());
		}
	}

	public void TheSpecifiedFind(string key)
	{
		if (!string.IsNullOrEmpty(key))
		{
			SearchUiViewModel.SelectedSourceItem = key;
			SearchUiViewModel.Config.SourceType = SearchSourceType.InSearchResultFind;
		}
		else
		{
			SearchUiViewModel.Config.SourceType = SearchSourceType.AllFiles;
		}
	}

	[Command]
	public void OpenSearchUI()
	{
		AppCore.ViewModelBase.OnSearchPvf(SelectedItem);
	}

	[Command]
	public async void OnExportToTextFile()
	{
		try
		{
			if (string.IsNullOrEmpty(SelectedItem))
			{
				return;
			}
			PooledList<string> fileList = GetFileList(SelectedItem);
			if (fileList == null || fileList.Count == 0)
			{
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_NoFileCanBeExportToTxt"));
				return;
			}
			string filter = AppSetting.Instance.GetIlogger()?.GetStr("mess_TextName") + " (*.txt)|*.txt";
			SaveFileDialog saveFileDialog = new SaveFileDialog
			{
				Filter = filter,
				FileName = SelectedItem + ".txt"
			};
			bool? flag = saveFileDialog.ShowDialog();
			if (flag.HasValue && flag.Value)
			{
				await File.WriteAllTextAsync(saveFileDialog.FileName, fileList.ListToString("\r\n"));
			}
		}
		catch (Exception ex)
		{
			AppCore.Logger.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_ExportToTxtError"), ex.Message), isError: true, null, loggerError: true);
		}
	}

	[Command]
	public async void OnTxtImportFileList()
	{
		try
		{
			CommonOpenFileDialog dialog = new CommonOpenFileDialog
			{
				Title = AppSetting.Instance.GetIlogger().GetStr("mess_SelectFileToImport"),
				IsFolderPicker = false,
				Multiselect = false,
				AllowPropertyEditing = true,
				EnsurePathExists = true,
				EnsureValidNames = true
			};
			dialog.Multiselect = false;
			dialog.Filters.Add(new CommonFileDialogFilter(AppSetting.Instance.GetIlogger()?.GetStr("mess_TextName"), ".txt"));
			if (dialog.ShowDialog() != CommonFileDialogResult.Ok)
			{
				return;
			}
			string text = await File.ReadAllTextAsync(dialog.FileName);
			if (string.IsNullOrEmpty(text))
			{
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_TxtContentIsEmpty"));
				return;
			}
			PooledList<string> pooledList = rgRFY04C7a(text);
			if (pooledList != null && pooledList.Count != 0)
			{
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(dialog.FileName);
				AddSearchResult(pooledList, fileNameWithoutExtension);
			}
		}
		catch (Exception ex)
		{
			AppCore.Logger.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_ImportFromTxtError"), ex.Message), isError: true, null, loggerError: true);
		}
	}

	private PooledList<string> rgRFY04C7a(string P_0)
	{
		if (string.IsNullOrEmpty(P_0))
		{
			return null;
		}
		PooledSet<string> pooledSet = new PooledSet<string>();
		string[] array = P_0.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
		foreach (string item in array)
		{
			pooledSet.Add(item);
		}
		return new PooledList<string>(pooledSet);
	}

	private void GOEFyvhfYL(PooledSet<string> P_0)
	{
		if (string.IsNullOrEmpty(SelectedItem) || P_0 == null || P_0.Count <= 0 || !SearchReusltData.TryGetValue(SelectedItem, out SearchResultGroup value))
		{
			return;
		}
		foreach (string item in P_0)
		{
			value.FileList.Remove(item);
		}
	}

	~SearchResultTreeViewModel()
	{
		TreeViewModel.RemoveSelectedItemsEvent -= GOEFyvhfYL;
	}

	[CompilerGenerated]
	private void EfSFi2pWbw()
	{
		TreeViewModel.Service.ExpandAllNodes();
	}
}
