using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Folding;

public class FoldingMargin : AbstractMargin
{
	internal const double SizeFactor = 1.3333333333333333;

	public static readonly DependencyProperty FoldingMarkerBrushProperty = DependencyProperty.RegisterAttached("FoldingMarkerBrush", typeof(Brush), typeof(FoldingMargin), new FrameworkPropertyMetadata(Brushes.Gray, FrameworkPropertyMetadataOptions.Inherits, OnUpdateBrushes));

	public static readonly DependencyProperty FoldingMarkerBackgroundBrushProperty = DependencyProperty.RegisterAttached("FoldingMarkerBackgroundBrush", typeof(Brush), typeof(FoldingMargin), new FrameworkPropertyMetadata(Brushes.White, FrameworkPropertyMetadataOptions.Inherits, OnUpdateBrushes));

	public static readonly DependencyProperty SelectedFoldingMarkerBrushProperty = DependencyProperty.RegisterAttached("SelectedFoldingMarkerBrush", typeof(Brush), typeof(FoldingMargin), new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.Inherits, OnUpdateBrushes));

	public static readonly DependencyProperty SelectedFoldingMarkerBackgroundBrushProperty = DependencyProperty.RegisterAttached("SelectedFoldingMarkerBackgroundBrush", typeof(Brush), typeof(FoldingMargin), new FrameworkPropertyMetadata(Brushes.White, FrameworkPropertyMetadataOptions.Inherits, OnUpdateBrushes));

	private List<FoldingMarginMarker> markers = new List<FoldingMarginMarker>();

	private Pen foldingControlPen = MakeFrozenPen((Brush)FoldingMarkerBrushProperty.DefaultMetadata.DefaultValue);

	private Pen selectedFoldingControlPen = MakeFrozenPen((Brush)SelectedFoldingMarkerBrushProperty.DefaultMetadata.DefaultValue);

	public FoldingManager FoldingManager { get; set; }

	public Brush FoldingMarkerBrush
	{
		get
		{
			return (Brush)GetValue(FoldingMarkerBrushProperty);
		}
		set
		{
			SetValue(FoldingMarkerBrushProperty, value);
		}
	}

	public Brush FoldingMarkerBackgroundBrush
	{
		get
		{
			return (Brush)GetValue(FoldingMarkerBackgroundBrushProperty);
		}
		set
		{
			SetValue(FoldingMarkerBackgroundBrushProperty, value);
		}
	}

	public Brush SelectedFoldingMarkerBrush
	{
		get
		{
			return (Brush)GetValue(SelectedFoldingMarkerBrushProperty);
		}
		set
		{
			SetValue(SelectedFoldingMarkerBrushProperty, value);
		}
	}

	public Brush SelectedFoldingMarkerBackgroundBrush
	{
		get
		{
			return (Brush)GetValue(SelectedFoldingMarkerBackgroundBrushProperty);
		}
		set
		{
			SetValue(SelectedFoldingMarkerBackgroundBrushProperty, value);
		}
	}

	protected override int VisualChildrenCount => markers.Count;

	public static Brush GetFoldingMarkerBrush(DependencyObject obj)
	{
		return (Brush)obj.GetValue(FoldingMarkerBrushProperty);
	}

	public static void SetFoldingMarkerBrush(DependencyObject obj, Brush value)
	{
		obj.SetValue(FoldingMarkerBrushProperty, value);
	}

	public static Brush GetFoldingMarkerBackgroundBrush(DependencyObject obj)
	{
		return (Brush)obj.GetValue(FoldingMarkerBackgroundBrushProperty);
	}

	public static void SetFoldingMarkerBackgroundBrush(DependencyObject obj, Brush value)
	{
		obj.SetValue(FoldingMarkerBackgroundBrushProperty, value);
	}

	public static Brush GetSelectedFoldingMarkerBrush(DependencyObject obj)
	{
		return (Brush)obj.GetValue(SelectedFoldingMarkerBrushProperty);
	}

	public static void SetSelectedFoldingMarkerBrush(DependencyObject obj, Brush value)
	{
		obj.SetValue(SelectedFoldingMarkerBrushProperty, value);
	}

	public static Brush GetSelectedFoldingMarkerBackgroundBrush(DependencyObject obj)
	{
		return (Brush)obj.GetValue(SelectedFoldingMarkerBackgroundBrushProperty);
	}

	public static void SetSelectedFoldingMarkerBackgroundBrush(DependencyObject obj, Brush value)
	{
		obj.SetValue(SelectedFoldingMarkerBackgroundBrushProperty, value);
	}

	private static void OnUpdateBrushes(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		FoldingMargin foldingMargin = null;
		if (d is FoldingMargin)
		{
			foldingMargin = (FoldingMargin)d;
		}
		else if (d is TextEditor)
		{
			foldingMargin = ((TextEditor)d).TextArea.LeftMargins.FirstOrDefault((UIElement c) => c is FoldingMargin) as FoldingMargin;
		}
		if (foldingMargin != null)
		{
			if (e.Property.Name == FoldingMarkerBrushProperty.Name)
			{
				foldingMargin.foldingControlPen = MakeFrozenPen((Brush)e.NewValue);
			}
			if (e.Property.Name == SelectedFoldingMarkerBrushProperty.Name)
			{
				foldingMargin.selectedFoldingControlPen = MakeFrozenPen((Brush)e.NewValue);
			}
		}
	}

	protected override Size MeasureOverride(Size availableSize)
	{
		foreach (FoldingMarginMarker marker in markers)
		{
			marker.Measure(availableSize);
		}
		return new Size(PixelSnapHelpers.RoundToOdd(1.3333333333333333 * (double)GetValue(TextBlock.FontSizeProperty), PixelSnapHelpers.GetPixelSize(this).Width), 0.0);
	}

	protected override Size ArrangeOverride(Size finalSize)
	{
		Size pixelSize = PixelSnapHelpers.GetPixelSize(this);
		foreach (FoldingMarginMarker marker in markers)
		{
			int visualColumn = marker.VisualLine.GetVisualColumn(marker.FoldingSection.StartOffset - marker.VisualLine.FirstDocumentLine.Offset);
			TextLine textLine = marker.VisualLine.GetTextLine(visualColumn);
			double num = marker.VisualLine.GetTextLineVisualYPosition(textLine, VisualYPosition.TextMiddle) - base.TextView.VerticalOffset;
			num -= marker.DesiredSize.Height / 2.0;
			double x = (finalSize.Width - marker.DesiredSize.Width) / 2.0;
			marker.Arrange(new Rect(PixelSnapHelpers.Round(new Point(x, num), pixelSize), marker.DesiredSize));
		}
		return base.ArrangeOverride(finalSize);
	}

	protected override void OnTextViewChanged(TextView oldTextView, TextView newTextView)
	{
		if (oldTextView != null)
		{
			oldTextView.VisualLinesChanged -= TextViewVisualLinesChanged;
		}
		base.OnTextViewChanged(oldTextView, newTextView);
		if (newTextView != null)
		{
			newTextView.VisualLinesChanged += TextViewVisualLinesChanged;
		}
		TextViewVisualLinesChanged(null, null);
	}

	private void TextViewVisualLinesChanged(object sender, EventArgs e)
	{
		foreach (FoldingMarginMarker marker in markers)
		{
			RemoveVisualChild(marker);
		}
		markers.Clear();
		InvalidateVisual();
		if (base.TextView == null || FoldingManager == null || !base.TextView.VisualLinesValid)
		{
			return;
		}
		foreach (VisualLine visualLine in base.TextView.VisualLines)
		{
			FoldingSection nextFolding = FoldingManager.GetNextFolding(visualLine.FirstDocumentLine.Offset);
			if (nextFolding != null && nextFolding.StartOffset <= visualLine.LastDocumentLine.Offset + visualLine.LastDocumentLine.Length)
			{
				FoldingMarginMarker foldingMarginMarker = new FoldingMarginMarker
				{
					IsExpanded = !nextFolding.IsFolded,
					VisualLine = visualLine,
					FoldingSection = nextFolding
				};
				markers.Add(foldingMarginMarker);
				AddVisualChild(foldingMarginMarker);
				foldingMarginMarker.IsMouseDirectlyOverChanged += delegate
				{
					InvalidateVisual();
				};
				InvalidateMeasure();
			}
		}
	}

	protected override Visual GetVisualChild(int index)
	{
		return markers[index];
	}

	private static Pen MakeFrozenPen(Brush brush)
	{
		Pen pen = new Pen(brush, 1.0);
		pen.Freeze();
		return pen;
	}

	protected override void OnRender(DrawingContext drawingContext)
	{
		if (base.TextView != null && base.TextView.VisualLinesValid && base.TextView.VisualLines.Count != 0 && FoldingManager != null)
		{
			List<TextLine> list = base.TextView.VisualLines.SelectMany((VisualLine vl) => vl.TextLines).ToList();
			Pen[] colors = new Pen[list.Count + 1];
			Pen[] endMarker = new Pen[list.Count];
			CalculateFoldLinesForFoldingsActiveAtStart(list, colors, endMarker);
			CalculateFoldLinesForMarkers(list, colors, endMarker);
			DrawFoldLines(drawingContext, colors, endMarker);
			base.OnRender(drawingContext);
		}
	}

	private void CalculateFoldLinesForFoldingsActiveAtStart(List<TextLine> allTextLines, Pen[] colors, Pen[] endMarker)
	{
		int offset = base.TextView.VisualLines[0].FirstDocumentLine.Offset;
		int endOffset = base.TextView.VisualLines.Last().LastDocumentLine.EndOffset;
		ReadOnlyCollection<FoldingSection> foldingsContaining = FoldingManager.GetFoldingsContaining(offset);
		int num = 0;
		foreach (FoldingSection item in foldingsContaining)
		{
			int endOffset2 = item.EndOffset;
			if (endOffset2 <= endOffset && !item.IsFolded)
			{
				int textLineIndexFromOffset = GetTextLineIndexFromOffset(allTextLines, endOffset2);
				if (textLineIndexFromOffset >= 0)
				{
					endMarker[textLineIndexFromOffset] = foldingControlPen;
				}
			}
			if (endOffset2 > num && item.StartOffset < offset)
			{
				num = endOffset2;
			}
		}
		if (num <= 0)
		{
			return;
		}
		if (num > endOffset)
		{
			for (int i = 0; i < colors.Length; i++)
			{
				colors[i] = foldingControlPen;
			}
			return;
		}
		int textLineIndexFromOffset2 = GetTextLineIndexFromOffset(allTextLines, num);
		for (int j = 0; j <= textLineIndexFromOffset2; j++)
		{
			colors[j] = foldingControlPen;
		}
	}

	private void CalculateFoldLinesForMarkers(List<TextLine> allTextLines, Pen[] colors, Pen[] endMarker)
	{
		foreach (FoldingMarginMarker marker in markers)
		{
			int endOffset = marker.FoldingSection.EndOffset;
			int textLineIndexFromOffset = GetTextLineIndexFromOffset(allTextLines, endOffset);
			if (!marker.FoldingSection.IsFolded && textLineIndexFromOffset >= 0)
			{
				if (marker.IsMouseDirectlyOver)
				{
					endMarker[textLineIndexFromOffset] = selectedFoldingControlPen;
				}
				else if (endMarker[textLineIndexFromOffset] == null)
				{
					endMarker[textLineIndexFromOffset] = foldingControlPen;
				}
			}
			int textLineIndexFromOffset2 = GetTextLineIndexFromOffset(allTextLines, marker.FoldingSection.StartOffset);
			if (textLineIndexFromOffset2 < 0)
			{
				continue;
			}
			for (int i = textLineIndexFromOffset2 + 1; i < colors.Length && i - 1 != textLineIndexFromOffset; i++)
			{
				if (marker.IsMouseDirectlyOver)
				{
					colors[i] = selectedFoldingControlPen;
				}
				else if (colors[i] == null)
				{
					colors[i] = foldingControlPen;
				}
			}
		}
	}

	private void DrawFoldLines(DrawingContext drawingContext, Pen[] colors, Pen[] endMarker)
	{
		Size pixelSize = PixelSnapHelpers.GetPixelSize(this);
		double num = PixelSnapHelpers.PixelAlign(base.RenderSize.Width / 2.0, pixelSize.Width);
		double num2 = 0.0;
		Pen pen = colors[0];
		int num3 = 0;
		foreach (VisualLine visualLine in base.TextView.VisualLines)
		{
			foreach (TextLine textLine in visualLine.TextLines)
			{
				if (endMarker[num3] != null)
				{
					double visualPos = GetVisualPos(visualLine, textLine, pixelSize.Height);
					drawingContext.DrawLine(endMarker[num3], new Point(num - pixelSize.Width / 2.0, visualPos), new Point(base.RenderSize.Width, visualPos));
				}
				if (colors[num3 + 1] != pen)
				{
					double visualPos2 = GetVisualPos(visualLine, textLine, pixelSize.Height);
					if (pen != null)
					{
						drawingContext.DrawLine(pen, new Point(num, num2 + pixelSize.Height / 2.0), new Point(num, visualPos2 - pixelSize.Height / 2.0));
					}
					pen = colors[num3 + 1];
					num2 = visualPos2;
				}
				num3++;
			}
		}
		if (pen != null)
		{
			drawingContext.DrawLine(pen, new Point(num, num2 + pixelSize.Height / 2.0), new Point(num, base.RenderSize.Height));
		}
	}

	private double GetVisualPos(VisualLine vl, TextLine tl, double pixelHeight)
	{
		return PixelSnapHelpers.PixelAlign(vl.GetTextLineVisualYPosition(tl, VisualYPosition.TextMiddle) - base.TextView.VerticalOffset, pixelHeight);
	}

	private int GetTextLineIndexFromOffset(List<TextLine> textLines, int offset)
	{
		int lineNumber = base.TextView.Document.GetLineByOffset(offset).LineNumber;
		VisualLine visualLine = base.TextView.GetVisualLine(lineNumber);
		if (visualLine != null)
		{
			int relativeTextOffset = offset - visualLine.FirstDocumentLine.Offset;
			TextLine textLine = visualLine.GetTextLine(visualLine.GetVisualColumn(relativeTextOffset));
			return textLines.IndexOf(textLine);
		}
		return -1;
	}
}
