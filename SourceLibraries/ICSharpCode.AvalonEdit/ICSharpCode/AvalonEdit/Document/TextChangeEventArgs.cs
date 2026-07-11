using System;

namespace ICSharpCode.AvalonEdit.Document;

[Serializable]
public class TextChangeEventArgs : EventArgs
{
	private readonly int offset;

	private readonly ITextSource removedText;

	private readonly ITextSource insertedText;

	public int Offset => offset;

	public ITextSource RemovedText => removedText;

	public int RemovalLength => removedText.TextLength;

	public ITextSource InsertedText => insertedText;

	public int InsertionLength => insertedText.TextLength;

	public TextChangeEventArgs(int offset, string removedText, string insertedText)
	{
		if (offset < 0)
		{
			throw new ArgumentOutOfRangeException("offset", offset, "offset must not be negative");
		}
		this.offset = offset;
		this.removedText = ((removedText != null) ? new StringTextSource(removedText) : StringTextSource.Empty);
		this.insertedText = ((insertedText != null) ? new StringTextSource(insertedText) : StringTextSource.Empty);
	}

	public TextChangeEventArgs(int offset, ITextSource removedText, ITextSource insertedText)
	{
		if (offset < 0)
		{
			throw new ArgumentOutOfRangeException("offset", offset, "offset must not be negative");
		}
		this.offset = offset;
		this.removedText = removedText ?? StringTextSource.Empty;
		this.insertedText = insertedText ?? StringTextSource.Empty;
	}

	public virtual int GetNewOffset(int offset, AnchorMovementType movementType = AnchorMovementType.Default)
	{
		if (offset >= Offset && offset <= Offset + RemovalLength)
		{
			if (movementType == AnchorMovementType.BeforeInsertion)
			{
				return Offset;
			}
			return Offset + InsertionLength;
		}
		if (offset > Offset)
		{
			return offset + InsertionLength - RemovalLength;
		}
		return offset;
	}

	public virtual TextChangeEventArgs Invert()
	{
		return new TextChangeEventArgs(offset, insertedText, removedText);
	}
}
