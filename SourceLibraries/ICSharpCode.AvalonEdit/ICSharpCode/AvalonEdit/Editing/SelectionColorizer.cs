using System;
using ICSharpCode.AvalonEdit.Rendering;

namespace ICSharpCode.AvalonEdit.Editing;

public sealed class SelectionColorizer : ColorizingTransformer
{
	private TextArea textArea;

	public SelectionColorizer(TextArea textArea)
	{
		if (textArea == null)
		{
			throw new ArgumentNullException("textArea");
		}
		this.textArea = textArea;
	}

	protected override void Colorize(ITextRunConstructionContext context)
	{
		if (textArea.SelectionForeground == null)
		{
			return;
		}
		int offset = context.VisualLine.FirstDocumentLine.Offset;
		int num = context.VisualLine.LastDocumentLine.Offset + context.VisualLine.LastDocumentLine.TotalLength;
		foreach (SelectionSegment segment in textArea.Selection.Segments)
		{
			int startOffset = segment.StartOffset;
			int endOffset = segment.EndOffset;
			if (endOffset > offset && startOffset < num)
			{
				int visualStartColumn = ((startOffset >= offset) ? context.VisualLine.ValidateVisualColumn(segment.StartOffset, segment.StartVisualColumn, textArea.Selection.EnableVirtualSpace) : 0);
				int visualEndColumn = ((endOffset <= num) ? context.VisualLine.ValidateVisualColumn(segment.EndOffset, segment.EndVisualColumn, textArea.Selection.EnableVirtualSpace) : (textArea.Selection.EnableVirtualSpace ? int.MaxValue : context.VisualLine.VisualLengthWithEndOfLineMarker));
				ChangeVisualElements(visualStartColumn, visualEndColumn, delegate(VisualLineElement element)
				{
					element.TextRunProperties.SetForegroundBrush(textArea.SelectionForeground);
				});
			}
		}
	}
}
