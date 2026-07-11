using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
	[CompilerGenerated]
	private FoldingManager qu8Y8rShUV;

	public static readonly DependencyProperty FoldingMarkerBrushProperty;

	public static readonly DependencyProperty FoldingMarkerBackgroundBrushProperty;

	public static readonly DependencyProperty SelectedFoldingMarkerBrushProperty;

	public static readonly DependencyProperty SelectedFoldingMarkerBackgroundBrushProperty;

	private List<FoldingMarginMarker> zeQYMUyf5O;

	private Pen sDgYVCGGG7;

	private Pen iLjY3Petma;

	public FoldingManager FoldingManager
	{
		[CompilerGenerated]
		get
		{
			return qu8Y8rShUV;
		}
		[CompilerGenerated]
		set
		{
			qu8Y8rShUV = value;
		}
	}

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

	protected override int VisualChildrenCount => zeQYMUyf5O.Count;

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

	private static void KcwYZvppKh(DependencyObject P_0, DependencyPropertyChangedEventArgs P_1)
	{
		FoldingMargin foldingMargin = null;
		if (P_0 is FoldingMargin)
		{
			foldingMargin = (FoldingMargin)(object)P_0;
		}
		else if (P_0 is TextEditor)
		{
			foldingMargin = ((TextEditor)(object)P_0).TextArea.LeftMargins.FirstOrDefault((UIElement c) => c is FoldingMargin) as FoldingMargin;
		}
		if (foldingMargin != null)
		{
			if (P_1.Property.Name == FoldingMarkerBrushProperty.Name)
			{
				foldingMargin.sDgYVCGGG7 = oNAYkLbqlF((Brush)P_1.NewValue);
			}
			if (P_1.Property.Name == SelectedFoldingMarkerBrushProperty.Name)
			{
				foldingMargin.iLjY3Petma = oNAYkLbqlF((Brush)P_1.NewValue);
			}
		}
	}

	protected override Size MeasureOverride(Size availableSize)
	{
		foreach (FoldingMarginMarker item in zeQYMUyf5O)
		{
			item.Measure(availableSize);
		}
		double value = 1.3333333333333333 * (double)((DependencyObject)this).GetValue(TextBlock.FontSizeProperty);
		Size pixelSize = PixelSnapHelpers.GetPixelSize(this);
		return new Size(PixelSnapHelpers.RoundToOdd(value, pixelSize.Width), 0.0);
	}

	protected override Size ArrangeOverride(Size finalSize)
	{
		Size pixelSize = PixelSnapHelpers.GetPixelSize(this);
		foreach (FoldingMarginMarker item in zeQYMUyf5O)
		{
			int visualColumn = item.yyfyla81Gf.GetVisualColumn(item.MfLyj1x2j1.StartOffset - item.yyfyla81Gf.FirstDocumentLine.Offset);
			TextLine textLine = item.yyfyla81Gf.GetTextLine(visualColumn);
			double num = item.yyfyla81Gf.GetTextLineVisualYPosition(textLine, VisualYPosition.TextMiddle) - base.TextView.VerticalOffset;
			double num2 = num;
			Size desiredSize = item.DesiredSize;
			num = num2 - desiredSize.Height / 2.0;
			double width = finalSize.Width;
			desiredSize = item.DesiredSize;
			double num3 = (width - desiredSize.Width) / 2.0;
			item.Arrange(new Rect(PixelSnapHelpers.Round(new Point(num3, num), pixelSize), item.DesiredSize));
		}
		return base.ArrangeOverride(finalSize);
	}

	protected override void OnTextViewChanged(TextView oldTextView, TextView newTextView)
	{
		if (oldTextView != null)
		{
			oldTextView.VisualLinesChanged -= iWMYJYOfN2;
		}
		base.OnTextViewChanged(oldTextView, newTextView);
		if (newTextView != null)
		{
			newTextView.VisualLinesChanged += iWMYJYOfN2;
		}
		iWMYJYOfN2(null, null);
	}

	private void iWMYJYOfN2(object P_0, EventArgs P_1)
	{
		foreach (FoldingMarginMarker item in zeQYMUyf5O)
		{
			RemoveVisualChild(item);
		}
		zeQYMUyf5O.Clear();
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
				FoldingMarginMarker esjaYkYR4eyUNhP8sbg = new FoldingMarginMarker
				{
					IsExpanded = !nextFolding.IsFolded,
					yyfyla81Gf = visualLine,
					MfLyj1x2j1 = nextFolding
				};
				zeQYMUyf5O.Add(esjaYkYR4eyUNhP8sbg);
				AddVisualChild(esjaYkYR4eyUNhP8sbg);
				esjaYkYR4eyUNhP8sbg.IsMouseDirectlyOverChanged += (DependencyPropertyChangedEventHandler)delegate
				{
					InvalidateVisual();
				};
				InvalidateMeasure();
			}
		}
	}

	protected override Visual GetVisualChild(int index)
	{
		return zeQYMUyf5O[index];
	}

	private static Pen oNAYkLbqlF(Brush P_0)
	{
		Pen pen = new Pen(P_0, 1.0);
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
			bnOY0BYZkf(list, array, array2);
			DZvY7sV2gO(list, array, array2);
			fOZYX0AKvW(drawingContext, array, array2);
			base.OnRender(drawingContext);
		}
	}

	private void bnOY0BYZkf(List<TextLine> P_0, Pen[] P_1, Pen[] P_2)
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
				int num2 = FMmYUbowut(P_0, endOffset2);
				if (num2 >= 0)
				{
					P_2[num2] = sDgYVCGGG7;
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
			for (int i = 0; i < P_1.Length; i++)
			{
				P_1[i] = sDgYVCGGG7;
			}
			return;
		}
		int num3 = FMmYUbowut(P_0, num);
		for (int j = 0; j <= num3; j++)
		{
			P_1[j] = sDgYVCGGG7;
		}
	}

	private void DZvY7sV2gO(List<TextLine> P_0, Pen[] P_1, Pen[] P_2)
	{
		foreach (FoldingMarginMarker item in zeQYMUyf5O)
		{
			int endOffset = item.MfLyj1x2j1.EndOffset;
			int num = FMmYUbowut(P_0, endOffset);
			if (!item.MfLyj1x2j1.IsFolded && num >= 0)
			{
				if (item.IsMouseDirectlyOver)
				{
					P_2[num] = iLjY3Petma;
				}
				else if (P_2[num] == null)
				{
					P_2[num] = sDgYVCGGG7;
				}
			}
			int num2 = FMmYUbowut(P_0, item.MfLyj1x2j1.StartOffset);
			if (num2 < 0)
			{
				continue;
			}
			for (int i = num2 + 1; i < P_1.Length && i - 1 != num; i++)
			{
				if (item.IsMouseDirectlyOver)
				{
					P_1[i] = iLjY3Petma;
				}
				else if (P_1[i] == null)
				{
					P_1[i] = sDgYVCGGG7;
				}
			}
		}
	}

	private void fOZYX0AKvW(DrawingContext P_0, Pen[] P_1, Pen[] P_2)
	{
		Size pixelSize = PixelSnapHelpers.GetPixelSize(this);
		Size renderSize = base.RenderSize;
		double num = PixelSnapHelpers.PixelAlign(renderSize.Width / 2.0, pixelSize.Width);
		double num2 = 0.0;
		Pen pen = P_1[0];
		int num3 = 0;
		foreach (VisualLine visualLine in base.TextView.VisualLines)
		{
			foreach (TextLine textLine in visualLine.TextLines)
			{
				if (P_2[num3] != null)
				{
					double num4 = AycYpfmZbG(visualLine, textLine, pixelSize.Height);
					Pen pen2 = P_2[num3];
					Point point = new Point(num - pixelSize.Width / 2.0, num4);
					renderSize = base.RenderSize;
					P_0.DrawLine(pen2, point, new Point(renderSize.Width, num4));
				}
				if (P_1[num3 + 1] != pen)
				{
					double num5 = AycYpfmZbG(visualLine, textLine, pixelSize.Height);
					if (pen != null)
					{
						P_0.DrawLine(pen, new Point(num, num2 + pixelSize.Height / 2.0), new Point(num, num5 - pixelSize.Height / 2.0));
					}
					pen = P_1[num3 + 1];
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
			P_0.DrawLine(pen3, point2, new Point(num, renderSize.Height));
		}
	}

	private double AycYpfmZbG(VisualLine P_0, TextLine P_1, double P_2)
	{
		return PixelSnapHelpers.PixelAlign(P_0.GetTextLineVisualYPosition(P_1, VisualYPosition.TextMiddle) - base.TextView.VerticalOffset, P_2);
	}

	private int FMmYUbowut(List<TextLine> P_0, int P_1)
	{
		int lineNumber = base.TextView.Document.GetLineByOffset(P_1).LineNumber;
		VisualLine visualLine = base.TextView.GetVisualLine(lineNumber);
		if (visualLine != null)
		{
			int relativeTextOffset = P_1 - visualLine.FirstDocumentLine.Offset;
			TextLine textLine = visualLine.GetTextLine(visualLine.GetVisualColumn(relativeTextOffset));
			return P_0.IndexOf(textLine);
		}
		return -1;
	}

	public FoldingMargin()
	{
		zeQYMUyf5O = new List<FoldingMarginMarker>();
		sDgYVCGGG7 = oNAYkLbqlF((Brush)FoldingMarkerBrushProperty.DefaultMetadata.DefaultValue);
		iLjY3Petma = oNAYkLbqlF((Brush)SelectedFoldingMarkerBrushProperty.DefaultMetadata.DefaultValue);
	}

	static FoldingMargin()
	{
		FoldingMarkerBrushProperty = DependencyProperty.RegisterAttached("FoldingMarkerBrush", typeof(Brush), typeof(FoldingMargin), (PropertyMetadata)(object)new FrameworkPropertyMetadata(Brushes.Gray, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(KcwYZvppKh)));
		FoldingMarkerBackgroundBrushProperty = DependencyProperty.RegisterAttached("FoldingMarkerBackgroundBrush", typeof(Brush), typeof(FoldingMargin), (PropertyMetadata)(object)new FrameworkPropertyMetadata(Brushes.White, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(KcwYZvppKh)));
		SelectedFoldingMarkerBrushProperty = DependencyProperty.RegisterAttached("SelectedFoldingMarkerBrush", typeof(Brush), typeof(FoldingMargin), (PropertyMetadata)(object)new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(KcwYZvppKh)));
		SelectedFoldingMarkerBackgroundBrushProperty = DependencyProperty.RegisterAttached("SelectedFoldingMarkerBackgroundBrush", typeof(Brush), typeof(FoldingMargin), (PropertyMetadata)(object)new FrameworkPropertyMetadata(Brushes.White, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(KcwYZvppKh)));
	}

	[CompilerGenerated]
	private void MVnYcnabCK(object P_0, DependencyPropertyChangedEventArgs P_1)
	{
		InvalidateVisual();
	}
}
