using ICSharpCode.AvalonEdit.Document;

namespace PvfCode.ViewModels.DocumentFolder.SearchViewModels.Searchs;

public interface ISearchResult : ISegment
{
	string ReplaceWith(string replacement);
}
