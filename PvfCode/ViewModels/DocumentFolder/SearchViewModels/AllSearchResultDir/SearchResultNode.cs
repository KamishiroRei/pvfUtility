using System.Runtime.CompilerServices;
using PvfCode.ViewModels.DocumentFolder.SearchViewModels.Searchs;
using Utools;

namespace PvfCode.ViewModels.DocumentFolder.SearchViewModels.AllSearchResultDir;

public class SearchResultNode
{
	[CompilerGenerated]
	private string xF6AcWSYoV;

	[CompilerGenerated]
	private int QATA8SfGDF;

	[CompilerGenerated]
	private int nFsAM6GXxi;

	[CompilerGenerated]
	private ISearchResult rD0AVEIVuX;

	private ObservableConcurrentDictionaryEx<string, SearchResultNode> tQKA3llaDI;

	public string Text
	{
		[CompilerGenerated]
		get
		{
			return xF6AcWSYoV;
		}
		[CompilerGenerated]
		set
		{
			xF6AcWSYoV = value;
		}
	}

	public int LineNumber
	{
		[CompilerGenerated]
		get
		{
			return QATA8SfGDF;
		}
		[CompilerGenerated]
		set
		{
			QATA8SfGDF = value;
		}
	}

	public int Column
	{
		[CompilerGenerated]
		get
		{
			return nFsAM6GXxi;
		}
		[CompilerGenerated]
		set
		{
			nFsAM6GXxi = value;
		}
	}

	public ISearchResult SearchResult
	{
		[CompilerGenerated]
		get
		{
			return rD0AVEIVuX;
		}
		[CompilerGenerated]
		set
		{
			rD0AVEIVuX = value;
		}
	}

	public ObservableConcurrentDictionaryEx<string, SearchResultNode> Children
	{
		get
		{
			if (tQKA3llaDI == null)
			{
				tQKA3llaDI = new ObservableConcurrentDictionaryEx<string, SearchResultNode>();
			}
			return tQKA3llaDI;
		}
		set
		{
			tQKA3llaDI = value;
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
