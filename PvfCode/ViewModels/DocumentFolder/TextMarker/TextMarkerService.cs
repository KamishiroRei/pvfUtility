using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace PvfCode.ViewModels.DocumentFolder.TextMarker;

public sealed class TextMarkerService : DocumentColorizingTransformer, IBackgroundRenderer, ITextMarkerService, ITextViewConnect
{
	private TextSegmentCollection<TextMarker> markers;

	private TextDocument document;

	private readonly List<TextView> textViews = new List<TextView>();

	public IEnumerable<ITextMarker> TextMarkers => markers ?? Enumerable.Empty<ITextMarker>();

	public KnownLayer Layer => KnownLayer.Selection;

	public event EventHandler RedrawRequested;

	public TextMarkerService(TextDocument document)
	{
		if (document == null)
		{
			throw new ArgumentNullException("document");
		}
		this.document = document;
		markers = new TextSegmentCollection<TextMarker>(document);
	}

	public ITextMarker Create(int startOffset, int length)
	{
		if (markers == null)
		{
			throw new InvalidOperationException("Cannot create a marker when not attached to a document");
		}
		int textLength = document.TextLength;
		if (startOffset < 0 || startOffset > textLength)
		{
			throw new ArgumentOutOfRangeException("startOffset", startOffset, "Value must be between 0 and " + textLength);
		}
		if (length < 0 || startOffset + length > textLength)
		{
			throw new ArgumentOutOfRangeException("length", length, "length must not be negative and startOffset+length must not be after the end of the document");
		}
		TextMarker marker = new TextMarker(this, startOffset, length);
		markers.Add(marker);
		return marker;
	}

	public IEnumerable<ITextMarker> GetMarkersAtOffset(int offset)
	{
		if (markers == null)
		{
			return Enumerable.Empty<ITextMarker>();
		}
		return markers.FindSegmentsContaining(offset);
	}

	public void RemoveAll(Predicate<ITextMarker> predicate)
	{
		if (predicate == null)
		{
			throw new ArgumentNullException("predicate");
		}
		if (markers == null)
		{
			return;
		}
		foreach (TextMarker marker in markers.ToArray())
		{
			if (predicate(marker))
			{
				Remove(marker);
			}
		}
	}

	public void Remove(ITextMarker marker)
	{
		if (marker == null)
		{
			throw new ArgumentNullException("marker");
		}
		TextMarker textMarker = marker as TextMarker;
		if (markers != null && markers.Remove(textMarker))
		{
			Redraw(textMarker);
			textMarker.OnDeleted();
		}
	}

	internal void Redraw(ISegment segment)
	{
		foreach (TextView view in textViews)
		{
			view.Redraw(segment, DispatcherPriority.Normal);
		}
		RedrawRequested?.Invoke(this, EventArgs.Empty);
	}

	protected override void ColorizeLine(DocumentLine line)
	{
		if (markers == null)
		{
			return;
		}
		int lineStart = line.Offset;
		int lineEnd = lineStart + line.Length;
		foreach (TextMarker marker in markers.FindOverlappingSegments(lineStart, line.Length))
		{
			Brush foregroundBrush = null;
			if (marker.ForegroundColor != null)
			{
				foregroundBrush = new SolidColorBrush(marker.ForegroundColor.Value);
				foregroundBrush.Freeze();
			}
			ChangeLinePart(
				Math.Max(marker.StartOffset, lineStart),
				Math.Min(marker.EndOffset, lineEnd),
				element =>
				{
					if (foregroundBrush != null)
					{
						element.TextRunProperties.SetForegroundBrush(foregroundBrush);
					}
					Typeface typeface = element.TextRunProperties.Typeface;
					element.TextRunProperties.SetTypeface(new Typeface(
						typeface.FontFamily,
						marker.FontStyle ?? typeface.Style,
						marker.FontWeight ?? typeface.Weight,
						typeface.Stretch));
				});
		}
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
		if (markers == null || !textView.VisualLinesValid)
		{
			return;
		}
		ReadOnlyCollection<VisualLine> visualLines = textView.VisualLines;
		if (visualLines.Count == 0)
		{
			return;
		}
		int viewStart = visualLines.First().FirstDocumentLine.Offset;
		int viewEnd = visualLines.Last().LastDocumentLine.EndOffset;
		foreach (TextMarker marker in markers.FindOverlappingSegments(viewStart, viewEnd - viewStart))
		{
			if (marker.BackgroundColor != null)
			{
				BackgroundGeometryBuilder geometryBuilder = new BackgroundGeometryBuilder();
				geometryBuilder.AlignToWholePixels = true;
				geometryBuilder.CornerRadius = 3.0;
				geometryBuilder.AddSegment(textView, marker);
				Geometry geometry = geometryBuilder.CreateGeometry();
				if (geometry != null)
				{
					SolidColorBrush backgroundBrush = new SolidColorBrush(marker.BackgroundColor.Value);
					backgroundBrush.Freeze();
					drawingContext.DrawGeometry(backgroundBrush, null, geometry);
				}
			}
			TextMarkerTypes underlineMarkerTypes = TextMarkerTypes.SquigglyUnderline | TextMarkerTypes.NormalUnderline | TextMarkerTypes.DottedUnderline;
			if ((marker.MarkerTypes & underlineMarkerTypes) == TextMarkerTypes.None)
			{
				continue;
			}
			foreach (Rect rect in BackgroundGeometryBuilder.GetRectsForSegment(textView, marker))
			{
				Point startPoint = rect.BottomLeft;
				Point endPoint = rect.BottomRight;
				Brush usedBrush = new SolidColorBrush(marker.MarkerColor);
				usedBrush.Freeze();
				if ((marker.MarkerTypes & TextMarkerTypes.SquigglyUnderline) != TextMarkerTypes.None)
				{
					double offset = 2.5;
					int count = Math.Max((int)((endPoint.X - startPoint.X) / offset) + 1, 4);
					StreamGeometry geometry = new StreamGeometry();
					using (StreamGeometryContext context = geometry.Open())
					{
						context.BeginFigure(startPoint, isFilled: false, isClosed: false);
						context.PolyLineTo(CreatePoints(startPoint, endPoint, offset, count).ToArray(), isStroked: true, isSmoothJoin: false);
					}
					geometry.Freeze();
					Pen usedPen = new Pen(usedBrush, 1.0);
					usedPen.Freeze();
					drawingContext.DrawGeometry(Brushes.Transparent, usedPen, geometry);
				}
				if ((marker.MarkerTypes & TextMarkerTypes.NormalUnderline) != TextMarkerTypes.None)
				{
					Pen usedPen = new Pen(usedBrush, 1.0);
					usedPen.Freeze();
					drawingContext.DrawLine(usedPen, startPoint, endPoint);
				}
				if ((marker.MarkerTypes & TextMarkerTypes.DottedUnderline) != TextMarkerTypes.None)
				{
					Pen usedPen = new Pen(usedBrush, 1.0);
					usedPen.DashStyle = DashStyles.Dot;
					usedPen.Freeze();
					drawingContext.DrawLine(usedPen, startPoint, endPoint);
				}
			}
		}
	}

	private IEnumerable<Point> CreatePoints(Point start, Point end, double offset, int count)
	{
		for (int i = 0; i < count; i++)
		{
			yield return new Point(start.X + (double)i * offset, start.Y - (((i + 1) % 2 == 0) ? offset : 0.0));
		}
	}

	void ITextViewConnect.AddToTextView(TextView textView)
	{
		if (textView != null && !textViews.Contains(textView))
		{
			textViews.Add(textView);
		}
	}

	void ITextViewConnect.RemoveFromTextView(TextView textView)
	{
		if (textView != null)
		{
			textViews.Remove(textView);
		}
	}
}
