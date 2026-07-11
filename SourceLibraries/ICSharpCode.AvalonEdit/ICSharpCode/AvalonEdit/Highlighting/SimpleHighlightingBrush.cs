using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Rendering;

namespace ICSharpCode.AvalonEdit.Highlighting;

[Serializable]
public sealed class SimpleHighlightingBrush : HighlightingBrush, ISerializable
{
	private readonly SolidColorBrush brush;

	public SimpleHighlightingBrush(SolidColorBrush brush)
	{
		brush.Freeze();
		this.brush = brush;
	}

	public SimpleHighlightingBrush(Color color)
		: this(new SolidColorBrush(color))
	{
	}

	public override Brush GetBrush(ITextRunConstructionContext context)
	{
		return brush;
	}

	public override string ToString()
	{
		return brush.ToString();
	}

	private SimpleHighlightingBrush(SerializationInfo info, StreamingContext context)
	{
		brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(info.GetString("color")));
		brush.Freeze();
	}

	void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("color", brush.Color.ToString(CultureInfo.InvariantCulture));
	}

	public override bool Equals(object obj)
	{
		if (!(obj is SimpleHighlightingBrush simpleHighlightingBrush))
		{
			return false;
		}
		return brush.Color.Equals(simpleHighlightingBrush.brush.Color);
	}

	public override int GetHashCode()
	{
		return brush.Color.GetHashCode();
	}
}
