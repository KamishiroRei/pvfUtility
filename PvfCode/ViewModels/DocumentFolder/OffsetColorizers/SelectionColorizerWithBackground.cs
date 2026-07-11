using System;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;

namespace PvfCode.ViewModels.DocumentFolder.OffsetColorizers;

public class SelectionColorizerWithBackground : ColorizingTransformer
{
	private TextArea bKo46gcXNa;

	public SelectionColorizerWithBackground(TextArea textArea)
	{
		if (textArea == null)
		{
			throw new ArgumentNullException("textArea");
		}
		bKo46gcXNa = textArea;
	}

	protected override void Colorize(ITextRunConstructionContext context)
	{
		try
		{
			int offset = context.VisualLine.FirstDocumentLine.Offset;
			int num = context.VisualLine.LastDocumentLine.Offset + context.VisualLine.LastDocumentLine.TotalLength;
			foreach (SelectionSegment segment in bKo46gcXNa.Selection.Segments)
			{
				int startOffset = segment.StartOffset;
				if (startOffset >= num)
				{
					continue;
				}
				int endOffset = segment.EndOffset;
				if (endOffset <= offset)
				{
					continue;
				}
				int visualStartColumn = ((startOffset >= offset) ? context.VisualLine.ValidateVisualColumn(segment.StartOffset, segment.StartVisualColumn, bKo46gcXNa.Selection.EnableVirtualSpace) : 0);
				int visualEndColumn = ((endOffset <= num) ? context.VisualLine.ValidateVisualColumn(segment.EndOffset, segment.EndVisualColumn, bKo46gcXNa.Selection.EnableVirtualSpace) : (bKo46gcXNa.Selection.EnableVirtualSpace ? int.MaxValue : context.VisualLine.VisualLengthWithEndOfLineMarker));
				ChangeVisualElements(visualStartColumn, visualEndColumn, delegate(VisualLineElement P_0)
				{
					P_0.TextRunProperties.SetBackgroundBrush(Brushes.Transparent);
					if (bKo46gcXNa.SelectionForeground != null)
					{
						P_0.TextRunProperties.SetForegroundBrush(bKo46gcXNa.SelectionForeground);
					}
				});
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "SelectionColorizerWithBackground.Colorize");
		}
	}

	[CompilerGenerated]
	private void zyk4gBFP6I(VisualLineElement P_0)
	{
		P_0.TextRunProperties.SetBackgroundBrush(Brushes.Transparent);
		if (bKo46gcXNa.SelectionForeground != null)
		{
			P_0.TextRunProperties.SetForegroundBrush(bKo46gcXNa.SelectionForeground);
		}
	}
}
