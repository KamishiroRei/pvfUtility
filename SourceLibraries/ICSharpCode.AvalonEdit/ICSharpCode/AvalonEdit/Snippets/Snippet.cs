using System;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace ICSharpCode.AvalonEdit.Snippets;

[Serializable]
public class Snippet : SnippetContainerElement
{
	public void Insert(TextArea textArea)
	{
		if (textArea == null)
		{
			throw new ArgumentNullException("textArea");
		}
		ISegment surroundingSegment = textArea.Selection.SurroundingSegment;
		int num = textArea.Caret.Offset;
		if (surroundingSegment != null)
		{
			num = surroundingSegment.Offset + TextUtilities.GetWhitespaceAfter(textArea.Document, surroundingSegment.Offset).Length;
		}
		InsertionContext insertionContext = new InsertionContext(textArea, num);
		using (insertionContext.Document.RunUpdate())
		{
			if (surroundingSegment != null)
			{
				textArea.Document.Remove(num, surroundingSegment.EndOffset - num);
			}
			Insert(insertionContext);
			insertionContext.RaiseInsertionCompleted(EventArgs.Empty);
		}
	}
}
