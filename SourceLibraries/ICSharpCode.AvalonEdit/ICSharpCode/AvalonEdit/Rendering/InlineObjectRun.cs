using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

namespace ICSharpCode.AvalonEdit.Rendering;

public class InlineObjectRun : TextEmbeddedObject
{
	private UIElement element;

	private int length;

	private TextRunProperties properties;

	internal Size desiredSize;

	public UIElement Element => element;

	public VisualLine VisualLine { get; internal set; }

	public override LineBreakCondition BreakBefore => LineBreakCondition.BreakDesired;

	public override LineBreakCondition BreakAfter => LineBreakCondition.BreakDesired;

	public override bool HasFixedSize => true;

	public override CharacterBufferReference CharacterBufferReference => default(CharacterBufferReference);

	public override int Length => length;

	public override TextRunProperties Properties => properties;

	public InlineObjectRun(int length, TextRunProperties properties, UIElement element)
	{
		if (length <= 0)
		{
			throw new ArgumentOutOfRangeException("length", length, "Value must be positive");
		}
		if (properties == null)
		{
			throw new ArgumentNullException("properties");
		}
		if (element == null)
		{
			throw new ArgumentNullException("element");
		}
		this.length = length;
		this.properties = properties;
		this.element = element;
	}

	public override TextEmbeddedObjectMetrics Format(double remainingParagraphWidth)
	{
		double num = TextBlock.GetBaselineOffset(element);
		if (double.IsNaN(num))
		{
			num = desiredSize.Height;
		}
		return new TextEmbeddedObjectMetrics(desiredSize.Width, desiredSize.Height, num);
	}

	public override Rect ComputeBoundingBox(bool rightToLeft, bool sideways)
	{
		if (element.IsArrangeValid)
		{
			double num = TextBlock.GetBaselineOffset(element);
			if (double.IsNaN(num))
			{
				num = desiredSize.Height;
			}
			return new Rect(new Point(0.0, 0.0 - num), desiredSize);
		}
		return Rect.Empty;
	}

	public override void Draw(DrawingContext drawingContext, Point origin, bool rightToLeft, bool sideways)
	{
	}
}
