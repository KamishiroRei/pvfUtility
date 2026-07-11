using System;

namespace ICSharpCode.AvalonEdit.Utils;

public struct StringSegment : IEquatable<StringSegment>
{
	private readonly string text;

	private readonly int offset;

	private readonly int count;

	public string Text => text;

	public int Offset => offset;

	public int Count => count;

	public StringSegment(string text, int offset, int count)
	{
		if (text == null)
		{
			throw new ArgumentNullException("text");
		}
		if (offset < 0 || offset > text.Length)
		{
			throw new ArgumentOutOfRangeException("offset");
		}
		if (offset + count > text.Length)
		{
			throw new ArgumentOutOfRangeException("count");
		}
		this.text = text;
		this.offset = offset;
		this.count = count;
	}

	public StringSegment(string text)
	{
		if (text == null)
		{
			throw new ArgumentNullException("text");
		}
		this.text = text;
		offset = 0;
		count = text.Length;
	}

	public override bool Equals(object obj)
	{
		if (obj is StringSegment)
		{
			return Equals((StringSegment)obj);
		}
		return false;
	}

	public bool Equals(StringSegment other)
	{
		if ((object)text == other.text && offset == other.offset)
		{
			return count == other.count;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return text.GetHashCode() ^ offset ^ count;
	}

	public static bool operator ==(StringSegment left, StringSegment right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(StringSegment left, StringSegment right)
	{
		return !left.Equals(right);
	}
}
