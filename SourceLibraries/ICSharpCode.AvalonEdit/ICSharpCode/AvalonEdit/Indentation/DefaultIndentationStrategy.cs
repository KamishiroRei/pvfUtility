using System;
using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Indentation;

public class DefaultIndentationStrategy : IIndentationStrategy
{
	public virtual void IndentLine(TextDocument document, DocumentLine line)
	{
		if (document == null)
		{
			throw new ArgumentNullException("document");
		}
		if (line == null)
		{
			throw new ArgumentNullException("line");
		}
		DocumentLine previousLine = line.PreviousLine;
		if (previousLine != null)
		{
			ISegment whitespaceAfter = TextUtilities.GetWhitespaceAfter(document, previousLine.Offset);
			string text = document.GetText(whitespaceAfter);
			whitespaceAfter = TextUtilities.GetWhitespaceAfter(document, line.Offset);
			document.Replace(whitespaceAfter.Offset, whitespaceAfter.Length, text, OffsetChangeMappingType.RemoveAndInsert);
		}
	}

	public virtual void IndentLines(TextDocument document, int beginLine, int endLine)
	{
	}
}
