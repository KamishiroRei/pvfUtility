using System;
using System.Globalization;

namespace ICSharpCode.AvalonEdit.Document;

internal struct SimpleSegment : IEquatable<SimpleSegment>, ISegment
{
	public static readonly SimpleSegment Invalid;

	public readonly int Offset;

	public readonly int Length;

	int ISegment.Offset => Offset;

	int ISegment.Length => Length;

	public int EndOffset => Offset + Length;

	public static SimpleSegment GetOverlap(ISegment segment1, ISegment segment2)
	{
		int num = Math.Max(segment1.Offset, segment2.Offset);
		int num2 = Math.Min(segment1.EndOffset, segment2.EndOffset);
		if (num2 < num)
		{
			return Invalid;
		}
		return new SimpleSegment(num, num2 - num);
	}

	public SimpleSegment(int offset, int length)
	{
		Offset = offset;
		Length = length;
	}

	public SimpleSegment(ISegment segment)
	{
		Offset = segment.Offset;
		Length = segment.Length;
	}

	public override int GetHashCode()
	{
		return Offset + 10301 * Length;
	}

	public override bool Equals(object obj)
	{
		if (obj is SimpleSegment)
		{
			return Equals((SimpleSegment)obj);
		}
		return false;
	}

	public bool Equals(SimpleSegment other)
	{
		if (Offset == other.Offset)
		{
			return Length == other.Length;
		}
		return false;
	}

	public static bool operator ==(SimpleSegment left, SimpleSegment right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(SimpleSegment left, SimpleSegment right)
	{
		return !left.Equals(right);
	}

	public override string ToString()
	{
		return "[Offset=" + Offset.ToString(CultureInfo.InvariantCulture) + ", Length=" + Length.ToString(CultureInfo.InvariantCulture) + "]";
	}

	static SimpleSegment()
	{
		Invalid = new SimpleSegment(-1, -1);
	}
}
