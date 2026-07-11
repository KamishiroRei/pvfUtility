using System;

namespace ICSharpCode.AvalonEdit.Document;

public class TextSegment : ISegment
{
	internal ISegmentTree ownerTree;

	internal TextSegment left;

	internal TextSegment right;

	internal TextSegment parent;

	internal bool color;

	internal int nodeLength;

	internal int totalNodeLength;

	internal int segmentLength;

	internal int distanceToMaxEnd;

	int ISegment.Offset => StartOffset;

	protected bool IsConnectedToCollection => ownerTree != null;

	public int StartOffset
	{
		get
		{
			TextSegment textSegment = this;
			int num = textSegment.nodeLength;
			if (textSegment.left != null)
			{
				num += textSegment.left.totalNodeLength;
			}
			while (textSegment.parent != null)
			{
				if (textSegment == textSegment.parent.right)
				{
					if (textSegment.parent.left != null)
					{
						num += textSegment.parent.left.totalNodeLength;
					}
					num += textSegment.parent.nodeLength;
				}
				textSegment = textSegment.parent;
			}
			return num;
		}
		set
		{
			if (value < 0)
			{
				throw new ArgumentOutOfRangeException("value", "Offset must not be negative");
			}
			if (StartOffset != value)
			{
				ISegmentTree segmentTree = ownerTree;
				if (segmentTree != null)
				{
					segmentTree.Remove(this);
					nodeLength = value;
					segmentTree.Add(this);
				}
				else
				{
					nodeLength = value;
				}
				OnSegmentChanged();
			}
		}
	}

	public int EndOffset
	{
		get
		{
			return StartOffset + Length;
		}
		set
		{
			int num = value - StartOffset;
			if (num < 0)
			{
				throw new ArgumentOutOfRangeException("value", "EndOffset must be greater or equal to StartOffset");
			}
			Length = num;
		}
	}

	public int Length
	{
		get
		{
			return segmentLength;
		}
		set
		{
			if (value < 0)
			{
				throw new ArgumentOutOfRangeException("value", "Length must not be negative");
			}
			if (segmentLength != value)
			{
				segmentLength = value;
				if (ownerTree != null)
				{
					ownerTree.UpdateAugmentedData(this);
				}
				OnSegmentChanged();
			}
		}
	}

	internal TextSegment LeftMost
	{
		get
		{
			TextSegment textSegment = this;
			while (textSegment.left != null)
			{
				textSegment = textSegment.left;
			}
			return textSegment;
		}
	}

	internal TextSegment RightMost
	{
		get
		{
			TextSegment textSegment = this;
			while (textSegment.right != null)
			{
				textSegment = textSegment.right;
			}
			return textSegment;
		}
	}

	internal TextSegment Successor
	{
		get
		{
			if (right != null)
			{
				return right.LeftMost;
			}
			TextSegment textSegment = this;
			TextSegment textSegment2;
			do
			{
				textSegment2 = textSegment;
				textSegment = textSegment.parent;
			}
			while (textSegment != null && textSegment.right == textSegment2);
			return textSegment;
		}
	}

	internal TextSegment Predecessor
	{
		get
		{
			if (left != null)
			{
				return left.RightMost;
			}
			TextSegment textSegment = this;
			TextSegment textSegment2;
			do
			{
				textSegment2 = textSegment;
				textSegment = textSegment.parent;
			}
			while (textSegment != null && textSegment.left == textSegment2);
			return textSegment;
		}
	}

	protected virtual void OnSegmentChanged()
	{
	}

	public override string ToString()
	{
		return "[" + GetType().Name + " Offset=" + StartOffset + " Length=" + Length + " EndOffset=" + EndOffset + "]";
	}
}
