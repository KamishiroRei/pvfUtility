using System;
using System.IO;
using System.Net;

namespace ICSharpCode.AvalonEdit.Highlighting;

public class HtmlOptions
{
	public int TabSize { get; set; }

	public HtmlOptions()
	{
		TabSize = 4;
	}

	public HtmlOptions(TextEditorOptions options)
		: this()
	{
		if (options == null)
		{
			throw new ArgumentNullException("options");
		}
		TabSize = options.IndentationSize;
	}

	public virtual void WriteStyleAttributeForColor(TextWriter writer, HighlightingColor color)
	{
		if (writer == null)
		{
			throw new ArgumentNullException("writer");
		}
		if (color == null)
		{
			throw new ArgumentNullException("color");
		}
		writer.Write(" style=\"");
		WebUtility.HtmlEncode(color.ToCss(), writer);
		writer.Write('"');
	}

	public virtual bool ColorNeedsSpanForStyling(HighlightingColor color)
	{
		if (color == null)
		{
			throw new ArgumentNullException("color");
		}
		return !string.IsNullOrEmpty(color.ToCss());
	}
}
