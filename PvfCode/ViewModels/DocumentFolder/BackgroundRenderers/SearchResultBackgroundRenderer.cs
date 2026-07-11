using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using PvfCode.ViewModels.DocumentFolder.SearchViewModels.Searchs;

namespace PvfCode.ViewModels.DocumentFolder.BackgroundRenderers;

internal class SearchResultBackgroundRenderer : IBackgroundRenderer
{
	public SearchResultBackgroundRenderer()
	{
		Segments = new TextSegmentCollection<SearchResult>();
		Brush = Brushes.LightGreen;
		CornerRadius = 3.0;
	}

	public TextSegmentCollection<SearchResult> Segments { get; private set; }

	public Brush Brush { get; set; }

	public Pen Pen { get; set; }

	public double CornerRadius { get; set; }

	public KnownLayer Layer => KnownLayer.Selection;

	public void Draw(TextView textView, DrawingContext drawingContext)
	{
		if (textView == null)
		{
			throw new ArgumentNullException(nameof(textView));
		}
		if (drawingContext == null)
		{
			throw new ArgumentNullException(nameof(drawingContext));
		}
		if (Segments == null || !textView.VisualLinesValid)
		{
			return;
		}

		ReadOnlyCollection<VisualLine> visualLines = textView.VisualLines;
		if (visualLines.Count == 0)
		{
			return;
		}

		int startOffset = visualLines.First().FirstDocumentLine.Offset;
		int endOffset = visualLines.Last().LastDocumentLine.EndOffset;
		Brush = (SolidColorBrush)textView.FindResource("SearchResultMarkerBrush");
		double borderThickness = Pen?.Thickness ?? 0.0;
		foreach (SearchResult result in Segments.FindOverlappingSegments(startOffset, endOffset - startOffset))
		{
			BackgroundGeometryBuilder geometryBuilder = new BackgroundGeometryBuilder
			{
				AlignToWholePixels = true,
				BorderThickness = borderThickness,
				CornerRadius = CornerRadius
			};
			geometryBuilder.AddSegment(textView, result);
			Geometry geometry = geometryBuilder.CreateGeometry();
			if (geometry != null)
			{
				drawingContext.DrawGeometry(Brush, Pen, geometry);
			}
		}
	}

	public void Clear()
	{
		Segments = new TextSegmentCollection<SearchResult>();
	}
}
