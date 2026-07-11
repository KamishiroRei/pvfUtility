using System.Collections.Generic;
using System.Linq;
using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Editing;

internal sealed class ReadOnlySectionDocument : IReadOnlySectionProvider
{
	public static readonly ReadOnlySectionDocument Instance = new ReadOnlySectionDocument();

	public bool CanInsert(int offset)
	{
		return false;
	}

	public IEnumerable<ISegment> GetDeletableSegments(ISegment segment)
	{
		return Enumerable.Empty<ISegment>();
	}
}
