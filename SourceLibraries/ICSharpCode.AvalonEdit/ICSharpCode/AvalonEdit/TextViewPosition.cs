using System;
using System.Globalization;
using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit;

public struct TextViewPosition : IEquatable<TextViewPosition>, IComparable<TextViewPosition>
{
	private int line;

	private int column;

	private int visualColumn;

	private bool isAtEndOfLine;

	public TextLocation Location
	{
		get
		{
			return new TextLocation(line, column);
		}
		set
		{
			line = value.Line;
			column = value.Column;
		}
	}

	public int Line
	{
		get
		{
			return line;
		}
		set
		{
			line = value;
		}
	}

	public int Column
	{
		get
		{
			return column;
		}
		set
		{
			column = value;
		}
	}

	public int VisualColumn
	{
		get
		{
			return visualColumn;
		}
		set
		{
			visualColumn = value;
		}
	}

	public bool IsAtEndOfLine
	{
		get
		{
			return isAtEndOfLine;
		}
		set
		{
			isAtEndOfLine = value;
		}
	}

	public TextViewPosition(int line, int column, int visualColumn)
	{
		this.line = line;
		this.column = column;
		this.visualColumn = visualColumn;
		isAtEndOfLine = false;
	}

	public TextViewPosition(int line, int column)
	{
		this = new TextViewPosition(line, column, -1);
	}

	public TextViewPosition(TextLocation location, int visualColumn)
	{
		line = location.Line;
		column = location.Column;
		this.visualColumn = visualColumn;
		isAtEndOfLine = false;
	}

	public TextViewPosition(TextLocation location)
	{
		this = new TextViewPosition(location, -1);
	}

	public override string ToString()
	{
		return string.Format(CultureInfo.InvariantCulture, "[TextViewPosition Line={0} Column={1} VisualColumn={2} IsAtEndOfLine={3}]", line, column, visualColumn, isAtEndOfLine);
	}

	public override bool Equals(object obj)
	{
		if (obj is TextViewPosition)
		{
			return Equals((TextViewPosition)obj);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (isAtEndOfLine ? 115817 : 0) + 1000000007 * Line.GetHashCode() + 1000000009 * Column.GetHashCode() + 1000000021 * VisualColumn.GetHashCode();
	}

	public bool Equals(TextViewPosition other)
	{
		if (Line == other.Line && Column == other.Column && VisualColumn == other.VisualColumn)
		{
			return IsAtEndOfLine == other.IsAtEndOfLine;
		}
		return false;
	}

	public static bool operator ==(TextViewPosition left, TextViewPosition right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(TextViewPosition left, TextViewPosition right)
	{
		return !left.Equals(right);
	}

	public int CompareTo(TextViewPosition other)
	{
		int num = Location.CompareTo(other.Location);
		if (num != 0)
		{
			return num;
		}
		num = visualColumn.CompareTo(other.visualColumn);
		if (num != 0)
		{
			return num;
		}
		if (isAtEndOfLine && !other.isAtEndOfLine)
		{
			return -1;
		}
		if (!isAtEndOfLine && other.isAtEndOfLine)
		{
			return 1;
		}
		return 0;
	}
}
