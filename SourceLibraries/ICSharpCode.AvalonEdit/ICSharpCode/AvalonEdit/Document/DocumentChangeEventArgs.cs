using System;

namespace ICSharpCode.AvalonEdit.Document;

[Serializable]
public class DocumentChangeEventArgs : TextChangeEventArgs
{
	private volatile OffsetChangeMap offsetChangeMap;

	public OffsetChangeMap OffsetChangeMap
	{
		get
		{
			OffsetChangeMap offsetChangeMap = this.offsetChangeMap;
			if (offsetChangeMap == null)
			{
				offsetChangeMap = (this.offsetChangeMap = OffsetChangeMap.FromSingleElement(CreateSingleChangeMapEntry()));
			}
			return offsetChangeMap;
		}
	}

	internal OffsetChangeMap OffsetChangeMapOrNull => offsetChangeMap;

	internal OffsetChangeMapEntry CreateSingleChangeMapEntry()
	{
		return new OffsetChangeMapEntry(base.Offset, base.RemovalLength, base.InsertionLength);
	}

	public override int GetNewOffset(int offset, AnchorMovementType movementType = AnchorMovementType.Default)
	{
		if (offsetChangeMap != null)
		{
			return offsetChangeMap.GetNewOffset(offset, movementType);
		}
		return CreateSingleChangeMapEntry().GetNewOffset(offset, movementType);
	}

	public DocumentChangeEventArgs(int offset, string removedText, string insertedText)
		: this(offset, removedText, insertedText, null)
	{
	}

	public DocumentChangeEventArgs(int offset, string removedText, string insertedText, OffsetChangeMap offsetChangeMap)
		: base(offset, removedText, insertedText)
	{
		SetOffsetChangeMap(offsetChangeMap);
	}

	public DocumentChangeEventArgs(int offset, ITextSource removedText, ITextSource insertedText, OffsetChangeMap offsetChangeMap)
		: base(offset, removedText, insertedText)
	{
		SetOffsetChangeMap(offsetChangeMap);
	}

	private void SetOffsetChangeMap(OffsetChangeMap offsetChangeMap)
	{
		if (offsetChangeMap != null)
		{
			if (!offsetChangeMap.IsFrozen)
			{
				throw new ArgumentException("The OffsetChangeMap must be frozen before it can be used in DocumentChangeEventArgs");
			}
			if (!offsetChangeMap.IsValidForDocumentChange(base.Offset, base.RemovalLength, base.InsertionLength))
			{
				throw new ArgumentException("OffsetChangeMap is not valid for this document change", "offsetChangeMap");
			}
			this.offsetChangeMap = offsetChangeMap;
		}
	}

	public override TextChangeEventArgs Invert()
	{
		OffsetChangeMap offsetChangeMap = OffsetChangeMapOrNull;
		if (offsetChangeMap != null)
		{
			offsetChangeMap = offsetChangeMap.Invert();
			offsetChangeMap.Freeze();
		}
		return new DocumentChangeEventArgs(base.Offset, base.InsertedText, base.RemovedText, offsetChangeMap);
	}
}
