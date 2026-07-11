using System;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Document;

namespace PvfCode.ViewModels.DocumentFolder.SearchViewModels.Searchs;

public interface ISearchStrategy : IEquatable<ISearchStrategy>
{
	IEnumerable<ISearchResult> FindAll(ITextSource document, int offset, int length);

	ISearchResult FindNext(ITextSource document, int offset, int length);

	int ReplaceAll(IDocument document, SearchConfig config);

	string ReplaceNext(IDocument document, int startOffset, int length, string findKeyword, string replaceText, bool useRegularExpression);
}
