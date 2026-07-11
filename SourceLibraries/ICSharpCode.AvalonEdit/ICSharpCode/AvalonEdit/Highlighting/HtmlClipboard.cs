using System;
using System.Globalization;
using System.Text;
using System.Windows;
using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Highlighting;

public static class HtmlClipboard
{
	private static string BuildHeader(int startHTML, int endHTML, int startFragment, int endFragment)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("Version:0.9");
		stringBuilder.AppendLine("StartHTML:" + startHTML.ToString("d8", CultureInfo.InvariantCulture));
		stringBuilder.AppendLine("EndHTML:" + endHTML.ToString("d8", CultureInfo.InvariantCulture));
		stringBuilder.AppendLine("StartFragment:" + startFragment.ToString("d8", CultureInfo.InvariantCulture));
		stringBuilder.AppendLine("EndFragment:" + endFragment.ToString("d8", CultureInfo.InvariantCulture));
		return stringBuilder.ToString();
	}

	public static void SetHtml(DataObject dataObject, string htmlFragment)
	{
		if (dataObject == null)
		{
			throw new ArgumentNullException("dataObject");
		}
		if (htmlFragment == null)
		{
			throw new ArgumentNullException("htmlFragment");
		}
		string text = "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">" + Environment.NewLine + "<HTML>" + Environment.NewLine + "<BODY>" + Environment.NewLine + "<!--StartFragment-->" + Environment.NewLine;
		string text2 = "<!--EndFragment-->" + Environment.NewLine + "</BODY>" + Environment.NewLine + "</HTML>" + Environment.NewLine;
		int length = BuildHeader(0, 0, 0, 0).Length;
		int num = length + text.Length;
		int num2 = num + Encoding.UTF8.GetByteCount(htmlFragment);
		int endHTML = num2 + text2.Length;
		string textData = BuildHeader(length, endHTML, num, num2) + text + htmlFragment + text2;
		dataObject.SetText(textData, TextDataFormat.Html);
	}

	public static string CreateHtmlFragment(IDocument document, IHighlighter highlighter, ISegment segment, HtmlOptions options)
	{
		if (document == null)
		{
			throw new ArgumentNullException("document");
		}
		if (options == null)
		{
			throw new ArgumentNullException("options");
		}
		if (highlighter != null && highlighter.Document != document)
		{
			throw new ArgumentException("Highlighter does not belong to the specified document.");
		}
		if (segment == null)
		{
			segment = new SimpleSegment(0, document.TextLength);
		}
		StringBuilder stringBuilder = new StringBuilder();
		int endOffset = segment.EndOffset;
		IDocumentLine documentLine = document.GetLineByOffset(segment.Offset);
		while (documentLine != null && documentLine.Offset < endOffset)
		{
			HighlightedLine highlightedLine = ((highlighter == null) ? new HighlightedLine(document, documentLine) : highlighter.HighlightLine(documentLine.LineNumber));
			SimpleSegment overlap = SimpleSegment.GetOverlap(segment, documentLine);
			if (stringBuilder.Length > 0)
			{
				stringBuilder.AppendLine("<br>");
			}
			stringBuilder.Append(highlightedLine.ToHtml(overlap.Offset, overlap.EndOffset, options));
			documentLine = documentLine.NextLine;
		}
		return stringBuilder.ToString();
	}
}
