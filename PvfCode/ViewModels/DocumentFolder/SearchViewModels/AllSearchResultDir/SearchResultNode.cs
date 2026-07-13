using PvfCode.ViewModels.DocumentFolder.SearchViewModels.Searchs;
using Utools;

namespace PvfCode.ViewModels.DocumentFolder.SearchViewModels.AllSearchResultDir;

public class SearchResultNode
{
	private ObservableConcurrentDictionaryEx<string, SearchResultNode> children;

	public string Text { get; set; }

	public int LineNumber { get; set; }

	public int Column { get; set; }

	public ISearchResult SearchResult { get; set; }

	public ObservableConcurrentDictionaryEx<string, SearchResultNode> Children
	{
		get
		{
			if (children == null)
			{
				children = new ObservableConcurrentDictionaryEx<string, SearchResultNode>();
			}
			return children;
		}
		set
		{
			children = value;
		}
	}

	public SearchResultNode(ISearchResult searchResult, string text, int lineNumber, int column)
	{
		SearchResult = searchResult;
		Text = text;
		LineNumber = lineNumber;
		Column = column;
	}
}
