using System;
using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Folding;

public class NewFolding : ISegment
{
	public int StartOffset { get; set; }

	public int EndOffset { get; set; }

	public string Name { get; set; }

	public bool DefaultClosed { get; set; }

	public bool IsDefinition { get; set; }

	int ISegment.Offset => StartOffset;

	int ISegment.Length => EndOffset - StartOffset;

	public NewFolding()
	{
	}

	public NewFolding(int start, int end)
	{
		if (start > end)
		{
			throw new ArgumentException("'start' must be less than 'end'");
		}
		StartOffset = start;
		EndOffset = end;
		Name = null;
		DefaultClosed = false;
	}
}
