using System;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Rendering;

internal sealed class ColumnRulerRenderer : IBackgroundRenderer
{
	private Pen pen;

	private int column;

	private TextView textView;

	public static readonly Color DefaultForeground = Colors.LightGray;

	public KnownLayer Layer => KnownLayer.Background;

	public ColumnRulerRenderer(TextView textView)
	{
		if (textView == null)
		{
			throw new ArgumentNullException("textView");
		}
		pen = new Pen(new SolidColorBrush(DefaultForeground), 1.0);
		pen.Freeze();
		this.textView = textView;
		this.textView.BackgroundRenderers.Add(this);
	}

	public void SetRuler(int column, Pen pen)
	{
		if (this.column != column)
		{
			this.column = column;
			textView.InvalidateLayer(Layer);
		}
		if (this.pen != pen)
		{
			this.pen = pen;
			textView.InvalidateLayer(Layer);
		}
	}

	public void Draw(TextView textView, DrawingContext drawingContext)
	{
		if (column >= 1)
		{
			double num = PixelSnapHelpers.PixelAlign(textView.WideSpaceWidth * (double)column, PixelSnapHelpers.GetPixelSize(textView).Width);
			num -= textView.ScrollOffset.X;
			drawingContext.DrawLine(point0: new Point(num, 0.0), point1: new Point(num, Math.Max(textView.DocumentHeight, textView.ActualHeight)), pen: pen);
		}
	}
}
