using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Highlighting;

internal class HtmlRichTextWriter : RichTextWriter
{
	private readonly TextWriter htmlWriter;

	private readonly HtmlOptions options;

	private Stack<string> endTagStack = new Stack<string>();

	private bool spaceNeedsEscaping = true;

	private bool hasSpace;

	private bool needIndentation = true;

	private int indentationLevel;

	private static readonly char[] specialChars = new char[4] { ' ', '\t', '\r', '\n' };

	public override Encoding Encoding => htmlWriter.Encoding;

	public HtmlRichTextWriter(TextWriter htmlWriter, HtmlOptions options = null)
	{
		if (htmlWriter == null)
		{
			throw new ArgumentNullException("htmlWriter");
		}
		this.htmlWriter = htmlWriter;
		this.options = options ?? new HtmlOptions();
	}

	public override void Flush()
	{
		FlushSpace(nextIsWhitespace: true);
		htmlWriter.Flush();
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			FlushSpace(nextIsWhitespace: true);
		}
		base.Dispose(disposing);
	}

	private void FlushSpace(bool nextIsWhitespace)
	{
		if (hasSpace)
		{
			if (spaceNeedsEscaping || nextIsWhitespace)
			{
				htmlWriter.Write("&nbsp;");
			}
			else
			{
				htmlWriter.Write(' ');
			}
			hasSpace = false;
			spaceNeedsEscaping = true;
		}
	}

	private void WriteIndentation()
	{
		if (needIndentation)
		{
			for (int i = 0; i < indentationLevel; i++)
			{
				WriteChar('\t');
			}
			needIndentation = false;
		}
	}

	public override void Write(char value)
	{
		WriteIndentation();
		WriteChar(value);
	}

	private void WriteChar(char c)
	{
		bool nextIsWhitespace = char.IsWhiteSpace(c);
		FlushSpace(nextIsWhitespace);
		switch (c)
		{
		case ' ':
			if (spaceNeedsEscaping)
			{
				htmlWriter.Write("&nbsp;");
			}
			else
			{
				hasSpace = true;
			}
			break;
		case '\t':
		{
			for (int i = 0; i < options.TabSize; i++)
			{
				htmlWriter.Write("&nbsp;");
			}
			break;
		}
		case '\n':
			htmlWriter.Write("<br/>");
			needIndentation = true;
			break;
		default:
			WebUtility.HtmlEncode(c.ToString(), htmlWriter);
			break;
		case '\r':
			break;
		}
		if (c != ' ')
		{
			spaceNeedsEscaping = nextIsWhitespace;
		}
	}

	public override void Write(string value)
	{
		int num = 0;
		do
		{
			int num2 = value.IndexOfAny(specialChars, num);
			if (num2 < 0)
			{
				WriteSimpleString(value.Substring(num));
				break;
			}
			if (num2 > num)
			{
				WriteSimpleString(value.Substring(num, num2 - num));
			}
			WriteChar(value[num2]);
			num = num2 + 1;
		}
		while (num < value.Length);
	}

	private void WriteIndentationAndSpace()
	{
		WriteIndentation();
		FlushSpace(nextIsWhitespace: false);
	}

	private void WriteSimpleString(string value)
	{
		if (value.Length != 0)
		{
			WriteIndentationAndSpace();
			WebUtility.HtmlEncode(value, htmlWriter);
		}
	}

	public override void Indent()
	{
		indentationLevel++;
	}

	public override void Unindent()
	{
		if (indentationLevel == 0)
		{
			throw new NotSupportedException();
		}
		indentationLevel--;
	}

	protected override void BeginUnhandledSpan()
	{
		endTagStack.Push(null);
	}

	public override void EndSpan()
	{
		htmlWriter.Write(endTagStack.Pop());
	}

	public override void BeginSpan(Color foregroundColor)
	{
		BeginSpan(new HighlightingColor
		{
			Foreground = new SimpleHighlightingBrush(foregroundColor)
		});
	}

	public override void BeginSpan(FontFamily fontFamily)
	{
		BeginUnhandledSpan();
	}

	public override void BeginSpan(FontStyle fontStyle)
	{
		BeginSpan(new HighlightingColor
		{
			FontStyle = fontStyle
		});
	}

	public override void BeginSpan(FontWeight fontWeight)
	{
		BeginSpan(new HighlightingColor
		{
			FontWeight = fontWeight
		});
	}

	public override void BeginSpan(HighlightingColor highlightingColor)
	{
		WriteIndentationAndSpace();
		if (options.ColorNeedsSpanForStyling(highlightingColor))
		{
			htmlWriter.Write("<span");
			options.WriteStyleAttributeForColor(htmlWriter, highlightingColor);
			htmlWriter.Write('>');
			endTagStack.Push("</span>");
		}
		else
		{
			endTagStack.Push(null);
		}
	}

	public override void BeginHyperlinkSpan(Uri uri)
	{
		WriteIndentationAndSpace();
		string text = WebUtility.HtmlEncode(uri.ToString());
		htmlWriter.Write("<a href=\"" + text + "\">");
		endTagStack.Push("</a>");
	}
}
