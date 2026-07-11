using System;
using System.ComponentModel;
using System.Globalization;

namespace ICSharpCode.AvalonEdit.Document;

[Serializable]
[TypeConverter(typeof(TextLocationConverter))]
public struct TextLocation : IComparable<TextLocation>, IEquatable<TextLocation>
{
	public static readonly TextLocation Empty = new TextLocation(0, 0);

	private readonly int column;

	private readonly int line;

	public int Line => line;

	public int Column => column;

	public bool IsEmpty
	{
		get
		{
			if (column <= 0)
			{
				return line <= 0;
			}
			return false;
		}
	}

	public TextLocation(int line, int column)
	{
		this.line = line;
		this.column = column;
	}

	public override string ToString()
	{
		return string.Format(CultureInfo.InvariantCulture, "(Line {1}, Col {0})", column, line);
	}

	public override int GetHashCode()
	{
		return (191 * column.GetHashCode()) ^ line.GetHashCode();
	}

	public override bool Equals(object obj)
	{
		if (!(obj is TextLocation))
		{
			return false;
		}
		return (TextLocation)obj == this;
	}

	public bool Equals(TextLocation other)
	{
		return this == other;
	}

	public static bool operator ==(TextLocation left, TextLocation right)
	{
		if (left.column == right.column)
		{
			return left.line == right.line;
		}
		return false;
	}

	public static bool operator !=(TextLocation left, TextLocation right)
	{
		if (left.column == right.column)
		{
			return left.line != right.line;
		}
		return true;
	}

	public static bool operator <(TextLocation left, TextLocation right)
	{
		if (left.line < right.line)
		{
			return true;
		}
		if (left.line == right.line)
		{
			return left.column < right.column;
		}
		return false;
	}

	public static bool operator >(TextLocation left, TextLocation right)
	{
		if (left.line > right.line)
		{
			return true;
		}
		if (left.line == right.line)
		{
			return left.column > right.column;
		}
		return false;
	}

	public static bool operator <=(TextLocation left, TextLocation right)
	{
		return !(left > right);
	}

	public static bool operator >=(TextLocation left, TextLocation right)
	{
		return !(left < right);
	}

	public int CompareTo(TextLocation other)
	{
		if (this == other)
		{
			return 0;
		}
		if (this < other)
		{
			return -1;
		}
		return 1;
	}
}
