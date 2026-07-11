using System;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Media.TextFormatting;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Rendering;

public class VisualLineText : VisualLineElement
{
	private VisualLine parentVisualLine;

	public VisualLine ParentVisualLine => parentVisualLine;

	public override bool CanSplit => true;

	public VisualLineText(VisualLine parentVisualLine, int length)
		: base(length, length)
	{
		if (parentVisualLine == null)
		{
			throw new ArgumentNullException("parentVisualLine");
		}
		this.parentVisualLine = parentVisualLine;
	}

	protected virtual VisualLineText CreateInstance(int length)
	{
		return new VisualLineText(parentVisualLine, length);
	}

	public override TextRun CreateTextRun(int startVisualColumn, ITextRunConstructionContext context)
	{
		if (context == null)
		{
			throw new ArgumentNullException("context");
		}
		int num = startVisualColumn - base.VisualColumn;
		StringSegment text = context.GetText(context.VisualLine.FirstDocumentLine.Offset + base.RelativeTextOffset + num, base.DocumentLength - num);
		return new TextCharacters(text.Text, text.Offset, text.Count, base.TextRunProperties);
	}

	public override bool IsWhitespace(int visualColumn)
	{
		int offset = visualColumn - base.VisualColumn + parentVisualLine.FirstDocumentLine.Offset + base.RelativeTextOffset;
		return char.IsWhiteSpace(parentVisualLine.Document.GetCharAt(offset));
	}

	public override TextSpan<CultureSpecificCharacterBufferRange> GetPrecedingText(int visualColumnLimit, ITextRunConstructionContext context)
	{
		if (context == null)
		{
			throw new ArgumentNullException("context");
		}
		int length = visualColumnLimit - base.VisualColumn;
		StringSegment text = context.GetText(context.VisualLine.FirstDocumentLine.Offset + base.RelativeTextOffset, length);
		CharacterBufferRange characterBufferRange = new CharacterBufferRange(text.Text, text.Offset, text.Count);
		return new TextSpan<CultureSpecificCharacterBufferRange>(characterBufferRange.Length, new CultureSpecificCharacterBufferRange(base.TextRunProperties.CultureInfo, characterBufferRange));
	}

	public override void Split(int splitVisualColumn, IList<VisualLineElement> elements, int elementIndex)
	{
		if (splitVisualColumn <= base.VisualColumn || splitVisualColumn >= base.VisualColumn + base.VisualLength)
		{
			throw new ArgumentOutOfRangeException("splitVisualColumn", splitVisualColumn, "Value must be between " + (base.VisualColumn + 1) + " and " + (base.VisualColumn + base.VisualLength - 1));
		}
		if (elements == null)
		{
			throw new ArgumentNullException("elements");
		}
		if (elements[elementIndex] != this)
		{
			throw new ArgumentException("Invalid elementIndex - couldn't find this element at the index");
		}
		int num = splitVisualColumn - base.VisualColumn;
		VisualLineText visualLineText = CreateInstance(base.DocumentLength - num);
		SplitHelper(this, visualLineText, splitVisualColumn, num + base.RelativeTextOffset);
		elements.Insert(elementIndex + 1, visualLineText);
	}

	public override int GetRelativeOffset(int visualColumn)
	{
		return base.RelativeTextOffset + visualColumn - base.VisualColumn;
	}

	public override int GetVisualColumn(int relativeTextOffset)
	{
		return base.VisualColumn + relativeTextOffset - base.RelativeTextOffset;
	}

	public override int GetNextCaretPosition(int visualColumn, LogicalDirection direction, CaretPositioningMode mode)
	{
		int num = parentVisualLine.StartOffset + base.RelativeTextOffset;
		int nextCaretPosition = TextUtilities.GetNextCaretPosition(parentVisualLine.Document, num + visualColumn - base.VisualColumn, direction, mode);
		if (nextCaretPosition < num || nextCaretPosition > num + base.DocumentLength)
		{
			return -1;
		}
		return base.VisualColumn + nextCaretPosition - num;
	}

	public virtual TextSegment GetSegment()
	{
		int num = GetRelativeOffset(base.VisualColumn) - base.VisualColumn;
		return new TextSegment
		{
			StartOffset = ParentVisualLine.FirstDocumentLine.Offset + base.RelativeTextOffset + num,
			Length = base.DocumentLength - num
		};
	}
}
