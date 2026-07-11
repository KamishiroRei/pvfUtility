using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using ICSharpCode.AvalonEdit.Document;

namespace PvfCode.ViewModels.DocumentFolder.SearchViewModels.Searchs;

public class SearchResult : TextSegment, ISearchResult, ISegment
{
	[CompilerGenerated]
	private Match PEYA1DamVT;

	public Match Data
	{
		[CompilerGenerated]
		get
		{
			return PEYA1DamVT;
		}
		[CompilerGenerated]
		set
		{
			PEYA1DamVT = value;
		}
	}

	public string ReplaceWith(string replacement)
	{
		return Data.Result(replacement);
	}

	public SearchResult()
	{
	}
}
