using System;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Document;

[Serializable]
public struct OffsetChangeMapEntry : IEquatable<OffsetChangeMapEntry>
{
	private readonly int offset;

	private readonly uint insertionLengthWithMovementFlag;

	private readonly uint removalLengthWithDeletionFlag;

	public int Offset => offset;

	public int InsertionLength => (int)(insertionLengthWithMovementFlag & 0x7FFFFFFF);

	public int RemovalLength => (int)(removalLengthWithDeletionFlag & 0x7FFFFFFF);

	public bool RemovalNeverCausesAnchorDeletion => (removalLengthWithDeletionFlag & 0x80000000u) != 0;

	public bool DefaultAnchorMovementIsBeforeInsertion => (insertionLengthWithMovementFlag & 0x80000000u) != 0;

	public int GetNewOffset(int oldOffset, AnchorMovementType movementType = AnchorMovementType.Default)
	{
		int insertionLength = InsertionLength;
		int removalLength = RemovalLength;
		if (removalLength != 0 || oldOffset != offset)
		{
			if (oldOffset <= offset)
			{
				return oldOffset;
			}
			if (oldOffset >= offset + removalLength)
			{
				return oldOffset + insertionLength - removalLength;
			}
		}
		switch (movementType)
		{
		case AnchorMovementType.AfterInsertion:
			return offset + insertionLength;
		case AnchorMovementType.BeforeInsertion:
			return offset;
		default:
			if (!DefaultAnchorMovementIsBeforeInsertion)
			{
				return offset + insertionLength;
			}
			return offset;
		}
	}

	public OffsetChangeMapEntry(int offset, int removalLength, int insertionLength)
	{
		ThrowUtil.CheckNotNegative(offset, "offset");
		ThrowUtil.CheckNotNegative(removalLength, "removalLength");
		ThrowUtil.CheckNotNegative(insertionLength, "insertionLength");
		this.offset = offset;
		removalLengthWithDeletionFlag = (uint)removalLength;
		insertionLengthWithMovementFlag = (uint)insertionLength;
	}

	public OffsetChangeMapEntry(int offset, int removalLength, int insertionLength, bool removalNeverCausesAnchorDeletion, bool defaultAnchorMovementIsBeforeInsertion)
		: this(offset, removalLength, insertionLength)
	{
		if (removalNeverCausesAnchorDeletion)
		{
			removalLengthWithDeletionFlag |= 2147483648u;
		}
		if (defaultAnchorMovementIsBeforeInsertion)
		{
			insertionLengthWithMovementFlag |= 2147483648u;
		}
	}

	public override int GetHashCode()
	{
		return offset + (int)(3559 * insertionLengthWithMovementFlag) + (int)(3571 * removalLengthWithDeletionFlag);
	}

	public override bool Equals(object obj)
	{
		if (obj is OffsetChangeMapEntry)
		{
			return Equals((OffsetChangeMapEntry)obj);
		}
		return false;
	}

	public bool Equals(OffsetChangeMapEntry other)
	{
		if (offset == other.offset && insertionLengthWithMovementFlag == other.insertionLengthWithMovementFlag)
		{
			return removalLengthWithDeletionFlag == other.removalLengthWithDeletionFlag;
		}
		return false;
	}

	public static bool operator ==(OffsetChangeMapEntry left, OffsetChangeMapEntry right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(OffsetChangeMapEntry left, OffsetChangeMapEntry right)
	{
		return !left.Equals(right);
	}
}
