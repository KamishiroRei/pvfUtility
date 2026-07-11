using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Search;

public interface ISearchResult : ISegment
{
	string ReplaceWith(string replacement);
}
