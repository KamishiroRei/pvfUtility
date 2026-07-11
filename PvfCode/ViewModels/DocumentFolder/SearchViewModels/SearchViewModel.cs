using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
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
	[CompilerGenerated]
	private TextEditorBase xRGAAc2Wtq;

	[CompilerGenerated]
	private SearchResultBackgroundRenderer KyWA4hWSkO;

	private ISearchStrategy nCcAYv6YwD;

	[CompilerGenerated]
	private bool LrDAyREWb6;

	[CompilerGenerated]
	private Action uHBAiDuF04;

	private readonly string FilePath;

	private bool q1OAunDcr4;

	[CompilerGenerated]
	private SearchPanelVisibilityChanged A6xAGxHsua;

	private bool VEHAx8vweP;

	[CompilerGenerated]
	private bool uR6AQ45kBN;

	private TextEditorBase Editor
	{
		[CompilerGenerated]
		get
		{
			return xRGAAc2Wtq;
		}
		[CompilerGenerated]
		set
		{
			xRGAAc2Wtq = value;
		}
	}

	private TextArea textArea => Editor.TextArea;

	public SearchConfig Config
	{
		get
		{
			return GetProperty(() => Config);
		}
		set
		{
			SetProperty<SearchConfig>(() => Config, value, f6HSUNFLma);
		}
	}

	public bool ExistFind
	{
		get
		{
			if (Config.SourceType != SourceType.所有打开的文档)
			{
				if (KuPABJDoNI() != null && KuPABJDoNI().Segments != null && KuPABJDoNI().Segments.Count != 0)
				{
					return true;
				}
				return false;
			}
			return true;
		}
	}

	public Action SetFindKeywordFocused
	{
		[CompilerGenerated]
		get
		{
			return uHBAiDuF04;
		}
		[CompilerGenerated]
		set
		{
			uHBAiDuF04 = value;
		}
	}

	public bool Visibility
	{
		get
		{
			return q1OAunDcr4;
		}
		set
		{
			q1OAunDcr4 = value;
			RaisePropertyChanged("Visibility");
			if (!value)
			{
				KuPABJDoNI().Segments.Clear();
			}
			A6xAGxHsua?.Invoke(value);
		}
	}

	public bool ShowReplacePanel
	{
		get
		{
			return VEHAx8vweP;
		}
		set
		{
			VEHAx8vweP = value;
			RaisePropertyChanged("ShowReplacePanel");
		}
	}

	public int SearchResultCount
	{
		get
		{
			RaisePropertyChanged("ExistFind");
			if (KuPABJDoNI() == null || KuPABJDoNI().Segments == null || KuPABJDoNI().Segments.Count == 0)
			{
				return 0;
			}
			return KuPABJDoNI().Segments.Count;
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

	public event SearchPanelVisibilityChanged EventSearchPanelVisibilityChanged
	{
		[CompilerGenerated]
		add
		{
			SearchPanelVisibilityChanged searchPanelVisibilityChanged = A6xAGxHsua;
			SearchPanelVisibilityChanged searchPanelVisibilityChanged2;
			do
			{
				searchPanelVisibilityChanged2 = searchPanelVisibilityChanged;
				SearchPanelVisibilityChanged value2 = (SearchPanelVisibilityChanged)Delegate.Combine(searchPanelVisibilityChanged2, value);
				searchPanelVisibilityChanged = Interlocked.CompareExchange(ref A6xAGxHsua, value2, searchPanelVisibilityChanged2);
			}
			while ((object)searchPanelVisibilityChanged != searchPanelVisibilityChanged2);
		}
		[CompilerGenerated]
		remove
		{
			SearchPanelVisibilityChanged searchPanelVisibilityChanged = A6xAGxHsua;
			SearchPanelVisibilityChanged searchPanelVisibilityChanged2;
			do
			{
				searchPanelVisibilityChanged2 = searchPanelVisibilityChanged;
				SearchPanelVisibilityChanged value2 = (SearchPanelVisibilityChanged)Delegate.Remove(searchPanelVisibilityChanged2, value);
				searchPanelVisibilityChanged = Interlocked.CompareExchange(ref A6xAGxHsua, value2, searchPanelVisibilityChanged2);
			}
			while ((object)searchPanelVisibilityChanged != searchPanelVisibilityChanged2);
		}
	}

	[SpecialName]
	private TextDocument lEaAh3iJDQ()
	{
		return Editor.Document;
	}

	[SpecialName]
	[CompilerGenerated]
	private SearchResultBackgroundRenderer KuPABJDoNI()
	{
		return KyWA4hWSkO;
	}

	[SpecialName]
	[CompilerGenerated]
	private void shuAFfN5Mi(SearchResultBackgroundRenderer P_0)
	{
		KyWA4hWSkO = P_0;
	}

	[SpecialName]
	[CompilerGenerated]
	private bool IXCAWXgKM1()
	{
		return LrDAyREWb6;
	}

	[SpecialName]
	[CompilerGenerated]
	private void R7QAm7n6Kt(bool P_0)
	{
		LrDAyREWb6 = P_0;
	}

	public bool InTheWork()
	{
		return IXCAWXgKM1();
	}

	public SearchViewModel(TextEditorBase editor, string filePath)
	{
		FilePath = filePath;
		Editor = editor;
		Config = new SearchConfig();
		shuAFfN5Mi(new SearchResultBackgroundRenderer());
		if (lEaAh3iJDQ() != null)
		{
			lEaAh3iJDQ().TextChanged += dLuSNs4TUR;
		}
		textArea.DocumentChanged += SwOSzNjBGZ;
		Config.EventDelegateConfigChanged += f6HSUNFLma;
		editor.TextArea.TextView.BackgroundRenderers.Add(KuPABJDoNI());
	}

	private void f6HSUNFLma()
	{
		vHvAD6rsky();
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
				vHvAD6rsky();
				SetFindKeywordFocused?.Invoke();
			}
			else
			{
				KuPABJDoNI().Clear();
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

	[SpecialName]
	[CompilerGenerated]
	private bool m3pAfohNwj()
	{
		return uR6AQ45kBN;
	}

	[SpecialName]
	[CompilerGenerated]
	private void IOEA56onrd(bool P_0)
	{
		uR6AQ45kBN = P_0;
	}

	public void ShowSearchPanel(SearchConfig config, bool showReplacePanel)
	{
		try
		{
			IOEA56onrd(true);
			Visibility = true;
			Config.EventDelegateConfigChanged -= f6HSUNFLma;
			Config = config.ToJson().JsonToObject<SearchConfig>();
			Config.EventDelegateConfigChanged += f6HSUNFLma;
			if (showReplacePanel)
			{
				ShowReplacePanel = showReplacePanel;
			}
			else
			{
				SetFindKeywordFocused?.Invoke();
			}
			IOEA56onrd(false);
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
			mOPScX2Q71();
			switch (Config.SearchType)
			{
			case SearchType.Next:
				SvESMeO1IR();
				break;
			case SearchType.Previous:
				BZmSVMryol();
				break;
			case SearchType.All:
				if (Config.SourceType == SourceType.所有打开的文档)
				{
					AppCore.ViewModelBase.RootDocument.DocumentsSearchManager.FindAll();
				}
				else if (KuPABJDoNI().Segments.Any())
				{
					AppCore.ViewModelBase.RootDocument.DocumentsSearchManager.AllSearchResultViewModel.AddResult(FilePath, KuPABJDoNI().Segments);
				}
				else
				{
					AppCore.ViewModelBase.RootDocument.DocumentsSearchManager.AllSearchResultViewModel.Tree = null;
				}
				break;
			}
			SearchType searchType = Config.SearchType;
			if ((uint)searchType <= 1u && Config.SourceType != SourceType.所有打开的文档 && !KuPABJDoNI().Segments.Any() && AppSetting.Instance.EditConfig.SearchPanelFindNotFoundAllowMessageBox)
			{
				AppCore.Logger.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_CannotFindSearchResult"), Config.FindKeyword));
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "SearchViewModel.OnFindMain");
		}
	}

	private void mOPScX2Q71()
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

	private void oG6S85k4sX()
	{
		try
		{
			mOPScX2Q71();
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

	private void SvESMeO1IR()
	{
		try
		{
			if (xdWS3pqTxo())
			{
				return;
			}
			switch (Config.SourceType)
			{
			case SourceType.当前文档:
			{
				SearchResult searchResult = KuPABJDoNI().Segments.FindFirstSegmentWithStartAfter(textArea.Caret.Offset + 1);
				if (searchResult == null)
				{
					searchResult = KuPABJDoNI().Segments.FirstSegment;
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

	private void BZmSVMryol()
	{
		try
		{
			switch (Config.SourceType)
			{
			case SourceType.当前文档:
				if (!xdWS3pqTxo())
				{
					SearchResult searchResult = KuPABJDoNI().Segments.FindFirstSegmentWithStartAfter(textArea.Caret.Offset);
					if (searchResult != null)
					{
						searchResult = KuPABJDoNI().Segments.GetPreviousSegment(searchResult);
					}
					if (searchResult == null)
					{
						searchResult = KuPABJDoNI().Segments.LastSegment;
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

	private bool xdWS3pqTxo()
	{
		return string.IsNullOrEmpty(Config.FindKeyword);
	}

	[Command]
	public void OnReplace(bool isReplaceAll)
	{
		try
		{
			oG6S85k4sX();
			if (Config.SourceType != SourceType.所有打开的文档 && !KuPABJDoNI().Segments.Any())
			{
				AppCore.Logger.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_CannotFindSearchResult"), Config.FindKeyword));
				return;
			}
			if (isReplaceAll)
			{
				BAiSRTV9lf();
				return;
			}
			if (Config.SourceType == SourceType.所有打开的文档)
			{
				AppCore.ViewModelBase.RootDocument.DocumentsSearchManager.FindNext(FilePath, replace: true);
				return;
			}
			SearchResult searchResult = KuPABJDoNI().Segments.FindFirstSegmentWithStartAfter(textArea.Caret.Offset);
			if (searchResult == null)
			{
				searchResult = KuPABJDoNI().Segments.FirstSegment;
			}
			if (searchResult != null)
			{
				string text = nCcAYv6YwD.ReplaceNext(lEaAh3iJDQ(), searchResult.StartOffset, searchResult.Length, Config.FindKeyword, Config.ReplaceKeyword, Config.RegularExpression);
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

	private void BAiSRTV9lf()
	{
		try
		{
			if (Config.SourceType == SourceType.当前文档)
			{
				int searchResultCount = SearchResultCount;
				textArea.TextView.InvalidateLayer(KnownLayer.Selection);
				R7QAm7n6Kt(true);
				Editor.BeginChange();
				TextDocument document = Editor.Document;
				nCcAYv6YwD.ReplaceAll(document, Config);
				Editor.EndChange();
				R7QAm7n6Kt(false);
				vHvAD6rsky();
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

	private void dLuSNs4TUR(object? sender, EventArgs P_1)
	{
		if (!InTheWork())
		{
			vHvAD6rsky();
		}
	}

	private void SwOSzNjBGZ(object? sender, EventArgs P_1)
	{
		if (lEaAh3iJDQ() != null)
		{
			lEaAh3iJDQ().TextChanged -= dLuSNs4TUR;
		}
		if (lEaAh3iJDQ() != null)
		{
			lEaAh3iJDQ().TextChanged += dLuSNs4TUR;
			if (!InTheWork())
			{
				oqKAjFIaUZ(false);
			}
		}
	}

	private void vHvAD6rsky()
	{
		try
		{
			if (Config.SourceType == SourceType.所有打开的文档 && !m3pAfohNwj())
			{
				AppCore.ViewModelBase.RootDocument.DocumentsSearchManager.RefSetData(Config, FilePath);
			}
			if (!IXCAWXgKM1())
			{
				textArea.TextView.InvalidateLayer(KnownLayer.Background);
				if (KuPABJDoNI() != null)
				{
					KuPABJDoNI().Clear();
				}
				RaisePropertyChanged("SearchResultCount");
				if (!xdWS3pqTxo())
				{
					I6DAlBcMOj();
				}
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "SearchViewModel.ValidateSearchText");
		}
	}

	private async void I6DAlBcMOj()
	{
		KuPABJDoNI().Segments.Any();
		try
		{
			ResultData<ISearchStrategy> resultData = await SearchStrategyFactory.Create(Config);
			if (resultData.IsError)
			{
				AppCore.ShowMsg(resultData.Msg);
				return;
			}
			nCcAYv6YwD = resultData.Data;
		}
		catch (Exception ex)
		{
			AppCore.Logger.Error(ex.Message);
			IsPopupOpen = true;
			nCcAYv6YwD = null;
			return;
		}
		oqKAjFIaUZ(true);
	}

	private void oqKAjFIaUZ(bool P_0)
	{
		try
		{
			if (!Visibility)
			{
				RaisePropertyChanged("ExistFind");
				return;
			}
			KuPABJDoNI().Segments.Clear();
			if (!string.IsNullOrEmpty(Config.FindKeyword))
			{
				int offset = textArea.Caret.Offset;
				if (P_0)
				{
					textArea.ClearSelection();
				}
				foreach (SearchResult item in nCcAYv6YwD.FindAll(textArea.Document, 0, textArea.Document.TextLength))
				{
					if (P_0 && item.StartOffset >= offset)
					{
						_ = Editor.CodeCompletionIsOpen;
						P_0 = false;
					}
					KuPABJDoNI().Segments.Add(item);
				}
				KuPABJDoNI().Segments.Any();
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
		lEaAh3iJDQ().TextChanged -= dLuSNs4TUR;
		textArea.DocumentChanged -= SwOSzNjBGZ;
		Config.EventDelegateConfigChanged -= f6HSUNFLma;
		KuPABJDoNI().Segments.Clear();
	}
}
