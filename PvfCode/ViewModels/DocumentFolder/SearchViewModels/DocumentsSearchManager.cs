using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
		public ObservableConcurrentDictionaryEx<string, SearchResultNode> bgWeeuunAw;

		public ObservableConcurrentDictionaryEx<string, SearchResultNode> Tree
		{
			get
			{
				return bgWeeuunAw;
			}
			set
			{
				bgWeeuunAw = value;
			}
		}

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

	private List<SearchResult> vEWSIcyT8s;

	[CompilerGenerated]
	private bool ofjSEDuT33;

	private bool rEkSOgMyxy;

	[CompilerGenerated]
	private int? OW3SKrYbkB;

	[CompilerGenerated]
	private string NeyS9eubh9;

	[CompilerGenerated]
	private ISearchResult iykSPjMQC4;

	[CompilerGenerated]
	private int PMoSZa9Nju;

	[CompilerGenerated]
	private string fQmSJ1tEd4;

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
		vEWSIcyT8s = new List<SearchResult>();
	}

	[SpecialName]
	[CompilerGenerated]
	private bool e2bSunvJMq()
	{
		return ofjSEDuT33;
	}

	[SpecialName]
	[CompilerGenerated]
	private void oPLSGNF1Xe(bool P_0)
	{
		ofjSEDuT33 = P_0;
	}

	public void Clear()
	{
		oPLSGNF1Xe(false);
		Strategy = null;
		fm1S1whJ05(null);
		SEDSa59Psy(null);
		LZlStqyl4R(null);
		OCPSq7DBI8(0);
		rEkSOgMyxy = true;
		AllSearchResultViewModel.Clear();
	}

	public void RefSetData(SearchConfig config, string filePath)
	{
		if (JSfS6Zlt9g() != filePath || rEkSOgMyxy)
		{
			rEkSOgMyxy = false;
			fm1S1whJ05(filePath);
			SEDSa59Psy(null);
			LZlStqyl4R(null);
			OCPSq7DBI8(0);
		}
		Config = config;
		e2bSunvJMq();
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
		oPLSGNF1Xe(true);
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
		oPLSGNF1Xe(false);
	}

	[SpecialName]
	[CompilerGenerated]
	private int? dGmSQe7Hi4()
	{
		return OW3SKrYbkB;
	}

	[SpecialName]
	[CompilerGenerated]
	private void SEDSa59Psy(int? P_0)
	{
		OW3SKrYbkB = P_0;
	}

	[SpecialName]
	[CompilerGenerated]
	private string JSfS6Zlt9g()
	{
		return NeyS9eubh9;
	}

	[SpecialName]
	[CompilerGenerated]
	private void fm1S1whJ05(string P_0)
	{
		NeyS9eubh9 = P_0;
	}

	[SpecialName]
	[CompilerGenerated]
	private ISearchResult dEESo6PvQg()
	{
		return iykSPjMQC4;
	}

	[SpecialName]
	[CompilerGenerated]
	private void L3LSs4cgJ1(ISearchResult P_0)
	{
		iykSPjMQC4 = P_0;
	}

	[SpecialName]
	[CompilerGenerated]
	private int HViSneFSpo()
	{
		return PMoSZa9Nju;
	}

	[SpecialName]
	[CompilerGenerated]
	private void OCPSq7DBI8(int P_0)
	{
		PMoSZa9Nju = P_0;
	}

	[SpecialName]
	[CompilerGenerated]
	private string vW5Se90EeO()
	{
		return fQmSJ1tEd4;
	}

	[SpecialName]
	[CompilerGenerated]
	private void LZlStqyl4R(string P_0)
	{
		fQmSJ1tEd4 = P_0;
	}

	private PvfFileDocument s80SS00yml(string P_0)
	{
		Documents.TryGetValue(P_0, out PvfFileDocument value);
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
		if (!qvESYKLqvX())
		{
			AppCore.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_CannotFindSearchResult"), Config.FindKeyword));
			return;
		}
		if (!dGmSQe7Hi4().HasValue)
		{
			UowS4p9EQh(nowFile, replace);
			return;
		}
		PvfFileDocument pvfFileDocument = s80SS00yml(nowFile);
		eZsSA9ItlF(pvfFileDocument, replace);
	}

	private int eZsSA9ItlF(PvfFileDocument P_0, bool P_1)
	{
		if (P_0.GetEditor().TextArea.Caret.Offset > HViSneFSpo())
		{
			OCPSq7DBI8(P_0.GetEditor().TextArea.Caret.Offset);
		}
		ISearchResult searchResult = Strategy.FindNext(P_0.Document, HViSneFSpo(), P_0.Document.TextLength);
		if (searchResult == null)
		{
			PvfFileDocument pvfFileDocument = RdRSytynxu(P_0.FullPath);
			pvfFileDocument.GetEditor().TextArea.Caret.Offset = 0;
			OCPSq7DBI8(pvfFileDocument.GetEditor().TextArea.Caret.Offset);
			return eZsSA9ItlF(pvfFileDocument, P_1);
		}
		P_0.SearchPanel.SelectResult(searchResult);
		P_0.IsActive = true;
		OCPSq7DBI8(searchResult.EndOffset);
		LZlStqyl4R(P_0.FullPath);
		P_0.SearchPanel.ShowSearchPanel(Config, P_1);
		if (P_1)
		{
			TextEdit editor = P_0.GetEditor();
			string text = Strategy.ReplaceNext(editor.Document, searchResult.Offset, searchResult.Length, Config.FindKeyword, Config.ReplaceKeyword, Config.RegularExpression);
			if (!string.IsNullOrEmpty(text))
			{
				editor.Select(searchResult.Offset, text.Length);
			}
		}
		if (JSfS6Zlt9g() == P_0.FullPath && dGmSQe7Hi4().HasValue && dGmSQe7Hi4() == searchResult.EndOffset)
		{
			SEDSa59Psy(null);
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_CannotFindSearchResult_PleaseSearchAgain2"));
		}
		else if (!dGmSQe7Hi4().HasValue)
		{
			SEDSa59Psy(searchResult.EndOffset);
			fm1S1whJ05(P_0.FullPath);
		}
		return searchResult.EndOffset;
	}

	private void UowS4p9EQh(string P_0, bool P_1)
	{
		PvfFileDocument pvfFileDocument = s80SS00yml(P_0);
		if (pvfFileDocument == null)
		{
			return;
		}
		TextEdit editor = pvfFileDocument.GetEditor();
		string selectedText = editor.SelectedText;
		if (selectedText != null && selectedText == Config.FindKeyword)
		{
			LZlStqyl4R(P_0);
			SEDSa59Psy(editor.SelectionStart + editor.SelectionLength);
			OCPSq7DBI8(dGmSQe7Hi4().Value);
			fm1S1whJ05(P_0);
			if (P_1)
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
			OCPSq7DBI8(editor.GetCaretLineOffset());
		}
		eZsSA9ItlF(pvfFileDocument, P_1);
	}

	private bool qvESYKLqvX()
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

	private PvfFileDocument RdRSytynxu(string P_0)
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
				if (value.FullPath == P_0)
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
			if (item.FullPath == P_0)
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
