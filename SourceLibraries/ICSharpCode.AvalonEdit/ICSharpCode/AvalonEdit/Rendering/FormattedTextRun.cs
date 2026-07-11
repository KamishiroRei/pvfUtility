using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

namespace ICSharpCode.AvalonEdit.Rendering;

public class FormattedTextRun : TextEmbeddedObject
{
	private readonly FormattedTextElement element;

	private TextRunProperties properties;

	public FormattedTextElement Element => element;

	public override LineBreakCondition BreakBefore => element.BreakBefore;

	public override LineBreakCondition BreakAfter => element.BreakAfter;

	public override bool HasFixedSize => true;

	public override CharacterBufferReference CharacterBufferReference => default(CharacterBufferReference);

	public override int Length => element.VisualLength;

	public override TextRunProperties Properties => properties;

	public FormattedTextRun(FormattedTextElement element, TextRunProperties properties)
	{
		if (element == null)
		{
			throw new ArgumentNullException("element");
		}
		if (properties == null)
		{
			throw new ArgumentNullException("properties");
		}
		this.properties = properties;
		this.element = element;
	}

	public override TextEmbeddedObjectMetrics Format(double remainingParagraphWidth)
	{
		FormattedText formattedText = element.formattedText;
		if (formattedText != null)
		{
			return new TextEmbeddedObjectMetrics(formattedText.WidthIncludingTrailingWhitespace, formattedText.Height, formattedText.Baseline);
		}
		TextLine textLine = element.textLine;
		return new TextEmbeddedObjectMetrics(textLine.WidthIncludingTrailingWhitespace, textLine.Height, textLine.Baseline);
	}

	public override Rect ComputeBoundingBox(bool rightToLeft, bool sideways)
	{
		FormattedText formattedText = element.formattedText;
		if (formattedText != null)
		{
			return new Rect(0.0, 0.0, formattedText.WidthIncludingTrailingWhitespace, formattedText.Height);
		}
		TextLine textLine = element.textLine;
		return new Rect(0.0, 0.0, textLine.WidthIncludingTrailingWhitespace, textLine.Height);
	}

	public override void Draw(DrawingContext drawingContext, Point origin, bool rightToLeft, bool sideways)
	{
		if (element.formattedText != null)
		{
			origin.Y -= element.formattedText.Baseline;
			drawingContext.DrawText(element.formattedText, origin);
		}
		else
		{
			origin.Y -= element.textLine.Baseline;
			element.textLine.Draw(drawingContext, origin, InvertAxes.None);
		}
	}
}
