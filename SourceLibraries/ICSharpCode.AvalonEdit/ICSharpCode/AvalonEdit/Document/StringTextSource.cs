using System;
using System.IO;

namespace ICSharpCode.AvalonEdit.Document;

[Serializable]
public class StringTextSource : ITextSource
{
	public static readonly StringTextSource Empty = new StringTextSource(string.Empty);

	private readonly string text;

	private readonly ITextSourceVersion version;

	public ITextSourceVersion Version => version;

	public int TextLength => text.Length;

	public string Text => text;

	public StringTextSource(string text)
	{
		if (text == null)
		{
			throw new ArgumentNullException("text");
		}
		this.text = text;
	}

	public StringTextSource(string text, ITextSourceVersion version)
	{
		if (text == null)
		{
			throw new ArgumentNullException("text");
		}
		this.text = text;
		this.version = version;
	}

	public ITextSource CreateSnapshot()
	{
		return this;
	}

	public ITextSource CreateSnapshot(int offset, int length)
	{
		return new StringTextSource(text.Substring(offset, length));
	}

	public TextReader CreateReader()
	{
		return new StringReader(text);
	}

	public TextReader CreateReader(int offset, int length)
	{
		return new StringReader(text.Substring(offset, length));
	}

	public void WriteTextTo(TextWriter writer)
	{
		writer.Write(text);
	}

	public void WriteTextTo(TextWriter writer, int offset, int length)
	{
		writer.Write(text.Substring(offset, length));
	}

	public char GetCharAt(int offset)
	{
		return text[offset];
	}

	public string GetText(int offset, int length)
	{
		return text.Substring(offset, length);
	}

	public string GetText(ISegment segment)
	{
		if (segment == null)
		{
			throw new ArgumentNullException("segment");
		}
		return text.Substring(segment.Offset, segment.Length);
	}

	public int IndexOf(char c, int startIndex, int count)
	{
		return text.IndexOf(c, startIndex, count);
	}

	public int IndexOfAny(char[] anyOf, int startIndex, int count)
	{
		return text.IndexOfAny(anyOf, startIndex, count);
	}

	public int IndexOf(string searchText, int startIndex, int count, StringComparison comparisonType)
	{
		return text.IndexOf(searchText, startIndex, count, comparisonType);
	}

	public int LastIndexOf(char c, int startIndex, int count)
	{
		return text.LastIndexOf(c, startIndex + count - 1, count);
	}

	public int LastIndexOf(string searchText, int startIndex, int count, StringComparison comparisonType)
	{
		return text.LastIndexOf(searchText, startIndex + count - 1, count, comparisonType);
	}
}
