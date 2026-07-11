using System;
using System.IO;
using System.Text;

namespace ICSharpCode.AvalonEdit.Utils;

internal class PlainRichTextWriter : RichTextWriter
{
	protected readonly TextWriter textWriter;

	private string indentationString = "\t";

	private int indentationLevel;

	private char prevChar;

	public string IndentationString
	{
		get
		{
			return indentationString;
		}
		set
		{
			indentationString = value;
		}
	}

	public override Encoding Encoding => textWriter.Encoding;

	public override IFormatProvider FormatProvider => textWriter.FormatProvider;

	public override string NewLine
	{
		get
		{
			return textWriter.NewLine;
		}
		set
		{
			textWriter.NewLine = value;
		}
	}

	public PlainRichTextWriter(TextWriter textWriter)
	{
		if (textWriter == null)
		{
			throw new ArgumentNullException("textWriter");
		}
		this.textWriter = textWriter;
	}

	protected override void BeginUnhandledSpan()
	{
	}

	public override void EndSpan()
	{
	}

	private void WriteIndentation()
	{
		for (int i = 0; i < indentationLevel; i++)
		{
			textWriter.Write(indentationString);
		}
	}

	protected void WriteIndentationIfNecessary()
	{
		if (prevChar == '\n')
		{
			WriteIndentation();
			prevChar = '\0';
		}
	}

	protected virtual void AfterWrite()
	{
	}

	public override void Write(char value)
	{
		if (prevChar == '\n')
		{
			WriteIndentation();
		}
		textWriter.Write(value);
		prevChar = value;
		AfterWrite();
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
}
