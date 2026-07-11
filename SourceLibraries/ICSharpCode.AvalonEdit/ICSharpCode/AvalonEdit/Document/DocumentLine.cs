using System;
using System.Diagnostics;
using System.Globalization;

namespace ICSharpCode.AvalonEdit.Document;

public sealed class DocumentLine : IDocumentLine, ISegment
{
	internal bool isDeleted;

	private int totalLength;

	private byte delimiterLength;

	internal DocumentLine left;

	internal DocumentLine right;

	internal DocumentLine parent;

	internal bool color;

	internal int nodeTotalCount;

	internal int nodeTotalLength;

	public bool IsDeleted => isDeleted;

	public int LineNumber
	{
		get
		{
			if (IsDeleted)
			{
				throw new InvalidOperationException();
			}
			return DocumentLineTree.GetIndexFromNode(this) + 1;
		}
	}

	public int Offset
	{
		get
		{
			if (IsDeleted)
			{
				throw new InvalidOperationException();
			}
			return DocumentLineTree.GetOffsetFromNode(this);
		}
	}

	public int EndOffset => Offset + Length;

	public int Length => totalLength - delimiterLength;

	public int TotalLength
	{
		get
		{
			return totalLength;
		}
		internal set
		{
			totalLength = value;
		}
	}

	public int DelimiterLength
	{
		get
		{
			return delimiterLength;
		}
		internal set
		{
			delimiterLength = (byte)value;
		}
	}

	public DocumentLine NextLine
	{
		get
		{
			if (right != null)
			{
				return right.LeftMost;
			}
			DocumentLine documentLine = this;
			DocumentLine documentLine2;
			do
			{
				documentLine2 = documentLine;
				documentLine = documentLine.parent;
			}
			while (documentLine != null && documentLine.right == documentLine2);
			return documentLine;
		}
	}

	public DocumentLine PreviousLine
	{
		get
		{
			if (left != null)
			{
				return left.RightMost;
			}
			DocumentLine documentLine = this;
			DocumentLine documentLine2;
			do
			{
				documentLine2 = documentLine;
				documentLine = documentLine.parent;
			}
			while (documentLine != null && documentLine.left == documentLine2);
			return documentLine;
		}
	}

	IDocumentLine IDocumentLine.NextLine => NextLine;

	IDocumentLine IDocumentLine.PreviousLine => PreviousLine;

	internal DocumentLine LeftMost
	{
		get
		{
			DocumentLine documentLine = this;
			while (documentLine.left != null)
			{
				documentLine = documentLine.left;
			}
			return documentLine;
		}
	}

	internal DocumentLine RightMost
	{
		get
		{
			DocumentLine documentLine = this;
			while (documentLine.right != null)
			{
				documentLine = documentLine.right;
			}
			return documentLine;
		}
	}

	internal DocumentLine(TextDocument document)
	{
	}

	[Conditional("DEBUG")]
	private void DebugVerifyAccess()
	{
	}

	public override string ToString()
	{
		if (IsDeleted)
		{
			return "[DocumentLine deleted]";
		}
		return string.Format(CultureInfo.InvariantCulture, "[DocumentLine Number={0} Offset={1} Length={2}]", LineNumber, Offset, Length);
	}

	internal void ResetLine()
	{
		totalLength = (delimiterLength = 0);
		isDeleted = (color = false);
		left = (right = (parent = null));
	}

	internal DocumentLine InitLineNode()
	{
		nodeTotalCount = 1;
		nodeTotalLength = TotalLength;
		return this;
	}
}
