using System;
using System.IO;
using System.Text;

namespace ICSharpCode.AvalonEdit.Document;

public class DocumentTextWriter : TextWriter
{
	private readonly IDocument document;

	private int insertionOffset;

	public int InsertionOffset
	{
		get
		{
			return insertionOffset;
		}
		set
		{
			insertionOffset = value;
		}
	}

	public override Encoding Encoding => Encoding.UTF8;

	public DocumentTextWriter(IDocument document, int insertionOffset)
	{
		this.insertionOffset = insertionOffset;
		if (document == null)
		{
			throw new ArgumentNullException("document");
		}
		this.document = document;
		IDocumentLine documentLine = document.GetLineByOffset(insertionOffset);
		if (documentLine.DelimiterLength == 0)
		{
			documentLine = documentLine.PreviousLine;
		}
		if (documentLine != null)
		{
			NewLine = document.GetText(documentLine.EndOffset, documentLine.DelimiterLength);
		}
	}

	public override void Write(char value)
	{
		document.Insert(insertionOffset, value.ToString());
		insertionOffset++;
	}

	public override void Write(char[] buffer, int index, int count)
	{
		document.Insert(insertionOffset, new string(buffer, index, count));
		insertionOffset += count;
	}

	public override void Write(string value)
	{
		document.Insert(insertionOffset, value);
		insertionOffset += value.Length;
	}
}
