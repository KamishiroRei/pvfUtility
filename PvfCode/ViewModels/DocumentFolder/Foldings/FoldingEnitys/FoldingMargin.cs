using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;
using PvfCode.ViewModels.DocumentFolder.Foldings;

namespace PvfCode.ViewModels.DocumentFolder.Foldings.FoldingEnitys;

public class FoldingMargin : AbstractMargin
{
	public static readonly DependencyProperty FoldingMarkerBrushProperty;

	public static readonly DependencyProperty FoldingMarkerBackgroundBrushProperty;

	public static readonly DependencyProperty SelectedFoldingMarkerBrushProperty;

	public static readonly DependencyProperty SelectedFoldingMarkerBackgroundBrushProperty;

	private List<FoldingMarginMarker> markers;

	private Pen foldingControlPen;

	private Pen selectedFoldingControlPen;

	public FoldingManager FoldingManager { get; set; }

	public Brush FoldingMarkerBrush
	{
		get
		{
			return (Brush)((DependencyObject)this).GetValue(FoldingMarkerBrushProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(FoldingMarkerBrushProperty, (object)value);
		}
	}

	public Brush FoldingMarkerBackgroundBrush
	{
		get
		{
			return (Brush)((DependencyObject)this).GetValue(FoldingMarkerBackgroundBrushProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(FoldingMarkerBackgroundBrushProperty, (object)value);
		}
	}

	public Brush SelectedFoldingMarkerBrush
	{
		get
		{
			return (Brush)((DependencyObject)this).GetValue(SelectedFoldingMarkerBrushProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(SelectedFoldingMarkerBrushProperty, (object)value);
		}
	}

	public Brush SelectedFoldingMarkerBackgroundBrush
	{
		get
		{
			return (Brush)((DependencyObject)this).GetValue(SelectedFoldingMarkerBackgroundBrushProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(SelectedFoldingMarkerBackgroundBrushProperty, (object)value);
		}
	}

	protected override int VisualChildrenCount => markers.Count;

	public static Brush GetFoldingMarkerBrush(DependencyObject obj)
	{
		return (Brush)obj.GetValue(FoldingMarkerBrushProperty);
	}

	public static void SetFoldingMarkerBrush(DependencyObject obj, Brush value)
	{
		obj.SetValue(FoldingMarkerBrushProperty, (object)value);
	}

	public static Brush GetFoldingMarkerBackgroundBrush(DependencyObject obj)
	{
		return (Brush)obj.GetValue(FoldingMarkerBackgroundBrushProperty);
	}

	public static void SetFoldingMarkerBackgroundBrush(DependencyObject obj, Brush value)
	{
		obj.SetValue(FoldingMarkerBackgroundBrushProperty, (object)value);
	}

	public static Brush GetSelectedFoldingMarkerBrush(DependencyObject obj)
	{
		return (Brush)obj.GetValue(SelectedFoldingMarkerBrushProperty);
	}

	public static void SetSelectedFoldingMarkerBrush(DependencyObject obj, Brush value)
	{
		obj.SetValue(SelectedFoldingMarkerBrushProperty, (object)value);
	}

	public static Brush GetSelectedFoldingMarkerBackgroundBrush(DependencyObject obj)
	{
		return (Brush)obj.GetValue(SelectedFoldingMarkerBackgroundBrushProperty);
	}

	public static void SetSelectedFoldingMarkerBackgroundBrush(DependencyObject obj, Brush value)
	{
		obj.SetValue(SelectedFoldingMarkerBackgroundBrushProperty, (object)value);
	}

	private static void OnUpdateBrushes(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
	{
		FoldingMargin foldingMargin = null;
		if (dependencyObject is FoldingMargin)
		{
			foldingMargin = (FoldingMargin)dependencyObject;
		}
		else if (dependencyObject is TextEditor)
		{
			foldingMargin = ((TextEditor)dependencyObject).TextArea.LeftMargins.FirstOrDefault((UIElement c) => c is FoldingMargin) as FoldingMargin;
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
		double value = 1.3333333333333333 * (double)((DependencyObject)this).GetValue(TextBlock.FontSizeProperty);
		Size pixelSize = PixelSnapHelpers.GetPixelSize(this);
		return new Size(PixelSnapHelpers.RoundToOdd(value, pixelSize.Width), 0.0);
	}

	protected override Size ArrangeOverride(Size finalSize)
	{
		Size pixelSize = PixelSnapHelpers.GetPixelSize(this);
		foreach (FoldingMarginMarker marker in markers)
		{
			int visualColumn = marker.VisualLine.GetVisualColumn(marker.FoldingSection.StartOffset - marker.VisualLine.FirstDocumentLine.Offset);
			TextLine textLine = marker.VisualLine.GetTextLine(visualColumn);
			double num = marker.VisualLine.GetTextLineVisualYPosition(textLine, VisualYPosition.TextMiddle) - base.TextView.VerticalOffset;
			double num2 = num;
			Size desiredSize = marker.DesiredSize;
			num = num2 - desiredSize.Height / 2.0;
			double width = finalSize.Width;
			desiredSize = marker.DesiredSize;
			double num3 = (width - desiredSize.Width) / 2.0;
			marker.Arrange(new Rect(PixelSnapHelpers.Round(new Point(num3, num), pixelSize), marker.DesiredSize));
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
				FoldingMarginMarker marker = new FoldingMarginMarker
				{
					IsExpanded = !nextFolding.IsFolded,
					VisualLine = visualLine,
					FoldingSection = nextFolding
				};
				markers.Add(marker);
				AddVisualChild(marker);
				marker.IsMouseDirectlyOverChanged += (DependencyPropertyChangedEventHandler)delegate
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
		((Freezable)pen).Freeze();
		return pen;
	}

	protected override void OnRender(DrawingContext drawingContext)
	{
		if (base.TextView != null && base.TextView.VisualLinesValid && base.TextView.VisualLines.Count != 0 && FoldingManager != null)
		{
			List<TextLine> list = base.TextView.VisualLines.SelectMany((VisualLine vl) => vl.TextLines).ToList();
			Pen[] array = new Pen[list.Count + 1];
			Pen[] array2 = new Pen[list.Count];
			CalculateFoldLinesForFoldingsActiveAtStart(list, array, array2);
			CalculateFoldLinesForMarkers(list, array, array2);
			DrawFoldLines(drawingContext, array, array2);
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
				if (endOffset2 < 0 || endOffset2 > base.TextView.Document.TextLength)
				{
					return;
				}
				int num2 = GetTextLineIndexFromOffset(allTextLines, endOffset2);
				if (num2 >= 0)
				{
					endMarker[num2] = foldingControlPen;
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
		int num3 = GetTextLineIndexFromOffset(allTextLines, num);
		for (int j = 0; j <= num3; j++)
		{
			colors[j] = foldingControlPen;
		}
	}

	private void CalculateFoldLinesForMarkers(List<TextLine> allTextLines, Pen[] colors, Pen[] endMarker)
	{
		foreach (FoldingMarginMarker marker in markers)
		{
			int endOffset = marker.FoldingSection.EndOffset;
			int num = GetTextLineIndexFromOffset(allTextLines, endOffset);
			if (!marker.FoldingSection.IsFolded && num >= 0)
			{
				if (marker.IsMouseDirectlyOver)
				{
					endMarker[num] = selectedFoldingControlPen;
				}
				else if (endMarker[num] == null)
				{
					endMarker[num] = foldingControlPen;
				}
			}
			int num2 = GetTextLineIndexFromOffset(allTextLines, marker.FoldingSection.StartOffset);
			if (num2 < 0)
			{
				continue;
			}
			for (int i = num2 + 1; i < colors.Length && i - 1 != num; i++)
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
		Size renderSize = base.RenderSize;
		double num = PixelSnapHelpers.PixelAlign(renderSize.Width / 2.0, pixelSize.Width);
		double num2 = 0.0;
		Pen pen = colors[0];
		int num3 = 0;
		foreach (VisualLine visualLine in base.TextView.VisualLines)
		{
			foreach (TextLine textLine in visualLine.TextLines)
			{
				if (endMarker[num3] != null)
				{
					double num4 = GetVisualPos(visualLine, textLine, pixelSize.Height);
					Pen pen2 = endMarker[num3];
					Point point = new Point(num - pixelSize.Width / 2.0, num4);
					renderSize = base.RenderSize;
					drawingContext.DrawLine(pen2, point, new Point(renderSize.Width, num4));
				}
				if (colors[num3 + 1] != pen)
				{
					double num5 = GetVisualPos(visualLine, textLine, pixelSize.Height);
					if (pen != null)
					{
						drawingContext.DrawLine(pen, new Point(num, num2 + pixelSize.Height / 2.0), new Point(num, num5 - pixelSize.Height / 2.0));
					}
					pen = colors[num3 + 1];
					num2 = num5;
				}
				num3++;
			}
		}
		if (pen != null)
		{
			Pen pen3 = pen;
			Point point2 = new Point(num, num2 + pixelSize.Height / 2.0);
			renderSize = base.RenderSize;
			drawingContext.DrawLine(pen3, point2, new Point(num, renderSize.Height));
		}
	}

	private double GetVisualPos(VisualLine visualLine, TextLine textLine, double pixelHeight)
	{
		return PixelSnapHelpers.PixelAlign(visualLine.GetTextLineVisualYPosition(textLine, VisualYPosition.TextMiddle) - base.TextView.VerticalOffset, pixelHeight);
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

	public FoldingMargin()
	{
		markers = new List<FoldingMarginMarker>();
		foldingControlPen = MakeFrozenPen((Brush)FoldingMarkerBrushProperty.DefaultMetadata.DefaultValue);
		selectedFoldingControlPen = MakeFrozenPen((Brush)SelectedFoldingMarkerBrushProperty.DefaultMetadata.DefaultValue);
	}

	static FoldingMargin()
	{
		FoldingMarkerBrushProperty = DependencyProperty.RegisterAttached("FoldingMarkerBrush", typeof(Brush), typeof(FoldingMargin), (PropertyMetadata)(object)new FrameworkPropertyMetadata(Brushes.Gray, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnUpdateBrushes)));
		FoldingMarkerBackgroundBrushProperty = DependencyProperty.RegisterAttached("FoldingMarkerBackgroundBrush", typeof(Brush), typeof(FoldingMargin), (PropertyMetadata)(object)new FrameworkPropertyMetadata(Brushes.White, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnUpdateBrushes)));
		SelectedFoldingMarkerBrushProperty = DependencyProperty.RegisterAttached("SelectedFoldingMarkerBrush", typeof(Brush), typeof(FoldingMargin), (PropertyMetadata)(object)new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnUpdateBrushes)));
		SelectedFoldingMarkerBackgroundBrushProperty = DependencyProperty.RegisterAttached("SelectedFoldingMarkerBackgroundBrush", typeof(Brush), typeof(FoldingMargin), (PropertyMetadata)(object)new FrameworkPropertyMetadata(Brushes.White, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnUpdateBrushes)));
	}
}
