using System;
using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Indentation.CSharp;

public sealed class TextDocumentAccessor : IDocumentAccessor
{
	private readonly TextDocument doc;

	private readonly int minLine;

	private readonly int maxLine;

	private int num;

	private string text;

	private DocumentLine line;

	private bool lineDirty;

	public bool IsReadOnly => num < minLine;

	public int LineNumber => num;

	public string Text
	{
		get
		{
			return text;
		}
		set
		{
			if (num >= minLine)
			{
				text = value;
				lineDirty = true;
			}
		}
	}

	public TextDocumentAccessor(TextDocument document)
	{
		if (document == null)
		{
			throw new ArgumentNullException("document");
		}
		doc = document;
		minLine = 1;
		maxLine = doc.LineCount;
	}

	public TextDocumentAccessor(TextDocument document, int minLine, int maxLine)
	{
		if (document == null)
		{
			throw new ArgumentNullException("document");
		}
		doc = document;
		this.minLine = minLine;
		this.maxLine = maxLine;
	}

	public bool MoveNext()
	{
		if (lineDirty)
		{
			doc.Replace(line, text);
			lineDirty = false;
		}
		num++;
		if (num > maxLine)
		{
			return false;
		}
		line = doc.GetLineByNumber(num);
		text = doc.GetText(line);
		return true;
	}
}
