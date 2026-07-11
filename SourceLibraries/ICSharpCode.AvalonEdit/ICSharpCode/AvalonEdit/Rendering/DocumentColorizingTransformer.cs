using System;
using System.Linq;
using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Rendering;

public abstract class DocumentColorizingTransformer : ColorizingTransformer
{
	private DocumentLine currentDocumentLine;

	private int firstLineStart;

	private int currentDocumentLineStartOffset;

	private int currentDocumentLineEndOffset;

	protected ITextRunConstructionContext CurrentContext { get; private set; }

	protected override void Colorize(ITextRunConstructionContext context)
	{
		if (context == null)
		{
			throw new ArgumentNullException("context");
		}
		CurrentContext = context;
		currentDocumentLine = context.VisualLine.FirstDocumentLine;
		firstLineStart = (currentDocumentLineStartOffset = currentDocumentLine.Offset);
		currentDocumentLineEndOffset = currentDocumentLineStartOffset + currentDocumentLine.Length;
		int num = currentDocumentLineStartOffset + currentDocumentLine.TotalLength;
		if (context.VisualLine.FirstDocumentLine == context.VisualLine.LastDocumentLine)
		{
			ColorizeLine(currentDocumentLine);
		}
		else
		{
			ColorizeLine(currentDocumentLine);
			VisualLineElement[] array = context.VisualLine.Elements.ToArray();
			foreach (VisualLineElement visualLineElement in array)
			{
				int num2 = firstLineStart + visualLineElement.RelativeTextOffset;
				if (num2 >= num)
				{
					currentDocumentLine = context.Document.GetLineByOffset(num2);
					currentDocumentLineStartOffset = currentDocumentLine.Offset;
					currentDocumentLineEndOffset = currentDocumentLineStartOffset + currentDocumentLine.Length;
					num = currentDocumentLineStartOffset + currentDocumentLine.TotalLength;
					ColorizeLine(currentDocumentLine);
				}
			}
		}
		currentDocumentLine = null;
		CurrentContext = null;
	}

	protected abstract void ColorizeLine(DocumentLine line);

	protected void ChangeLinePart(int startOffset, int endOffset, Action<VisualLineElement> action)
	{
		if (startOffset < currentDocumentLineStartOffset || startOffset > currentDocumentLineEndOffset)
		{
			throw new ArgumentOutOfRangeException("startOffset", startOffset, "Value must be between " + currentDocumentLineStartOffset + " and " + currentDocumentLineEndOffset);
		}
		if (endOffset < startOffset || endOffset > currentDocumentLineEndOffset)
		{
			throw new ArgumentOutOfRangeException("endOffset", endOffset, "Value must be between " + startOffset + " and " + currentDocumentLineEndOffset);
		}
		VisualLine visualLine = CurrentContext.VisualLine;
		int visualColumn = visualLine.GetVisualColumn(startOffset - firstLineStart);
		int visualColumn2 = visualLine.GetVisualColumn(endOffset - firstLineStart);
		if (visualColumn < visualColumn2)
		{
			ChangeVisualElements(visualColumn, visualColumn2, action);
		}
	}
}
