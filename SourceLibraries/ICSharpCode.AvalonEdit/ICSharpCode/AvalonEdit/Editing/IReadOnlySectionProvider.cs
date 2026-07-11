using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Editing;

public interface IReadOnlySectionProvider
{
	bool CanInsert(int offset);

	IEnumerable<ISegment> GetDeletableSegments(ISegment segment);
}
