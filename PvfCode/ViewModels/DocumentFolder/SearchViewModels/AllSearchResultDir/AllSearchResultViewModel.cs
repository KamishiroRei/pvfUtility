using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DevExpress.Mvvm;
using DevExpress.Xpf.Grid;
using ICSharpCode.AvalonEdit.Document;
using PvfCode.ViewModels.DocumentFolder.SearchViewModels.Searchs;
using PvfCode.ViewModels.TreeFolder;
using Utools;

namespace PvfCode.ViewModels.DocumentFolder.SearchViewModels.AllSearchResultDir;

public class AllSearchResultViewModel : ViewModelBase
{
	public delegate TreeListNode ContentToNode(object obj);

	[CompilerGenerated]
	private ContentToNode sNIApuqujX;

	public ObservableConcurrentDictionaryEx<string, SearchResultNode> _Tree;

	[CompilerGenerated]
	private PVfTreeChildrenSelector BRpAUFYfxU;

	public bool IsActive
	{
		get
		{
			return GetProperty(() => IsActive);
		}
		set
		{
			SetProperty(() => IsActive, value);
		}
	}

	public ContentToNode ContentToNodeMethods
	{
		[CompilerGenerated]
		get
		{
			return sNIApuqujX;
		}
		[CompilerGenerated]
		set
		{
			sNIApuqujX = value;
		}
	}

	public ObservableConcurrentDictionaryEx<string, SearchResultNode> Tree
	{
		get
		{
			return _Tree;
		}
		set
		{
			_Tree = value;
			RaisePropertyChanged("Tree");
		}
	}

	public KeyValuePair<string, SearchResultNode>? FocusRow
	{
		get
		{
			return GetProperty(() => FocusRow);
		}
		set
		{
			SetProperty<KeyValuePair<string, SearchResultNode>?>(() => FocusRow, value, WsXA0IemqC);
		}
	}

	public PVfTreeChildrenSelector ChildNodesSelector
	{
		[CompilerGenerated]
		get
		{
			return BRpAUFYfxU;
		}
		[CompilerGenerated]
		set
		{
			BRpAUFYfxU = value;
		}
	}

	public void Clear()
	{
		Tree?.Clear();
	}

	public AllSearchResultViewModel()
	{
		ChildNodesSelector = new PVfTreeChildrenSelector(vAVA7JUXn4);
	}

	private void WsXA0IemqC()
	{
		if (!FocusRow.HasValue)
		{
			return;
		}
		TreeListNode treeListNode = ContentToNodeMethods(FocusRow);
		if (treeListNode == null || treeListNode.ParentNode == null)
		{
			return;
		}
		DocumentBase document = AppCore.ViewModelBase.RootDocument.GetDocument(((KeyValuePair<string, SearchResultNode>)treeListNode.ParentNode.Content).Key);
		if (document != null && document is PvfFileDocument pvfFileDocument)
		{
			document.IsActive = true;
			if (!pvfFileDocument.SearchPanel.SelectResult(FocusRow.Value.Value.SearchResult))
			{
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_CannotFindSearchResult_PleaseSearchAgain"));
			}
		}
	}

	private IEnumerable vAVA7JUXn4(object P_0)
	{
		if (P_0 == null)
		{
			return null;
		}
		return ((KeyValuePair<string, SearchResultNode>)P_0).Value.Children;
	}

	public void AddResult(string filePath, IEnumerable<ISearchResult> searchResult)
	{
		Tree = null;
		ObservableConcurrentDictionaryEx<string, SearchResultNode> observableConcurrentDictionaryEx = new ObservableConcurrentDictionaryEx<string, SearchResultNode>();
		if (!(AppCore.ViewModelBase.RootDocument.GetDocument(filePath) is PvfFileDocument pvfFileDocument))
		{
			return;
		}
		TextDocument document = pvfFileDocument.Document;
		pvfFileDocument.GetEditor();
		if (observableConcurrentDictionaryEx.TryGetValue(filePath, out var value))
		{
			ObservableConcurrentDictionaryEx<string, SearchResultNode> children = value.Children;
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
		}
		else
		{
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
		Tree = observableConcurrentDictionaryEx;
		Tree.NotifyObserversOfChange();
		IsActive = true;
	}
}
