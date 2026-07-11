using System;
using System.Windows;
using System.Windows.Media;

namespace ICSharpCode.AvalonEdit.Rendering;

internal sealed class CurrentLineHighlightRenderer : IBackgroundRenderer
{
	private int line;

	private TextView textView;

	public static readonly Color DefaultBackground = Color.FromArgb(22, 20, 220, 224);

	public static readonly Color DefaultBorder = Color.FromArgb(52, 0, byte.MaxValue, 110);

	public int Line
	{
		get
		{
			return line;
		}
		set
		{
			if (line != value)
			{
				line = value;
				textView.InvalidateLayer(Layer);
			}
		}
	}

	public KnownLayer Layer => KnownLayer.Selection;

	public Brush BackgroundBrush { get; set; }

	public Pen BorderPen { get; set; }

	public CurrentLineHighlightRenderer(TextView textView)
	{
		if (textView == null)
		{
			throw new ArgumentNullException("textView");
		}
		BorderPen = new Pen(new SolidColorBrush(DefaultBorder), 1.0);
		BorderPen.Freeze();
		BackgroundBrush = new SolidColorBrush(DefaultBackground);
		BackgroundBrush.Freeze();
		this.textView = textView;
		this.textView.BackgroundRenderers.Add(this);
		line = 0;
	}

	public void Draw(TextView textView, DrawingContext drawingContext)
	{
		if (!this.textView.Options.HighlightCurrentLine)
		{
			return;
		}
		BackgroundGeometryBuilder backgroundGeometryBuilder = new BackgroundGeometryBuilder();
		VisualLine visualLine = this.textView.GetVisualLine(line);
		if (visualLine != null)
		{
			double y = visualLine.VisualTop - this.textView.ScrollOffset.Y;
			backgroundGeometryBuilder.AddRectangle(textView, new Rect(0.0, y, textView.ActualWidth, visualLine.Height));
			Geometry geometry = backgroundGeometryBuilder.CreateGeometry();
			if (geometry != null)
			{
				drawingContext.DrawGeometry(BackgroundBrush, BorderPen, geometry);
			}
		}
	}
}
