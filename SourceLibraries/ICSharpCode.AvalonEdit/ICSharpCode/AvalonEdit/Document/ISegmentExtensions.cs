namespace ICSharpCode.AvalonEdit.Document;

public static class ISegmentExtensions
{
	public static bool Contains(this ISegment segment, int offset, int length)
	{
		if (segment.Offset <= offset)
		{
			return offset + length <= segment.EndOffset;
		}
		return false;
	}

	public static bool Contains(this ISegment thisSegment, ISegment segment)
	{
		if (segment != null && thisSegment.Offset <= segment.Offset)
		{
			return segment.EndOffset <= thisSegment.EndOffset;
		}
		return false;
	}
}
