using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Mvvm;
using ICSharpCode.AvalonEdit.Document;
using PvfCode.Controls.TextEditorFolder;
using PvfCode.Dot;
using PvfCode.LoggerBase;
using PvfCode.ViewModels.DocumentFolder.Enums;
using PvfCode.ViewModels.DocumentFolder.SearchViewModels.AllSearchResultDir;
using PvfCode.ViewModels.DocumentFolder.SearchViewModels.Enums;
using PvfCode.ViewModels.DocumentFolder.SearchViewModels.Searchs;
using TextEditLib;
using Utools;

namespace PvfCode.ViewModels.DocumentFolder.SearchViewModels;

public class DocumentsSearchManager : ViewModelBase
{
	internal class wFHHWYenfs8COlq055l
	{
		public ObservableConcurrentDictionaryEx<string, SearchResultNode> Tree { get; set; }

		public void Create(string filePath, IEnumerable<ISearchResult> searchResult)
		{
			if (Tree == null)
			{
				Tree = new ObservableConcurrentDictionaryEx<string, SearchResultNode>();
			}
			ObservableConcurrentDictionaryEx<string, SearchResultNode> observableConcurrentDictionaryEx = Tree;
			if (!(AppCore.ViewModelBase.RootDocument.GetDocument(filePath) is PvfFileDocument pvfFileDocument))
			{
				return;
			}
			TextDocument document = pvfFileDocument.Document;
			if (observableConcurrentDictionaryEx.TryGetValue(filePath, out var value))
			{
				ObservableConcurrentDictionaryEx<string, SearchResultNode> children = value.Children;
				{
					foreach (ISearchResult item in searchResult)
					{
						string key = item.Offset.ToString();
						if (!children.ContainsKey(key))
						{
							DocumentLine lineByOffset = document.GetLineByOffset(item.Offset);
							int offset = lineByOffset.Offset;
							int column = 0;
							if (offset < item.Offset)
							{
								column = item.Offset - offset;
							}
							children.Add(key, new SearchResultNode(item, document.GetText(lineByOffset), lineByOffset.LineNumber, column));
						}
					}
					return;
				}
			}
			observableConcurrentDictionaryEx.AddTry(filePath, new SearchResultNode(null, null, 0, 0));
			ObservableConcurrentDictionaryEx<string, SearchResultNode> children2 = observableConcurrentDictionaryEx[filePath].Children;
			foreach (ISearchResult item2 in searchResult)
			{
				string key2 = item2.Offset.ToString();
				if (!children2.ContainsKey(key2))
				{
					DocumentLine lineByOffset2 = document.GetLineByOffset(item2.Offset);
					int offset2 = lineByOffset2.Offset;
					int column2 = 0;
					if (offset2 < item2.Offset)
					{
						column2 = item2.Offset - offset2;
					}
					children2.Add(key2, new SearchResultNode(item2, document.GetText(lineByOffset2), lineByOffset2.LineNumber, column2));
				}
			}
		}

		public wFHHWYenfs8COlq055l()
		{
		}
	}

	public SearchConfig Config;

	public ISearchStrategy Strategy;

	private bool resetSearchState;

	private bool IsReplacing { get; set; }

	private int? FirstResultEndOffset { get; set; }

	private string FirstResultFilePath { get; set; }

	private int CurrentOffset { get; set; }

	private string CurrentFilePath { get; set; }

	public AllSearchResultViewModel AllSearchResultViewModel
	{
		get
		{
			return GetProperty(() => AllSearchResultViewModel);
		}
		set
		{
			SetProperty<AllSearchResultViewModel>(() => AllSearchResultViewModel, value);
		}
	}

	private IDictionary<string, PvfFileDocument> Documents => AppCore.ViewModelBase.RootDocument.Documents.Where((DocumentBase it) => it is PvfFileDocument).ToDictionary((DocumentBase it) => ((PvfFileDocument)it).FullPath, (DocumentBase it) => (PvfFileDocument)it);

	public DocumentsSearchManager()
	{
		AllSearchResultViewModel = new AllSearchResultViewModel();
	}

	public void Clear()
	{
		IsReplacing = false;
		Strategy = null;
		FirstResultFilePath = null;
		FirstResultEndOffset = null;
		CurrentFilePath = null;
		CurrentOffset = 0;
		resetSearchState = true;
		AllSearchResultViewModel.Clear();
	}

	public void RefSetData(SearchConfig config, string filePath)
	{
		if (FirstResultFilePath != filePath || resetSearchState)
		{
			resetSearchState = false;
			FirstResultFilePath = filePath;
			FirstResultEndOffset = null;
			CurrentFilePath = null;
			CurrentOffset = 0;
		}
		Config = config;
	}

	public async void ReplaceAll()
	{
		try
		{
			ResultData<ISearchStrategy> resultData = await SearchStrategyFactory.Create(Config);
			if (resultData.IsError)
			{
				AppCore.ShowMsg(resultData.Msg);
				return;
			}
			Strategy = resultData.Data;
		}
		catch (Exception ex)
		{
			AppCore.Logger.Error(ex.Message);
		}
		if (Strategy == null || Config == null)
		{
			AppCore.Logger.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_FindParamError"), isError: true);
			return;
		}
		IsReplacing = true;
		int num = 0;
		foreach (PvfFileDocument value in Documents.Values)
		{
			if (value.DocumentType == PvfFileDocumentType.PVF文档)
			{
				TextDocument document = value.Document;
				if (Strategy.FindNext(document, 0, document.TextLength) != null)
				{
					num += Strategy.ReplaceAll(document, Config);
				}
			}
		}
		if (num == 0)
		{
			AppCore.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_CannotFindSearchResult"), Config.FindKeyword));
		}
		else
		{
			AppCore.Logger.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_ReplaceSuccessCount"), num));
		}
		IsReplacing = false;
	}

	private PvfFileDocument GetDocument(string filePath)
	{
		Documents.TryGetValue(filePath, out PvfFileDocument value);
		return value;
	}

	public async void FindNext(string nowFile, bool replace)
	{
		ResultData<ISearchStrategy> resultData = await SearchStrategyFactory.Create(Config);
		if (resultData.IsError)
		{
			AppCore.ShowMsg(resultData.Msg);
			return;
		}
		Strategy = resultData.Data;
		if (!HasAnyResult())
		{
			AppCore.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_CannotFindSearchResult"), Config.FindKeyword));
			return;
		}
		if (!FirstResultEndOffset.HasValue)
		{
			StartSearch(nowFile, replace);
			return;
		}
		PvfFileDocument pvfFileDocument = GetDocument(nowFile);
		FindNextInDocument(pvfFileDocument, replace);
	}

	private int FindNextInDocument(PvfFileDocument document, bool replace)
	{
		if (document.GetEditor().TextArea.Caret.Offset > CurrentOffset)
		{
			CurrentOffset = document.GetEditor().TextArea.Caret.Offset;
		}
		ISearchResult searchResult = Strategy.FindNext(document.Document, CurrentOffset, document.Document.TextLength);
		if (searchResult == null)
		{
			PvfFileDocument pvfFileDocument = GetAdjacentDocument(document.FullPath);
			pvfFileDocument.GetEditor().TextArea.Caret.Offset = 0;
			CurrentOffset = pvfFileDocument.GetEditor().TextArea.Caret.Offset;
			return FindNextInDocument(pvfFileDocument, replace);
		}
		document.SearchPanel.SelectResult(searchResult);
		document.IsActive = true;
		CurrentOffset = searchResult.EndOffset;
		CurrentFilePath = document.FullPath;
		document.SearchPanel.ShowSearchPanel(Config, replace);
		if (replace)
		{
			TextEdit editor = document.GetEditor();
			string text = Strategy.ReplaceNext(editor.Document, searchResult.Offset, searchResult.Length, Config.FindKeyword, Config.ReplaceKeyword, Config.RegularExpression);
			if (!string.IsNullOrEmpty(text))
			{
				editor.Select(searchResult.Offset, text.Length);
			}
		}
		if (FirstResultFilePath == document.FullPath && FirstResultEndOffset.HasValue && FirstResultEndOffset == searchResult.EndOffset)
		{
			FirstResultEndOffset = null;
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_CannotFindSearchResult_PleaseSearchAgain2"));
		}
		else if (!FirstResultEndOffset.HasValue)
		{
			FirstResultEndOffset = searchResult.EndOffset;
			FirstResultFilePath = document.FullPath;
		}
		return searchResult.EndOffset;
	}

	private void StartSearch(string filePath, bool replace)
	{
		PvfFileDocument pvfFileDocument = GetDocument(filePath);
		if (pvfFileDocument == null)
		{
			return;
		}
		TextEdit editor = pvfFileDocument.GetEditor();
		string selectedText = editor.SelectedText;
		if (selectedText != null && selectedText == Config.FindKeyword)
		{
			CurrentFilePath = filePath;
			FirstResultEndOffset = editor.SelectionStart + editor.SelectionLength;
			CurrentOffset = FirstResultEndOffset.Value;
			FirstResultFilePath = filePath;
			if (replace)
			{
				TextDocument document = pvfFileDocument.Document;
				int selectionStart = editor.SelectionStart;
				string text = Strategy.ReplaceNext(document, editor.SelectionStart, editor.SelectionStart + editor.SelectionLength, Config.FindKeyword, Config.ReplaceKeyword, Config.RegularExpression);
				if (!string.IsNullOrEmpty(text))
				{
					editor.Select(selectionStart, text.Length);
				}
				return;
			}
		}
		else
		{
			CurrentOffset = editor.GetCaretLineOffset();
		}
		FindNextInDocument(pvfFileDocument, replace);
	}

	private bool HasAnyResult()
	{
		foreach (PvfFileDocument value in Documents.Values)
		{
			if (value.DocumentType == PvfFileDocumentType.PVF文档 && Strategy.FindNext(value.Document, 0, value.Document.TextLength) != null)
			{
				return true;
			}
		}
		return false;
	}

	private PvfFileDocument GetAdjacentDocument(string filePath)
	{
		bool flag = false;
		if (Config.SearchType == SearchType.Next)
		{
			foreach (PvfFileDocument value in Documents.Values)
			{
				if (flag)
				{
					return value;
				}
				if (value.FullPath == filePath)
				{
					flag = true;
				}
			}
			return Documents.Values.FirstOrDefault();
		}
		List<PvfFileDocument> list = Documents.Values.ToList();
		list.Reverse();
		foreach (PvfFileDocument item in list)
		{
			if (flag)
			{
				return item;
			}
			if (item.FullPath == filePath)
			{
				flag = true;
			}
		}
		return list.FirstOrDefault();
	}

	public async void FindAll()
	{
		wFHHWYenfs8COlq055l treeGroup = new wFHHWYenfs8COlq055l();
		ResultData<ISearchStrategy> resultData = await SearchStrategyFactory.Create(Config);
		if (resultData.IsError)
		{
			AppCore.ShowMsg(resultData.Msg);
			return;
		}
		Strategy = resultData.Data;
		int num = 0;
		foreach (KeyValuePair<string, PvfFileDocument> item in Documents)
		{
			if (item.Value.DocumentType == PvfFileDocumentType.PVF文档)
			{
				IEnumerable<ISearchResult> enumerable = Strategy.FindAll(item.Value.Document, 0, item.Value.Document.TextLength);
				if (enumerable != null && enumerable.Any())
				{
					num += enumerable.Count();
					treeGroup.Create(item.Key, enumerable);
				}
			}
		}
		AllSearchResultViewModel.Tree = treeGroup.Tree;
		AllSearchResultViewModel.IsActive = true;
		string message = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_AllOpendDocumentFindResultCount"), num);
		await AppCore.Logger.ShowNotification(new NotificationViewModel(AppSetting.Instance.AppName, message, Res.Instance.VisualStudioBlendLogo2015Pre_16x));
	}
}
