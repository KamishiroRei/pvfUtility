using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Collections.Pooled;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Xpf.Editors;
using ICSharpCode.AvalonEdit.Highlighting;
using PvfCode.Dot;
using PvfCode.Dot.Desktop.Enums;
using PvfCode.Models.Macro;
using PvfCode.Models.Pvf.Enums;
using PvfCode.Services.SearchModel;
using Utools;

namespace PvfCode.ViewModels.SearchPvf;

public class ViewSearchPvfViewModel : ViewModelBase, IDisposable
{
	public Action CloseAction;

	private List<SearchConfig>? RecordedSearchConfigs { get; set; }

	public bool RecordingLoading
	{
		get
		{
			return GetProperty(() => RecordingLoading);
		}
		set
		{
			SetProperty(() => RecordingLoading, value);
		}
	}

	public ObservableCollection<string> KeywordLog
	{
		get
		{
			return GetProperty(() => KeywordLog);
		}
		set
		{
			SetProperty<ObservableCollection<string>>(() => KeywordLog, value);
		}
	}

	public ObservableCollection<string> FolderLog
	{
		get
		{
			return GetProperty(() => FolderLog);
		}
		set
		{
			SetProperty<ObservableCollection<string>>(() => FolderLog, value);
		}
	}

	public IHighlightingDefinition Highlighting
	{
		get
		{
			return GetProperty(() => Highlighting);
		}
		set
		{
			SetProperty<IHighlightingDefinition>(() => Highlighting, value);
		}
	}

	public SearchConfig Config { get; set; }

	public SearchConfig MainWindowSearchConfig { get; set; }

	public string SelectedSourceItem
	{
		get
		{
			return GetProperty(() => SelectedSourceItem);
		}
		set
		{
			SetProperty<string>(() => SelectedSourceItem, value);
		}
	}

	public bool ImmediatePopup
	{
		get
		{
			return GetProperty(() => ImmediatePopup);
		}
		set
		{
			SetProperty(() => ImmediatePopup, value);
		}
	}

	public ViewSearchPvfViewModel()
	{
		Config = new SearchConfig();
		FolderLog = new ObservableCollection<string>();
		KeywordLog = new ObservableCollection<string>();
		MainWindowSearchConfig = new SearchConfig
		{
			Type = SearchType.Strings
		};
		ThemeSwitcher.Instance.PvfCodeThemeChangedEvent += OnThemeChanged;
		UpdateHighlighting();
	}

	private void OnThemeChanged()
	{
		UpdateHighlighting();
	}

	private void UpdateHighlighting()
	{
		Highlighting = ThemeSwitcher.Instance.GetHighlightingDefinition(PvfFileType.equ);
	}

	public void Clear()
	{
		Config.ReSet();
		KeywordLog.Clear();
		FolderLog.Clear();
	}

	[Command]
	public void OnSelectedFolder()
	{
		string selectedFolderPath = AppCore.SelectPvfFolderPath();
		if (selectedFolderPath != null)
		{
			Config.SearchFolder = selectedFolderPath;
		}
	}

	[Command]
	public void OnCancel()
	{
		CloseAction();
	}

	[Command]
	public void ReSet()
	{
		Config.ReSet();
	}

	public void Closing(object sender, CancelEventArgs e)
	{
		if (RecordingLoading)
		{
			e.Cancel = true;
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseStopRecordMacro"), isError: true);
		}
	}

	public void textKeyword_QuerySubmitted(object sender, AutoSuggestEditQuerySubmittedEventArgs e)
	{
		lock (this)
		{
			try
			{
				AutoSuggestEdit autoSuggestEdit = (AutoSuggestEdit)sender;
				autoSuggestEdit.ItemsSource = null;
				PvfGroup pvf = AppCore.ViewModelBase.PVF;
				if (!string.IsNullOrEmpty(e.Text))
				{
					string keyword = ((AppSetting.Instance.PvfConfig.DefaultEncoding == EncodingType.TW) ? ChineseHelper.ToTraditional(e.Text) : e.Text);
					IEnumerable<string> suggestions = null;
					switch (Config.Type)
					{
					case SearchType.Strings:
						if (AppSetting.Instance.PublicSearchServiceOptions.OpenStringAndSectionCompletion)
						{
							suggestions = pvf.Strtable.SearchPanelGetKeywords(keyword);
						}
						break;
					case SearchType.FileName:
						if (AppSetting.Instance.PublicSearchServiceOptions.OpenFilePathCompletion)
						{
							suggestions = AppCore.ViewModelBase.PVF.FileList.Keys.Where((string filePath) => Regex.IsMatch(filePath, Regex.Escape(keyword), RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace)).Take(AppSetting.Instance.PublicSearchServiceOptions.TakeNumber);
						}
						break;
					case SearchType.Name:
						if (AppSetting.Instance.PublicSearchServiceOptions.OpenNameCompletion)
						{
							suggestions = pvf.Strtable.SearchPanelGetKeywords(keyword);
						}
						break;
					}
					autoSuggestEdit.ItemsSource = suggestions;
				}
				if (autoSuggestEdit.ItemsSource == null)
				{
					ImmediatePopup = false;
					autoSuggestEdit.ItemsSource = KeywordLog;
				}
				else
				{
					ImmediatePopup = true;
				}
			}
			catch (Exception ex)
			{
				AppCore.ShowMsg(ex.Message, isError: true);
			}
		}
	}

	[Command]
	public async void OnStart(bool isEnterKey)
	{
		try
		{
			if (isEnterKey && AppSetting.Instance.PublicSearchServiceOptions.DisableEnterShortcutsSearch)
			{
				return;
			}
			if (Config.SourceType != SearchSourceType.AllFiles)
			{
				if (SelectedSourceItem == null)
				{
					AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseSelectSearchResult"));
					return;
				}
				PooledList<string> sourceFiles = AppCore.ViewModelBase.SearchResultViewModel?.GetFileList(SelectedSourceItem);
				if (sourceFiles == null || sourceFiles.Count == 0)
				{
					AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("选中的搜索结果中没有可搜索的文件"), isError: true);
					return;
				}
				Config.SearchResult = sourceFiles.ToHashSet();
			}
			if (Config.Type == SearchType.ScriptContent)
			{
				if (Config.Trait)
				{
					if (string.IsNullOrEmpty(Config.ScriptContentStart))
					{
						AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseInputStartFeature"));
						return;
					}
					if (string.IsNullOrEmpty(Config.ScriptContentStop))
					{
						AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseInputEndFeature"));
						return;
					}
				}
				else if (string.IsNullOrEmpty(Config.ScriptContent))
				{
					AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseInputSearchKey"));
					return;
				}
			}
			if (Config.Type == SearchType.Num && Config.Keyword.Contains("."))
			{
				if (!float.TryParse(Config.Keyword, out var _))
				{
					AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseInputCorrectFloat"), isError: true);
					return;
				}
				if (!int.TryParse(Config.Keyword, out var _))
				{
					AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseInputCorrectInt"), isError: true);
					return;
				}
			}
			Config.IsUseLikeSearchPath = string.IsNullOrEmpty(Config.SearchFolder);
			SearchService searchService = new SearchService(Config, AppCore.ViewModelBase.PVF);
			if (!RecordingLoading)
			{
				CloseAction?.Invoke();
			}
			AppCore.ViewModelBase.SearchResultViewModel.TreeViewModel.TreeGroupData.Loading = true;
			ResultData<HashSet<string>> searchResult = await Task.Run((Func<Task<ResultData<HashSet<string>>>?>)searchService.Search);
			if (searchResult.IsError)
			{
				AppCore.ViewModelBase.SearchResultViewModel.TreeViewModel.TreeGroupData.Loading = false;
				AppCore.ShowMsg(searchResult.Msg, isError: true);
				return;
			}
			if (Config.Type != SearchType.ScriptContent)
			{
				AddSearchHistoryEntry(KeywordLog, Config.Keyword);
				if (!Config.IsUseLikeSearchPath)
				{
					AddSearchHistoryEntry(FolderLog, Config.SearchFolder);
				}
			}
			AppCore.ViewModelBase.SearchResultViewModel?.AddSearchResult(searchResult.Data.ToPooledList());
			AppCore.ViewModelBase.SearchResultViewModel.TreeViewModel.TreeGroupData.Loading = false;
			if (RecordingLoading)
			{
				RecordedSearchConfigs.Add(Config.CloneData());
			}
		}
		catch (Exception ex)
		{
			AppCore.Logger.Error(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_SearchError_OnStart"), ex.Message, ex.Source, ex.StackTrace));
		}
	}

	[Command]
	public async void OnMainWindowSearch()
	{
		AppCore.ViewModelBase.SearchResultViewModel.TreeViewModel.TreeGroupData.Loading = true;
		MainWindowSearchConfig.IsUseLikeSearchPath = string.IsNullOrEmpty(MainWindowSearchConfig.SearchFolder);
		ResultData<HashSet<string>> searchResult = await Task.Run((Func<Task<ResultData<HashSet<string>>>?>)new SearchService(MainWindowSearchConfig, AppCore.ViewModelBase.PVF).Search);
		if (searchResult.IsError)
		{
			AppCore.ViewModelBase.SearchResultViewModel.TreeViewModel.TreeGroupData.Loading = false;
			AppCore.ShowMsg(searchResult.Msg, isError: true);
			return;
		}
		if (MainWindowSearchConfig.Type != SearchType.ScriptContent)
		{
			AddSearchHistoryEntry(KeywordLog, MainWindowSearchConfig.Keyword);
			if (!MainWindowSearchConfig.IsUseLikeSearchPath)
			{
				AddSearchHistoryEntry(FolderLog, MainWindowSearchConfig.SearchFolder);
			}
		}
		AppCore.ViewModelBase.SearchResultViewModel?.AddSearchResult(searchResult.Data.ToPooledList(), MainWindowSearchConfig.Keyword);
		AppCore.ViewModelBase.SearchResultViewModel.TreeViewModel.TreeGroupData.Loading = false;
	}

	public async Task MacroSearchTask(KeyValuePair<string, MacroData> row)
	{
		List<SearchConfig> searchConfigs = row.Value.Data.ToString().JsonToObject<List<SearchConfig>>();
		HashSet<string> fileList = new HashSet<string>();
		foreach (SearchConfig config in searchConfigs)
		{
			if (config.SourceType != SearchSourceType.AllFiles)
			{
				if (fileList.Count == 0)
				{
					break;
				}
				config.SearchResult = fileList;
			}
			ResultData<HashSet<string>> searchResult = await Task.Run((Func<Task<ResultData<HashSet<string>>>?>)new SearchService(config, AppCore.ViewModelBase.PVF).Search);
			if (searchResult.IsError)
			{
				AppCore.Logger.Error(AppSetting.Instance.GetIlogger()?.GetStr("mess_ExecuteSearchMacro"));
			}
			else if (config.SourceType == SearchSourceType.AllFiles)
			{
				fileList.AddRange(searchResult.Data.ToArray());
			}
			else
			{
				fileList = searchResult.Data;
			}
		}
		AppCore.ViewModelBase.SearchResultViewModel?.AddSearchResult(fileList.ToPooledList(), row.Key);
	}

	[Command]
	public async void SetRecordingLoading(bool isLoading)
	{
		if (isLoading)
		{
			RecordedSearchConfigs = new List<SearchConfig>();
		}
		else if (RecordingLoading && RecordedSearchConfigs != null && RecordedSearchConfigs.Count > 0)
		{
			MacroData macroData = new MacroData();
			macroData.SetData(RecordedSearchConfigs);
			macroData.MacroType = MacroType.全局搜索;
			await AppCore.SaveMacroData(macroData, AppSetting.Instance.GetIlogger()?.GetStr("mess_NewMacro"), Application.Current.MainWindow);
		}
		RecordingLoading = isLoading;
	}

	private void AddSearchHistoryEntry(ObservableCollection<string> history, string entry)
	{
		if (!string.IsNullOrEmpty(entry))
		{
			if (history.Contains(entry))
			{
				history.Remove(entry);
			}
			history.Add(entry);
			if (history.Count > 20)
			{
				history.RemoveAt(0);
			}
		}
	}

	public void Dispose()
	{
		ThemeSwitcher.Instance.PvfCodeThemeChangedEvent -= OnThemeChanged;
	}
}
