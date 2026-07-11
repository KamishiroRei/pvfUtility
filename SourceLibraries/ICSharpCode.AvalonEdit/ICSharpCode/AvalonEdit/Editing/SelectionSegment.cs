using System;
using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Editing;

public class SelectionSegment : ISegment
{
	private readonly int startOffset;

	private readonly int endOffset;

	private readonly int startVC;

	private readonly int endVC;

	public int StartOffset => startOffset;

	public int EndOffset => endOffset;

	public int StartVisualColumn => startVC;

	public int EndVisualColumn => endVC;

	int ISegment.Offset => startOffset;

	public int Length => endOffset - startOffset;

	public SelectionSegment(int startOffset, int endOffset)
	{
		this.startOffset = Math.Min(startOffset, endOffset);
		this.endOffset = Math.Max(startOffset, endOffset);
		startVC = (endVC = -1);
	}

	public SelectionSegment(int startOffset, int startVC, int endOffset, int endVC)
	{
		if (startOffset < endOffset || (startOffset == endOffset && startVC <= endVC))
		{
			this.startOffset = startOffset;
			this.startVC = startVC;
			this.endOffset = endOffset;
			this.endVC = endVC;
		}
		else
		{
			this.startOffset = endOffset;
			this.startVC = endVC;
			this.endOffset = startOffset;
			this.endVC = startVC;
		}
	}

	public override string ToString()
	{
		return $"[SelectionSegment StartOffset={startOffset}, EndOffset={endOffset}, StartVC={startVC}, EndVC={endVC}]";
	}
}
