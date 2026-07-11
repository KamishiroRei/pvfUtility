using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace PvfCode.ViewModels.DocumentFolder.TextMarker;

public sealed class TextMarkerService : DocumentColorizingTransformer, IBackgroundRenderer, ITextMarkerService, ITextViewConnect
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass13_0
	{
		public TextMarker FbtesiR3CN;

		public Brush ofxeLjORFd;

		public _003C_003Ec__DisplayClass13_0()
		{
		}

		internal void K02eoVW0QX(VisualLineElement element)
		{
			if (ofxeLjORFd != null)
			{
				element.TextRunProperties.SetForegroundBrush(ofxeLjORFd);
			}
			Typeface typeface = element.TextRunProperties.Typeface;
			element.TextRunProperties.SetTypeface(new Typeface(typeface.FontFamily, FbtesiR3CN.FontStyle ?? typeface.Style, FbtesiR3CN.FontWeight ?? typeface.Weight, typeface.Stretch));
		}
	}

	private TextSegmentCollection<TextMarker> pk7SDtNHUV;

	private TextDocument rW7Sl4RsmB;

	[CompilerGenerated]
	private EventHandler gwBSjUIJYY;

	private readonly List<TextView> gBFSTRc9V6;

	public IEnumerable<ITextMarker> TextMarkers
	{
		get
		{
			IEnumerable<ITextMarker> enumerable = pk7SDtNHUV;
			return enumerable ?? Enumerable.Empty<ITextMarker>();
		}
	}

	public KnownLayer Layer => KnownLayer.Selection;

	public event EventHandler RedrawRequested
	{
		[CompilerGenerated]
		add
		{
			EventHandler eventHandler = gwBSjUIJYY;
			EventHandler eventHandler2;
			do
			{
				eventHandler2 = eventHandler;
				EventHandler value2 = (EventHandler)Delegate.Combine(eventHandler2, value);
				eventHandler = Interlocked.CompareExchange(ref gwBSjUIJYY, value2, eventHandler2);
			}
			while ((object)eventHandler != eventHandler2);
		}
		[CompilerGenerated]
		remove
		{
			EventHandler eventHandler = gwBSjUIJYY;
			EventHandler eventHandler2;
			do
			{
				eventHandler2 = eventHandler;
				EventHandler value2 = (EventHandler)Delegate.Remove(eventHandler2, value);
				eventHandler = Interlocked.CompareExchange(ref gwBSjUIJYY, value2, eventHandler2);
			}
			while ((object)eventHandler != eventHandler2);
		}
	}

	public TextMarkerService(TextDocument document)
	{
		gBFSTRc9V6 = new List<TextView>();
		if (document == null)
		{
			throw new ArgumentNullException("document");
		}
		rW7Sl4RsmB = document;
		pk7SDtNHUV = new TextSegmentCollection<TextMarker>(document);
	}

	public ITextMarker Create(int startOffset, int length)
	{
		if (pk7SDtNHUV == null)
		{
			throw new InvalidOperationException("Cannot create a marker when not attached to a document");
		}
		int textLength = rW7Sl4RsmB.TextLength;
		if (startOffset < 0 || startOffset > textLength)
		{
			throw new ArgumentOutOfRangeException("startOffset", startOffset, "Value must be between 0 and " + textLength);
		}
		if (length < 0 || startOffset + length > textLength)
		{
			throw new ArgumentOutOfRangeException("length", length, "length must not be negative and startOffset+length must not be after the end of the document");
		}
		TextMarker textMarker = new TextMarker(this, startOffset, length);
		pk7SDtNHUV.Add(textMarker);
		return textMarker;
	}

	public IEnumerable<ITextMarker> GetMarkersAtOffset(int offset)
	{
		if (pk7SDtNHUV == null)
		{
			return Enumerable.Empty<ITextMarker>();
		}
		return pk7SDtNHUV.FindSegmentsContaining(offset);
	}

	public void RemoveAll(Predicate<ITextMarker> predicate)
	{
		if (predicate == null)
		{
			throw new ArgumentNullException("predicate");
		}
		if (pk7SDtNHUV == null)
		{
			return;
		}
		TextMarker[] array = pk7SDtNHUV.ToArray();
		foreach (TextMarker textMarker in array)
		{
			if (predicate(textMarker))
			{
				Remove(textMarker);
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
		if (pk7SDtNHUV != null && pk7SDtNHUV.Remove(textMarker))
		{
			Ymo5NGv8tI(textMarker);
			textMarker.sXFSCtUS7y();
		}
	}

	internal void Ymo5NGv8tI(ISegment P_0)
	{
		foreach (TextView item in gBFSTRc9V6)
		{
			item.Redraw(P_0, (DispatcherPriority)9);
		}
		if (gwBSjUIJYY != null)
		{
			gwBSjUIJYY(this, EventArgs.Empty);
		}
	}

	protected override void ColorizeLine(DocumentLine line)
	{
		if (pk7SDtNHUV == null)
		{
			return;
		}
		int offset = line.Offset;
		int val = offset + line.Length;
		using IEnumerator<TextMarker> enumerator = pk7SDtNHUV.FindOverlappingSegments(offset, line.Length).GetEnumerator();
		while (enumerator.MoveNext())
		{
			_003C_003Ec__DisplayClass13_0 CS_0024_003C_003E8__locals12 = new _003C_003Ec__DisplayClass13_0();
			CS_0024_003C_003E8__locals12.FbtesiR3CN = enumerator.Current;
			CS_0024_003C_003E8__locals12.ofxeLjORFd = null;
			if (CS_0024_003C_003E8__locals12.FbtesiR3CN.ForegroundColor.HasValue)
			{
				CS_0024_003C_003E8__locals12.ofxeLjORFd = new SolidColorBrush(CS_0024_003C_003E8__locals12.FbtesiR3CN.ForegroundColor.Value);
				((Freezable)CS_0024_003C_003E8__locals12.ofxeLjORFd).Freeze();
			}
			ChangeLinePart(Math.Max(CS_0024_003C_003E8__locals12.FbtesiR3CN.StartOffset, offset), Math.Min(CS_0024_003C_003E8__locals12.FbtesiR3CN.EndOffset, val), delegate(VisualLineElement element)
			{
				if (CS_0024_003C_003E8__locals12.ofxeLjORFd != null)
				{
					element.TextRunProperties.SetForegroundBrush(CS_0024_003C_003E8__locals12.ofxeLjORFd);
				}
				Typeface typeface = element.TextRunProperties.Typeface;
				element.TextRunProperties.SetTypeface(new Typeface(typeface.FontFamily, CS_0024_003C_003E8__locals12.FbtesiR3CN.FontStyle ?? typeface.Style, CS_0024_003C_003E8__locals12.FbtesiR3CN.FontWeight ?? typeface.Weight, typeface.Stretch));
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
		if (pk7SDtNHUV == null || !textView.VisualLinesValid)
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
		foreach (TextMarker item in pk7SDtNHUV.FindOverlappingSegments(offset, endOffset - offset))
		{
			if (item.BackgroundColor.HasValue)
			{
				BackgroundGeometryBuilder backgroundGeometryBuilder = new BackgroundGeometryBuilder();
				backgroundGeometryBuilder.AlignToWholePixels = true;
				backgroundGeometryBuilder.CornerRadius = 3.0;
				backgroundGeometryBuilder.AddSegment(textView, item);
				Geometry geometry = backgroundGeometryBuilder.CreateGeometry();
				if (geometry != null)
				{
					SolidColorBrush solidColorBrush = new SolidColorBrush(item.BackgroundColor.Value);
					((Freezable)solidColorBrush).Freeze();
					drawingContext.DrawGeometry(solidColorBrush, null, geometry);
				}
			}
			TextMarkerTypes textMarkerTypes = TextMarkerTypes.SquigglyUnderline | TextMarkerTypes.NormalUnderline | TextMarkerTypes.DottedUnderline;
			if ((item.MarkerTypes & textMarkerTypes) == 0)
			{
				continue;
			}
			foreach (Rect item2 in BackgroundGeometryBuilder.GetRectsForSegment(textView, item))
			{
				Rect current2 = item2;
				Point bottomLeft = current2.BottomLeft;
				Point bottomRight = current2.BottomRight;
				Brush brush = new SolidColorBrush(item.MarkerColor);
				((Freezable)brush).Freeze();
				if ((item.MarkerTypes & TextMarkerTypes.SquigglyUnderline) != TextMarkerTypes.None)
				{
					double num = 2.5;
					int num2 = Math.Max((int)((bottomRight.X - bottomLeft.X) / num) + 1, 4);
					StreamGeometry streamGeometry = new StreamGeometry();
					using (StreamGeometryContext streamGeometryContext = streamGeometry.Open())
					{
						streamGeometryContext.BeginFigure(bottomLeft, isFilled: false, isClosed: false);
						streamGeometryContext.PolyLineTo(NVy5z4GuVq(bottomLeft, bottomRight, num, num2).ToArray(), isStroked: true, isSmoothJoin: false);
					}
					((Freezable)streamGeometry).Freeze();
					Pen pen = new Pen(brush, 1.0);
					((Freezable)pen).Freeze();
					drawingContext.DrawGeometry(Brushes.Transparent, pen, streamGeometry);
				}
				if ((item.MarkerTypes & TextMarkerTypes.NormalUnderline) != TextMarkerTypes.None)
				{
					Pen pen2 = new Pen(brush, 1.0);
					((Freezable)pen2).Freeze();
					drawingContext.DrawLine(pen2, bottomLeft, bottomRight);
				}
				if ((item.MarkerTypes & TextMarkerTypes.DottedUnderline) != TextMarkerTypes.None)
				{
					Pen pen3 = new Pen(brush, 1.0);
					pen3.DashStyle = DashStyles.Dot;
					((Freezable)pen3).Freeze();
					drawingContext.DrawLine(pen3, bottomLeft, bottomRight);
				}
			}
		}
	}

	private IEnumerable<Point> NVy5z4GuVq(Point P_0, Point P_1, double P_2, int P_3)
	{
		for (int i = 0; i < P_3; i++)
		{
			yield return new Point(P_0.X + (double)i * P_2, P_0.Y - (((i + 1) % 2 == 0) ? P_2 : 0.0));
		}
	}

	void ITextViewConnect.AddToTextView(TextView textView)
	{
		if (textView != null && !gBFSTRc9V6.Contains(textView))
		{
			gBFSTRc9V6.Add(textView);
		}
	}

	void ITextViewConnect.RemoveFromTextView(TextView textView)
	{
		if (textView != null)
		{
			gBFSTRc9V6.Remove(textView);
		}
	}
}
