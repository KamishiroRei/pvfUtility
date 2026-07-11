using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace ICSharpCode.AvalonEdit.Search;

internal class SearchResultBackgroundRenderer : IBackgroundRenderer
{
	private TextSegmentCollection<SearchResult> currentResults = new TextSegmentCollection<SearchResult>();

	public TextSegmentCollection<SearchResult> CurrentResults => currentResults;

	public KnownLayer Layer => KnownLayer.Selection;

	public Brush MarkerBrush { get; set; }

	public Pen MarkerPen { get; set; }

	public double MarkerCornerRadius { get; set; }

	public SearchResultBackgroundRenderer()
	{
		MarkerBrush = Brushes.LightGreen;
		MarkerPen = null;
		MarkerCornerRadius = 3.0;
	}

	public void Draw(TextView textView, DrawingContext drawingContext)
	{
		if (textView == null)
		{
			throw new ArgumentNullException("textView");
		}
		if (drawingContext == null)
		{
			throw new ArgumentNullException("drawingContext");
		}
		if (currentResults == null || !textView.VisualLinesValid)
		{
			return;
		}
		ReadOnlyCollection<VisualLine> visualLines = textView.VisualLines;
		if (visualLines.Count == 0)
		{
			return;
		}
		int offset = visualLines.First().FirstDocumentLine.Offset;
		int endOffset = visualLines.Last().LastDocumentLine.EndOffset;
		Brush markerBrush = MarkerBrush;
		Pen markerPen = MarkerPen;
		double markerCornerRadius = MarkerCornerRadius;
		double borderThickness = markerPen?.Thickness ?? 0.0;
		foreach (SearchResult item in currentResults.FindOverlappingSegments(offset, endOffset - offset))
		{
			BackgroundGeometryBuilder backgroundGeometryBuilder = new BackgroundGeometryBuilder();
			backgroundGeometryBuilder.AlignToWholePixels = true;
			backgroundGeometryBuilder.BorderThickness = borderThickness;
			backgroundGeometryBuilder.CornerRadius = markerCornerRadius;
			backgroundGeometryBuilder.AddSegment(textView, item);
			Geometry geometry = backgroundGeometryBuilder.CreateGeometry();
			if (geometry != null)
			{
				drawingContext.DrawGeometry(markerBrush, markerPen, geometry);
			}
		}
	}
}
