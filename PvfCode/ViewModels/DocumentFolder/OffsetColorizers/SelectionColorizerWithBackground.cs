using System;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;

namespace PvfCode.ViewModels.DocumentFolder.OffsetColorizers;

public class SelectionColorizerWithBackground : ColorizingTransformer
{
	private TextArea textArea;

	public SelectionColorizerWithBackground(TextArea textArea)
	{
		if (textArea == null)
		{
			throw new ArgumentNullException("textArea");
		}
		this.textArea = textArea;
	}

	protected override void Colorize(ITextRunConstructionContext context)
	{
		try
		{
			int lineStartOffset = context.VisualLine.FirstDocumentLine.Offset;
			int lineEndOffset = context.VisualLine.LastDocumentLine.Offset + context.VisualLine.LastDocumentLine.TotalLength;
			foreach (SelectionSegment segment in textArea.Selection.Segments)
			{
				int startOffset = segment.StartOffset;
				if (startOffset >= lineEndOffset)
				{
					continue;
				}
				int endOffset = segment.EndOffset;
				if (endOffset <= lineStartOffset)
				{
					continue;
				}
				int visualStartColumn = (startOffset >= lineStartOffset) ? context.VisualLine.ValidateVisualColumn(segment.StartOffset, segment.StartVisualColumn, textArea.Selection.EnableVirtualSpace) : 0;
				int visualEndColumn = (endOffset <= lineEndOffset) ? context.VisualLine.ValidateVisualColumn(segment.EndOffset, segment.EndVisualColumn, textArea.Selection.EnableVirtualSpace) : (textArea.Selection.EnableVirtualSpace ? int.MaxValue : context.VisualLine.VisualLengthWithEndOfLineMarker);
				ChangeVisualElements(visualStartColumn, visualEndColumn, delegate(VisualLineElement element)
				{
					element.TextRunProperties.SetBackgroundBrush(Brushes.Transparent);
					if (textArea.SelectionForeground != null)
					{
						element.TextRunProperties.SetForegroundBrush(textArea.SelectionForeground);
					}
				});
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "SelectionColorizerWithBackground.Colorize");
		}
	}
}
