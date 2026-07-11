using System;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Rendering;

public abstract class VisualLineElement
{
	public int VisualLength { get; private set; }

	public int DocumentLength { get; private set; }

	public int VisualColumn { get; internal set; }

	public int RelativeTextOffset { get; internal set; }

	public VisualLineElementTextRunProperties TextRunProperties { get; private set; }

	public Brush BackgroundBrush { get; set; }

	public virtual bool CanSplit => false;

	public virtual bool HandlesLineBorders => false;

	protected VisualLineElement(int visualLength, int documentLength)
	{
		if (visualLength < 1)
		{
			throw new ArgumentOutOfRangeException("visualLength", visualLength, "Value must be at least 1");
		}
		if (documentLength < 0)
		{
			throw new ArgumentOutOfRangeException("documentLength", documentLength, "Value must be at least 0");
		}
		VisualLength = visualLength;
		DocumentLength = documentLength;
	}

	internal void SetTextRunProperties(VisualLineElementTextRunProperties p)
	{
		TextRunProperties = p;
	}

	public abstract TextRun CreateTextRun(int startVisualColumn, ITextRunConstructionContext context);

	public virtual TextSpan<CultureSpecificCharacterBufferRange> GetPrecedingText(int visualColumnLimit, ITextRunConstructionContext context)
	{
		return null;
	}

	public virtual void Split(int splitVisualColumn, IList<VisualLineElement> elements, int elementIndex)
	{
		throw new NotSupportedException();
	}

	protected void SplitHelper(VisualLineElement firstPart, VisualLineElement secondPart, int splitVisualColumn, int splitRelativeTextOffset)
	{
		if (firstPart == null)
		{
			throw new ArgumentNullException("firstPart");
		}
		if (secondPart == null)
		{
			throw new ArgumentNullException("secondPart");
		}
		int num = splitVisualColumn - VisualColumn;
		int num2 = splitRelativeTextOffset - RelativeTextOffset;
		if (num <= 0 || num >= VisualLength)
		{
			throw new ArgumentOutOfRangeException("splitVisualColumn", splitVisualColumn, "Value must be between " + (VisualColumn + 1) + " and " + (VisualColumn + VisualLength - 1));
		}
		if (num2 < 0 || num2 > DocumentLength)
		{
			throw new ArgumentOutOfRangeException("splitRelativeTextOffset", splitRelativeTextOffset, "Value must be between " + RelativeTextOffset + " and " + (RelativeTextOffset + DocumentLength));
		}
		int visualLength = VisualLength;
		int documentLength = DocumentLength;
		int visualColumn = VisualColumn;
		int relativeTextOffset = RelativeTextOffset;
		firstPart.VisualColumn = visualColumn;
		secondPart.VisualColumn = visualColumn + num;
		firstPart.RelativeTextOffset = relativeTextOffset;
		secondPart.RelativeTextOffset = relativeTextOffset + num2;
		firstPart.VisualLength = num;
		secondPart.VisualLength = visualLength - num;
		firstPart.DocumentLength = num2;
		secondPart.DocumentLength = documentLength - num2;
		if (firstPart.TextRunProperties == null)
		{
			firstPart.TextRunProperties = TextRunProperties.Clone();
		}
		if (secondPart.TextRunProperties == null)
		{
			secondPart.TextRunProperties = TextRunProperties.Clone();
		}
		firstPart.BackgroundBrush = BackgroundBrush;
		secondPart.BackgroundBrush = BackgroundBrush;
	}

	public virtual int GetVisualColumn(int relativeTextOffset)
	{
		if (relativeTextOffset >= RelativeTextOffset + DocumentLength)
		{
			return VisualColumn + VisualLength;
		}
		return VisualColumn;
	}

	public virtual int GetRelativeOffset(int visualColumn)
	{
		if (visualColumn >= VisualColumn + VisualLength)
		{
			return RelativeTextOffset + DocumentLength;
		}
		return RelativeTextOffset;
	}

	public virtual int GetNextCaretPosition(int visualColumn, LogicalDirection direction, CaretPositioningMode mode)
	{
		int visualColumn2 = VisualColumn;
		int num = VisualColumn + VisualLength;
		if (direction == LogicalDirection.Backward)
		{
			if (visualColumn > num && mode != CaretPositioningMode.WordStart && mode != CaretPositioningMode.WordStartOrSymbol)
			{
				return num;
			}
			if (visualColumn > visualColumn2)
			{
				return visualColumn2;
			}
		}
		else
		{
			if (visualColumn < visualColumn2)
			{
				return visualColumn2;
			}
			if (visualColumn < num && mode != CaretPositioningMode.WordStart && mode != CaretPositioningMode.WordStartOrSymbol)
			{
				return num;
			}
		}
		return -1;
	}

	public virtual bool IsWhitespace(int visualColumn)
	{
		return false;
	}

	public virtual void OnQueryCursor(QueryCursorEventArgs e)
	{
	}

	public virtual void OnMouseDown(MouseButtonEventArgs e)
	{
	}

	public virtual void OnMouseUp(MouseButtonEventArgs e)
	{
	}

	public virtual void OnPreviewUp(MouseButtonEventArgs e)
	{
	}

	public virtual void OnMouseHoverStopped(MouseEventArgs e)
	{
	}
}
