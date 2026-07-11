using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;

namespace ICSharpCode.AvalonEdit.Utils;

public static class DocumentPrinter
{
	public static Block ConvertTextDocumentToBlock(IDocument document, IHighlighter highlighter)
	{
		if (document == null)
		{
			throw new ArgumentNullException("document");
		}
		Paragraph paragraph = new Paragraph();
		paragraph.TextAlignment = TextAlignment.Left;
		for (int i = 1; i <= document.LineCount; i++)
		{
			if (i > 1)
			{
				paragraph.Inlines.Add(new LineBreak());
			}
			IDocumentLine lineByNumber = document.GetLineByNumber(i);
			if (highlighter != null)
			{
				HighlightedLine highlightedLine = highlighter.HighlightLine(i);
				paragraph.Inlines.AddRange(highlightedLine.ToRichText().CreateRuns());
			}
			else
			{
				paragraph.Inlines.Add(document.GetText(lineByNumber));
			}
		}
		return paragraph;
	}

	public static RichText ConvertTextDocumentToRichText(IDocument document, IHighlighter highlighter)
	{
		if (document == null)
		{
			throw new ArgumentNullException("document");
		}
		List<RichText> list = new List<RichText>();
		for (int i = 1; i <= document.LineCount; i++)
		{
			IDocumentLine lineByNumber = document.GetLineByNumber(i);
			if (i > 1)
			{
				list.Add((lineByNumber.PreviousLine.DelimiterLength == 2) ? "\r\n" : "\n");
			}
			if (highlighter != null)
			{
				HighlightedLine highlightedLine = highlighter.HighlightLine(i);
				list.Add(highlightedLine.ToRichText());
			}
			else
			{
				list.Add(document.GetText(lineByNumber));
			}
		}
		return RichText.Concat(list.ToArray());
	}

	public static FlowDocument CreateFlowDocumentForEditor(TextEditor editor)
	{
		IHighlighter highlighter = editor.TextArea.GetService(typeof(IHighlighter)) as IHighlighter;
		return new FlowDocument(ConvertTextDocumentToBlock(editor.Document, highlighter))
		{
			FontFamily = editor.FontFamily,
			FontSize = editor.FontSize
		};
	}
}
