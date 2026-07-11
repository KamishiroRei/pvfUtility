using System.Text.RegularExpressions;
using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Search;

internal class SearchResult : TextSegment, ISearchResult, ISegment
{
	public Match Data { get; set; }

	public string ReplaceWith(string replacement)
	{
		return Data.Result(replacement);
	}
}
