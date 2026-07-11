using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass35_0
	{
		public string rs2qgk5NEj;

		public _003C_003Ec__DisplayClass35_0()
		{
		}

		internal bool lf0qaCUXxq(string x)
		{
			return Regex.IsMatch(x, Regex.Escape(rs2qgk5NEj), RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
		}
	}

	public Action CloseAction;

	[CompilerGenerated]
	private SearchConfig qWkWwOZkMl;

	[CompilerGenerated]
	private SearchConfig CRsWoBwRew;

	[CompilerGenerated]
	private List<SearchConfig>? YSaWsOuUUT;

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

	public SearchConfig Config
	{
		[CompilerGenerated]
		get
		{
			return qWkWwOZkMl;
		}
		[CompilerGenerated]
		set
		{
			qWkWwOZkMl = value;
		}
	}

	public SearchConfig MainWindowSearchConfig
	{
		[CompilerGenerated]
		get
		{
			return CRsWoBwRew;
		}
		[CompilerGenerated]
		set
		{
			CRsWoBwRew = value;
		}
	}

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

	private List<SearchConfig>? hXxW1b79X0
	{
		[CompilerGenerated]
		get
		{
			return YSaWsOuUUT;
		}
		[CompilerGenerated]
		set
		{
			YSaWsOuUUT = value;
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
		ThemeSwitcher.Instance.PvfCodeThemeChangedEvent += u2RWxWKrAb;
		OLpWQiIaI2();
	}

	private void u2RWxWKrAb()
	{
		OLpWQiIaI2();
	}

	private void OLpWQiIaI2()
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
		string text = AppCore.SelectPvfFolderPath();
		if (text != null)
		{
			Config.SearchFolder = text;
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
				PvfGroup pVF = AppCore.ViewModelBase.PVF;
				if (!string.IsNullOrEmpty(e.Text))
				{
					_003C_003Ec__DisplayClass35_0 CS_0024_003C_003E8__locals4 = new _003C_003Ec__DisplayClass35_0();
					CS_0024_003C_003E8__locals4.rs2qgk5NEj = ((AppSetting.Instance.PvfConfig.DefaultEncoding == EncodingType.TW) ? ChineseHelper.ToTraditional(e.Text) : e.Text);
					IEnumerable<string> itemsSource = null;
					switch (Config.Type)
					{
					case SearchType.Strings:
						if (AppSetting.Instance.PublicSearchServiceOptions.OpenStringAndSectionCompletion)
						{
							itemsSource = pVF.Strtable.SearchPanelGetKeywords(CS_0024_003C_003E8__locals4.rs2qgk5NEj);
						}
						break;
					case SearchType.FileName:
						if (AppSetting.Instance.PublicSearchServiceOptions.OpenFilePathCompletion)
						{
							itemsSource = AppCore.ViewModelBase.PVF.FileList.Keys.Where((string x) => Regex.IsMatch(x, Regex.Escape(CS_0024_003C_003E8__locals4.rs2qgk5NEj), RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace)).Take(AppSetting.Instance.PublicSearchServiceOptions.TakeNumber);
						}
						break;
					case SearchType.Name:
						if (AppSetting.Instance.PublicSearchServiceOptions.OpenNameCompletion)
						{
							itemsSource = pVF.Strtable.SearchPanelGetKeywords(CS_0024_003C_003E8__locals4.rs2qgk5NEj);
						}
						break;
					}
					autoSuggestEdit.ItemsSource = itemsSource;
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
				PooledList<string> pooledList = AppCore.ViewModelBase.SearchResultViewModel?.GetFileList(SelectedSourceItem);
				if (pooledList == null || pooledList.Count == 0)
				{
					AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("选中的搜索结果中没有可搜索的文件"), isError: true);
					return;
				}
				Config.SearchResult = pooledList.ToHashSet();
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
			ResultData<HashSet<string>> resultData = await Task.Run((Func<Task<ResultData<HashSet<string>>>?>)searchService.Search);
			if (resultData.IsError)
			{
				AppCore.ViewModelBase.SearchResultViewModel.TreeViewModel.TreeGroupData.Loading = false;
				AppCore.ShowMsg(resultData.Msg, isError: true);
				return;
			}
			if (Config.Type != SearchType.ScriptContent)
			{
				RlBWaCy0hJ(KeywordLog, Config.Keyword);
				if (!Config.IsUseLikeSearchPath)
				{
					RlBWaCy0hJ(FolderLog, Config.SearchFolder);
				}
			}
			AppCore.ViewModelBase.SearchResultViewModel?.AddSearchResult(resultData.Data.ToPooledList());
			AppCore.ViewModelBase.SearchResultViewModel.TreeViewModel.TreeGroupData.Loading = false;
			if (RecordingLoading)
			{
				hXxW1b79X0.Add(Config.CloneData());
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
		ResultData<HashSet<string>> resultData = await Task.Run((Func<Task<ResultData<HashSet<string>>>?>)new SearchService(MainWindowSearchConfig, AppCore.ViewModelBase.PVF).Search);
		if (resultData.IsError)
		{
			AppCore.ViewModelBase.SearchResultViewModel.TreeViewModel.TreeGroupData.Loading = false;
			AppCore.ShowMsg(resultData.Msg, isError: true);
			return;
		}
		if (MainWindowSearchConfig.Type != SearchType.ScriptContent)
		{
			RlBWaCy0hJ(KeywordLog, MainWindowSearchConfig.Keyword);
			if (!MainWindowSearchConfig.IsUseLikeSearchPath)
			{
				RlBWaCy0hJ(FolderLog, MainWindowSearchConfig.SearchFolder);
			}
		}
		AppCore.ViewModelBase.SearchResultViewModel?.AddSearchResult(resultData.Data.ToPooledList(), MainWindowSearchConfig.Keyword);
		AppCore.ViewModelBase.SearchResultViewModel.TreeViewModel.TreeGroupData.Loading = false;
	}

	public async Task MacroSearchTask(KeyValuePair<string, MacroData> row)
	{
		List<SearchConfig> list = row.Value.Data.ToString().JsonToObject<List<SearchConfig>>();
		HashSet<string> fileList = new HashSet<string>();
		foreach (SearchConfig config in list)
		{
			if (config.SourceType != SearchSourceType.AllFiles)
			{
				if (fileList.Count == 0)
				{
					break;
				}
				config.SearchResult = fileList;
			}
			ResultData<HashSet<string>> resultData = await Task.Run((Func<Task<ResultData<HashSet<string>>>?>)new SearchService(config, AppCore.ViewModelBase.PVF).Search);
			if (resultData.IsError)
			{
				AppCore.Logger.Error(AppSetting.Instance.GetIlogger()?.GetStr("mess_ExecuteSearchMacro"));
			}
			else if (config.SourceType == SearchSourceType.AllFiles)
			{
				fileList.AddRange(resultData.Data.ToArray());
			}
			else
			{
				fileList = resultData.Data;
			}
		}
		AppCore.ViewModelBase.SearchResultViewModel?.AddSearchResult(fileList.ToPooledList(), row.Key);
	}

	[Command]
	public async void SetRecordingLoading(bool isLoading)
	{
		if (isLoading)
		{
			hXxW1b79X0 = new List<SearchConfig>();
		}
		else if (RecordingLoading && hXxW1b79X0 != null && hXxW1b79X0.Count > 0)
		{
			MacroData macroData = new MacroData();
			macroData.SetData(hXxW1b79X0);
			macroData.MacroType = MacroType.全局搜索;
			await AppCore.SaveMacroData(macroData, AppSetting.Instance.GetIlogger()?.GetStr("mess_NewMacro"), Application.Current.MainWindow);
		}
		RecordingLoading = isLoading;
	}

	private void RlBWaCy0hJ(ObservableCollection<string> P_0, string P_1)
	{
		if (!string.IsNullOrEmpty(P_1))
		{
			if (P_0.Contains(P_1))
			{
				P_0.Remove(P_1);
			}
			P_0.Add(P_1);
			if (P_0.Count > 20)
			{
				P_0.RemoveAt(0);
			}
		}
	}

	public void Dispose()
	{
		ThemeSwitcher.Instance.PvfCodeThemeChangedEvent -= u2RWxWKrAb;
	}
}
