using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;
using PvfCode.ViewModels.DocumentFolder.Foldings.FoldingEnitys;

namespace PvfCode.ViewModels.DocumentFolder.Foldings;

internal sealed class FoldingMarginMarker : UIElement
{
	internal VisualLine VisualLine;

	internal FoldingSection FoldingSection;

	private bool isExpanded;

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
		double value = 0.9333333333333332 * (double)((DependencyObject)this).GetValue(TextBlock.FontSizeProperty);
		Size pixelSize = PixelSnapHelpers.GetPixelSize(this);
		double num = PixelSnapHelpers.RoundToOdd(value, pixelSize.Width);
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
			double num = pixelSize.Width / 2.0;
			double num2 = pixelSize.Height / 2.0;
			Size renderSize = base.RenderSize;
			double num3 = renderSize.Width - pixelSize.Width;
			renderSize = base.RenderSize;
			Rect rectangle = default(Rect);
			rectangle = new Rect(num, num2, num3, renderSize.Height - pixelSize.Height);
			drawingContext.DrawRectangle(base.IsMouseDirectlyOver ? foldingMargin.SelectedFoldingMarkerBackgroundBrush : foldingMargin.FoldingMarkerBackgroundBrush, base.IsMouseDirectlyOver ? pen : pen2, rectangle);
			double num4 = rectangle.Left + rectangle.Width / 2.0;
			double num5 = rectangle.Top + rectangle.Height / 2.0;
			double num6 = PixelSnapHelpers.Round(rectangle.Width / 8.0, pixelSize.Width) + pixelSize.Width;
			drawingContext.DrawLine(pen, new Point(rectangle.Left + num6, num5), new Point(rectangle.Right - num6, num5));
			if (!isExpanded)
			{
				drawingContext.DrawLine(pen, new Point(num4, rectangle.Top + num6), new Point(num4, rectangle.Bottom - num6));
			}
		}
	}

	protected override void OnIsMouseDirectlyOverChanged(DependencyPropertyChangedEventArgs e)
	{
		base.OnIsMouseDirectlyOverChanged(e);
		InvalidateVisual();
	}

	public FoldingMarginMarker()
	{
	}
}
