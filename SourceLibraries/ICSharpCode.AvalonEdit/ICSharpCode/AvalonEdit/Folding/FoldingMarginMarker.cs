using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Folding;

internal sealed class FoldingMarginMarker : UIElement
{
	internal VisualLine VisualLine;

	internal FoldingSection FoldingSection;

	private bool isExpanded;

	private const double MarginSizeFactor = 0.7;

	public bool IsExpanded
	{
		get
		{
			return isExpanded;
		}
		set
		{
			if (isExpanded != value)
			{
				isExpanded = value;
				InvalidateVisual();
			}
			if (FoldingSection != null)
			{
				FoldingSection.IsFolded = !value;
			}
		}
	}

	protected override void OnMouseDown(MouseButtonEventArgs e)
	{
		base.OnMouseDown(e);
		if (!e.Handled && e.ChangedButton == MouseButton.Left)
		{
			IsExpanded = !IsExpanded;
			e.Handled = true;
		}
	}

	protected override Size MeasureCore(Size availableSize)
	{
		double num = PixelSnapHelpers.RoundToOdd(0.9333333333333332 * (double)GetValue(TextBlock.FontSizeProperty), PixelSnapHelpers.GetPixelSize(this).Width);
		return new Size(num, num);
	}

	protected override void OnRender(DrawingContext drawingContext)
	{
		if (base.VisualParent is FoldingMargin foldingMargin)
		{
			Pen pen = new Pen(foldingMargin.SelectedFoldingMarkerBrush, 1.0);
			Pen pen2 = new Pen(foldingMargin.FoldingMarkerBrush, 1.0);
			PenLineCap startLineCap = (pen2.StartLineCap = PenLineCap.Square);
			pen.StartLineCap = startLineCap;
			startLineCap = (pen2.EndLineCap = PenLineCap.Square);
			pen.EndLineCap = startLineCap;
			Size pixelSize = PixelSnapHelpers.GetPixelSize(this);
			Rect rectangle = new Rect(pixelSize.Width / 2.0, pixelSize.Height / 2.0, base.RenderSize.Width - pixelSize.Width, base.RenderSize.Height - pixelSize.Height);
			drawingContext.DrawRectangle(base.IsMouseDirectlyOver ? foldingMargin.SelectedFoldingMarkerBackgroundBrush : foldingMargin.FoldingMarkerBackgroundBrush, base.IsMouseDirectlyOver ? pen : pen2, rectangle);
			double x = rectangle.Left + rectangle.Width / 2.0;
			double y = rectangle.Top + rectangle.Height / 2.0;
			double num = PixelSnapHelpers.Round(rectangle.Width / 8.0, pixelSize.Width) + pixelSize.Width;
			drawingContext.DrawLine(pen, new Point(rectangle.Left + num, y), new Point(rectangle.Right - num, y));
			if (!isExpanded)
			{
				drawingContext.DrawLine(pen, new Point(x, rectangle.Top + num), new Point(x, rectangle.Bottom - num));
			}
		}
	}

	protected override void OnIsMouseDirectlyOverChanged(DependencyPropertyChangedEventArgs e)
	{
		base.OnIsMouseDirectlyOverChanged(e);
		InvalidateVisual();
	}
}
