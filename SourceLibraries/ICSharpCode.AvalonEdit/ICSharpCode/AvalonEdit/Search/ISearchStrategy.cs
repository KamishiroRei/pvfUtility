using System;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Search;

public interface ISearchStrategy : IEquatable<ISearchStrategy>
{
	IEnumerable<ISearchResult> FindAll(ITextSource document, int offset, int length);

	ISearchResult FindNext(ITextSource document, int offset, int length);
}
