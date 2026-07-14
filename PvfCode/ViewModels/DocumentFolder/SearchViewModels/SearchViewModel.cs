using System;
using System.Linq;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Xpf.Editors;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;
using PvfCode.Controls.TextEditorFolder;
using PvfCode.Dot;
using PvfCode.Models.Pvf.Enums;
using PvfCode.ViewModels.DocumentFolder.BackgroundRenderers;
using PvfCode.ViewModels.DocumentFolder.SearchViewModels.Enums;
using PvfCode.ViewModels.DocumentFolder.SearchViewModels.Searchs;
using Utools;

namespace PvfCode.ViewModels.DocumentFolder.SearchViewModels;

public class SearchViewModel : ViewModelBase, IDisposable
{
	private ISearchStrategy searchStrategy;

	private readonly string FilePath;

	private bool visibility;

	private bool showReplacePanel;

	private TextEditorBase Editor { get; set; }

	private SearchResultBackgroundRenderer SearchRenderer { get; set; }

	private bool IsReplacing { get; set; }

	private bool IsUpdatingConfig { get; set; }

	private TextArea textArea => Editor.TextArea;

	public SearchConfig Config
	{
		get
		{
			return GetProperty(() => Config);
		}
		set
		{
			SetProperty<SearchConfig>(() => Config, value, OnConfigChanged);
		}
	}

	public bool ExistFind
	{
		get
		{
			if (Config.SourceType != SourceType.所有打开的文档)
			{
			if (SearchRenderer != null && SearchRenderer.Segments != null && SearchRenderer.Segments.Count != 0)
				{
					return true;
				}
				return false;
			}
			return true;
		}
	}

	public Action SetFindKeywordFocused { get; set; }

	public bool Visibility
	{
		get
		{
			return visibility;
		}
		set
		{
			visibility = value;
			RaisePropertyChanged("Visibility");
			if (!value)
			{
				SearchRenderer.Segments.Clear();
			}
			EventSearchPanelVisibilityChanged?.Invoke(value);
		}
	}

	public bool ShowReplacePanel
	{
		get
		{
			return showReplacePanel;
		}
		set
		{
			showReplacePanel = value;
			RaisePropertyChanged("ShowReplacePanel");
		}
	}

	public int SearchResultCount
	{
		get
		{
			RaisePropertyChanged("ExistFind");
			if (SearchRenderer == null || SearchRenderer.Segments == null || SearchRenderer.Segments.Count == 0)
			{
				return 0;
			}
			return SearchRenderer.Segments.Count;
		}
	}

	public bool IsPopupOpen
	{
		get
		{
			return GetProperty(() => IsPopupOpen);
		}
		set
		{
			SetProperty(() => IsPopupOpen, value);
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

	public event SearchPanelVisibilityChanged EventSearchPanelVisibilityChanged;

	private TextDocument Document => Editor.Document;

	public bool InTheWork()
	{
		return IsReplacing;
	}

	public SearchViewModel(TextEditorBase editor, string filePath)
	{
		FilePath = filePath;
		Editor = editor;
		Config = new SearchConfig();
		SearchRenderer = new SearchResultBackgroundRenderer();
		if (Document != null)
		{
			Document.TextChanged += OnDocumentTextChanged;
		}
		textArea.DocumentChanged += OnTextAreaDocumentChanged;
		Config.EventDelegateConfigChanged += OnConfigChanged;
		editor.TextArea.TextView.BackgroundRenderers.Add(SearchRenderer);
	}

	private void OnConfigChanged()
	{
		RefreshSearchResults();
	}

	[Command]
	public void OnShowSearchPanel(bool visibility)
	{
		try
		{
			Visibility = visibility;
			if (Visibility)
			{
				string selectedText = Editor.SelectedText;
				if (selectedText != null && selectedText.Length < 2000)
				{
					Config.FindKeyword = selectedText;
				}
				RefreshSearchResults();
				SetFindKeywordFocused?.Invoke();
			}
			else
			{
				SearchRenderer.Clear();
				textArea.TextView.InvalidateLayer(KnownLayer.Background);
				AppCore.ViewModelBase.RootDocument.DocumentsSearchManager.Clear();
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "SearchViewModel.OnShowSearchPanel");
		}
	}

	[Command]
	public void OnShowReplacePanel()
	{
		try
		{
			Visibility = true;
			ShowReplacePanel = true;
			string selectedText = Editor.SelectedText;
			if (selectedText != null && selectedText.Length < 2000)
			{
				Config.ReplaceKeyword = selectedText;
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "SearchViewModel.OnShowSearchPanel");
		}
	}

	public void ShowSearchPanel(SearchConfig config, bool showReplacePanel)
	{
		try
		{
			IsUpdatingConfig = true;
			Visibility = true;
			Config.EventDelegateConfigChanged -= OnConfigChanged;
			Config = config.ToJson().JsonToObject<SearchConfig>();
			Config.EventDelegateConfigChanged += OnConfigChanged;
			if (showReplacePanel)
			{
				ShowReplacePanel = showReplacePanel;
			}
			else
			{
				SetFindKeywordFocused?.Invoke();
			}
			IsUpdatingConfig = false;
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "SearchViewModel.ShowSearchPanel");
		}
	}

	[Command]
	public void OnAppedLine(bool isReplace)
	{
		if (isReplace)
		{
			Config.ReplaceKeyword += "\r\n";
		}
		else
		{
			Config.FindKeyword += "\r\n";
		}
	}

	[Command]
	public void OnFindMain()
	{
		try
		{
			AddSearchKeywordLog();
			switch (Config.SearchType)
			{
			case SearchType.Next:
				FindNext();
				break;
			case SearchType.Previous:
				FindPrevious();
				break;
			case SearchType.All:
				if (Config.SourceType == SourceType.所有打开的文档)
				{
					AppCore.ViewModelBase.RootDocument.DocumentsSearchManager.FindAll();
				}
				else if (SearchRenderer.Segments.Any())
				{
					AppCore.ViewModelBase.RootDocument.DocumentsSearchManager.AllSearchResultViewModel.AddResult(FilePath, SearchRenderer.Segments);
				}
				else
				{
					AppCore.ViewModelBase.RootDocument.DocumentsSearchManager.AllSearchResultViewModel.Tree = null;
				}
				break;
			}
			SearchType searchType = Config.SearchType;
			if ((uint)searchType <= 1u && Config.SourceType != SourceType.所有打开的文档 && !SearchRenderer.Segments.Any() && AppSetting.Instance.EditConfig.SearchPanelFindNotFoundAllowMessageBox)
			{
				AppCore.Logger.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_CannotFindSearchResult"), Config.FindKeyword));
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "SearchViewModel.OnFindMain");
		}
	}

	private void AddSearchKeywordLog()
	{
		try
		{
			if (AppCore.EditorSearchKeywordLog.Contains(Config.FindKeyword))
			{
				AppCore.EditorSearchKeywordLog.Remove(Config.FindKeyword);
			}
			if (AppCore.EditorSearchKeywordLog.Count >= 20)
			{
				AppCore.EditorSearchKeywordLog.RemoveAt(AppCore.EditorSearchKeywordLog.Count - 1);
			}
			AppCore.EditorSearchKeywordLog.Insert(0, Config.FindKeyword);
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "SearchViewModel.AddSearchKeywordLog");
		}
	}

	private void AddReplaceKeywordLog()
	{
		try
		{
			AddSearchKeywordLog();
			if (AppCore.EditorReplaceKeywordLog.Contains(Config.ReplaceKeyword))
			{
				AppCore.EditorReplaceKeywordLog.Remove(Config.ReplaceKeyword);
			}
			if (AppCore.EditorReplaceKeywordLog.Count >= 20)
			{
				AppCore.EditorReplaceKeywordLog.RemoveAt(AppCore.EditorReplaceKeywordLog.Count - 1);
			}
			AppCore.EditorReplaceKeywordLog.Insert(0, Config.ReplaceKeyword);
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "SearchViewModel.AddSearchKeywordLog");
		}
	}

	[Command]
	public void OnFind(SearchType searchType)
	{
		Config.SearchType = searchType;
		OnFindMain();
	}

	private void FindNext()
	{
		try
		{
			if (IsFindKeywordEmpty())
			{
				return;
			}
			switch (Config.SourceType)
			{
			case SourceType.当前文档:
			{
				SearchResult searchResult = SearchRenderer.Segments.FindFirstSegmentWithStartAfter(textArea.Caret.Offset + 1);
				if (searchResult == null)
				{
					searchResult = SearchRenderer.Segments.FirstSegment;
				}
				if (searchResult != null)
				{
					SelectResult(searchResult);
				}
				break;
			}
			case SourceType.所有打开的文档:
				AppCore.ViewModelBase.RootDocument.DocumentsSearchManager.FindNext(FilePath, replace: false);
				break;
			case SourceType.当前块:
				break;
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "SearchViewModel.FindNext");
		}
	}

	private void FindPrevious()
	{
		try
		{
			switch (Config.SourceType)
			{
			case SourceType.当前文档:
				if (!IsFindKeywordEmpty())
				{
					SearchResult searchResult = SearchRenderer.Segments.FindFirstSegmentWithStartAfter(textArea.Caret.Offset);
					if (searchResult != null)
					{
						searchResult = SearchRenderer.Segments.GetPreviousSegment(searchResult);
					}
					if (searchResult == null)
					{
						searchResult = SearchRenderer.Segments.LastSegment;
					}
					if (searchResult != null)
					{
						SelectResult(searchResult);
					}
				}
				break;
			case SourceType.所有打开的文档:
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_GlobalSearchUpNotSupport"));
				break;
			case SourceType.当前块:
				break;
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "SearchViewModel.FindPrevious");
		}
	}

	private bool IsFindKeywordEmpty()
	{
		return string.IsNullOrEmpty(Config.FindKeyword);
	}

	[Command]
	public void OnReplace(bool isReplaceAll)
	{
		try
		{
			AddReplaceKeywordLog();
			if (Config.SourceType != SourceType.所有打开的文档 && !SearchRenderer.Segments.Any())
			{
				AppCore.Logger.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_CannotFindSearchResult"), Config.FindKeyword));
				return;
			}
			if (isReplaceAll)
			{
				ReplaceAll();
				return;
			}
			if (Config.SourceType == SourceType.所有打开的文档)
			{
				AppCore.ViewModelBase.RootDocument.DocumentsSearchManager.FindNext(FilePath, replace: true);
				return;
			}
			SearchResult searchResult = SearchRenderer.Segments.FindFirstSegmentWithStartAfter(textArea.Caret.Offset);
			if (searchResult == null)
			{
				searchResult = SearchRenderer.Segments.FirstSegment;
			}
			if (searchResult != null)
			{
				string text = searchStrategy.ReplaceNext(Document, searchResult.StartOffset, searchResult.Length, Config.FindKeyword, Config.ReplaceKeyword, Config.RegularExpression);
				if (!string.IsNullOrEmpty(text))
				{
					searchResult.Length = text.Length;
					SelectResult(searchResult);
				}
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "SearchViewModel.OnReplace");
		}
	}

	private void ReplaceAll()
	{
		try
		{
			if (Config.SourceType == SourceType.当前文档)
			{
				int searchResultCount = SearchResultCount;
				textArea.TextView.InvalidateLayer(KnownLayer.Selection);
				IsReplacing = true;
				Editor.BeginChange();
				TextDocument document = Editor.Document;
				searchStrategy.ReplaceAll(document, Config);
				Editor.EndChange();
				IsReplacing = false;
				RefreshSearchResults();
				AppCore.Logger.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_ReplaceSuccessCount"), searchResultCount));
			}
			else if (Config.SourceType == SourceType.所有打开的文档)
			{
				AppCore.ViewModelBase.RootDocument.DocumentsSearchManager.ReplaceAll();
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "SearchViewModel.ReplaceAll");
		}
	}

	private void OnDocumentTextChanged(object? sender, EventArgs e)
	{
		if (!InTheWork())
		{
			RefreshSearchResults();
		}
	}

	private void OnTextAreaDocumentChanged(object? sender, EventArgs e)
	{
		if (Document != null)
		{
			Document.TextChanged -= OnDocumentTextChanged;
		}
		if (Document != null)
		{
			Document.TextChanged += OnDocumentTextChanged;
			if (!InTheWork())
			{
				DoSearch(false);
			}
		}
	}

	private void RefreshSearchResults()
	{
		try
		{
			if (Config.SourceType == SourceType.所有打开的文档 && !IsUpdatingConfig)
			{
				AppCore.ViewModelBase.RootDocument.DocumentsSearchManager.RefSetData(Config, FilePath);
			}
			if (!IsReplacing)
			{
				textArea.TextView.InvalidateLayer(KnownLayer.Background);
				if (SearchRenderer != null)
				{
					SearchRenderer.Clear();
				}
				RaisePropertyChanged("SearchResultCount");
				if (!IsFindKeywordEmpty())
				{
					CreateSearchStrategy();
				}
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "SearchViewModel.ValidateSearchText");
		}
	}

	private async void CreateSearchStrategy()
	{
		SearchRenderer.Segments.Any();
		try
		{
			ResultData<ISearchStrategy> resultData = await SearchStrategyFactory.Create(Config);
			if (resultData.IsError)
			{
				AppCore.ShowMsg(resultData.Msg);
				return;
			}
			searchStrategy = resultData.Data;
		}
		catch (Exception ex)
		{
			AppCore.Logger.Error(ex.Message);
			IsPopupOpen = true;
			searchStrategy = null;
			return;
		}
		DoSearch(true);
	}

	private void DoSearch(bool selectNextResult)
	{
		try
		{
			if (!Visibility)
			{
				RaisePropertyChanged("ExistFind");
				return;
			}
			SearchRenderer.Segments.Clear();
			if (!string.IsNullOrEmpty(Config.FindKeyword))
			{
				int offset = textArea.Caret.Offset;
				if (selectNextResult)
				{
					textArea.ClearSelection();
				}
				foreach (SearchResult item in searchStrategy.FindAll(textArea.Document, 0, textArea.Document.TextLength))
				{
					if (selectNextResult && item.StartOffset >= offset)
					{
						_ = Editor.CodeCompletionIsOpen;
						selectNextResult = false;
					}
					SearchRenderer.Segments.Add(item);
				}
				SearchRenderer.Segments.Any();
			}
			textArea.TextView.InvalidateLayer(KnownLayer.Selection);
			RaisePropertyChanged("SearchResultCount");
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "SearchViewModel.DoSearch");
		}
	}

	public bool SelectResult(ISearchResult result)
	{
		try
		{
			textArea.Caret.Offset = result.Offset;
			textArea.Selection = Selection.Create(textArea, result.Offset, result.EndOffset);
			textArea.Caret.BringCaretToView();
			textArea.Caret.Show();
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public void FindKeywordText_QuerySubmitted(object sender, AutoSuggestEditQuerySubmittedEventArgs e)
	{
		lock (this)
		{
			AutoSuggestEdit autoSuggestEdit = (AutoSuggestEdit)sender;
			autoSuggestEdit.ItemsSource = null;
			PvfGroup pVF = AppCore.ViewModelBase.PVF;
			if (!string.IsNullOrEmpty(e.Text))
			{
				string keyword = ((AppSetting.Instance.PvfConfig.DefaultEncoding == EncodingType.TW) ? ChineseHelper.ToTraditional(e.Text) : e.Text);
				if (Config.NameConvertCode)
				{
					autoSuggestEdit.ItemsSource = pVF.Strtable.SearchPanelGetKeywords(keyword);
				}
			}
			if (autoSuggestEdit.ItemsSource == null)
			{
				ImmediatePopup = false;
			}
			else
			{
				ImmediatePopup = true;
			}
		}
	}

	public void Dispose()
	{
		Document.TextChanged -= OnDocumentTextChanged;
		textArea.DocumentChanged -= OnTextAreaDocumentChanged;
		Config.EventDelegateConfigChanged -= OnConfigChanged;
		SearchRenderer.Segments.Clear();
	}
}
