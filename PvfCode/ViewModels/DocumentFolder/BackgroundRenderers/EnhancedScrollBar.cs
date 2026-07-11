using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using PvfCode.ViewModels.DocumentFolder.TextMarker;

namespace PvfCode.ViewModels.DocumentFolder.BackgroundRenderers;

[TextEditorService]
public class EnhancedScrollBar : IDisposable
{
	private sealed class nwHDuSIs3qkwK6wu6g8 : Adorner
	{
		private static readonly StreamGeometry U5mIb0tdjq;

		private readonly TextEditor editor;

		private readonly TextMarkerService R6HII3uQ7M;

		private static StreamGeometry TeDIL0GyFL()
		{
			StreamGeometry streamGeometry = new StreamGeometry();
			using (StreamGeometryContext streamGeometryContext = streamGeometry.Open())
			{
				streamGeometryContext.BeginFigure(new Point(-2.8145, 3.25), isFilled: true, isClosed: true);
				streamGeometryContext.LineTo(new Point(-2.8145, -3.25), isStroked: true, isSmoothJoin: false);
				streamGeometryContext.LineTo(new Point(2.8145, 0.0), isStroked: true, isSmoothJoin: false);
			}
			((Freezable)streamGeometry).Freeze();
			return streamGeometry;
		}

		public nwHDuSIs3qkwK6wu6g8(EnhancedScrollBar P_0, Grid P_1)
			: base(P_1)
		{
			editor = P_0.editor;
			R6HII3uQ7M = P_0.yZ6GBxjarA;
			base.Cursor = Cursors.Hand;
			base.ToolTip = string.Empty;
			R6HII3uQ7M.RedrawRequested += uVKIqBFsFv;
			editor.TextArea.TextView.VisualLinesChanged += KaYIdtuu34;
		}

		public void eEfIncKYHH()
		{
			R6HII3uQ7M.RedrawRequested -= uVKIqBFsFv;
			editor.TextArea.TextView.VisualLinesChanged -= KaYIdtuu34;
			AdornerLayer.GetAdornerLayer(base.AdornedElement)?.Remove(this);
		}

		private void uVKIqBFsFv(object P_0, EventArgs P_1)
		{
			InvalidateVisual();
		}

		private void KaYIdtuu34(object P_0, EventArgs P_1)
		{
			InvalidateVisual();
		}

		protected override void OnRender(DrawingContext P_0)
		{
			Size renderSize = base.RenderSize;
			TextDocument document = editor.Document;
			TextView textView = editor.TextArea.TextView;
			double documentHeight = textView.DocumentHeight;
			foreach (ITextMarker textMarker in R6HII3uQ7M.TextMarkers)
			{
				_ = textMarker.MarkerTypes;
				if (!xOQIeZ2c0O(textMarker))
				{
					continue;
				}
				double num = textView.GetVisualTopByDocumentLine(document.GetLocation(textMarker.StartOffset).Line) / documentHeight * renderSize.Height;
				Brush brush = ufRGvuupNJ(textMarker.MarkerColor);
				bool flag = false;
				if ((textMarker.MarkerTypes & TextMarkerTypes.LineInScrollBar) != TextMarkerTypes.None)
				{
					P_0.DrawRectangle(brush, null, new Rect(3.0, num - 1.0, renderSize.Width - 6.0, 2.0));
					flag = true;
				}
				if ((textMarker.MarkerTypes & TextMarkerTypes.CircleInScrollBar) != TextMarkerTypes.None)
				{
					P_0.DrawEllipse(brush, null, new Point(renderSize.Width / 2.0, num), 3.0, 3.0);
					flag = true;
				}
				if (!flag)
				{
					TranslateTransform translateTransform = new TranslateTransform(6.0, num);
					((Freezable)translateTransform).Freeze();
					P_0.PushTransform(translateTransform);
					if ((textMarker.MarkerTypes & TextMarkerTypes.ScrollBarLeftTriangle) != TextMarkerTypes.None)
					{
						ScaleTransform scaleTransform = new ScaleTransform(-1.0, 1.0);
						((Freezable)scaleTransform).Freeze();
						P_0.PushTransform(scaleTransform);
						P_0.DrawGeometry(brush, null, U5mIb0tdjq);
						P_0.Pop();
					}
					if ((textMarker.MarkerTypes & TextMarkerTypes.ScrollBarRightTriangle) != TextMarkerTypes.None)
					{
						P_0.DrawGeometry(brush, null, U5mIb0tdjq);
					}
					P_0.Pop();
				}
			}
		}

		private bool xOQIeZ2c0O(ITextMarker P_0)
		{
			return (P_0.MarkerTypes & (TextMarkerTypes.LineInScrollBar | TextMarkerTypes.ScrollBarRightTriangle | TextMarkerTypes.ScrollBarLeftTriangle | TextMarkerTypes.CircleInScrollBar)) != 0;
		}

		protected override void OnMouseDown(MouseButtonEventArgs P_0)
		{
			base.OnMouseDown(P_0);
			ITextMarker textMarker = HFNItWUANI(P_0.GetPosition(this));
			if (textMarker != null)
			{
				TextLocation location = editor.Document.GetLocation(textMarker.StartOffset);
				editor.ScrollTo(location.Line, location.Column);
				P_0.Handled = true;
			}
		}

		private ITextMarker HFNItWUANI(Point P_0)
		{
			Size renderSize = base.RenderSize;
			TextDocument document = editor.Document;
			TextView textView = editor.TextArea.TextView;
			double documentHeight = textView.DocumentHeight;
			ITextMarker result = null;
			double num = double.PositiveInfinity;
			foreach (ITextMarker textMarker in R6HII3uQ7M.TextMarkers)
			{
				if (xOQIeZ2c0O(textMarker))
				{
					double num2 = Math.Abs(textView.GetVisualTopByDocumentLine(document.GetLocation(textMarker.StartOffset).Line) / documentHeight * renderSize.Height - P_0.Y);
					if (num2 < num)
					{
						num = num2;
						result = textMarker;
					}
				}
			}
			return result;
		}

		protected override void OnToolTipOpening(ToolTipEventArgs P_0)
		{
			base.OnToolTipOpening(P_0);
			ITextMarker textMarker = HFNItWUANI(Mouse.GetPosition(this));
			if (textMarker != null && textMarker.ToolTip != null)
			{
				base.ToolTip = textMarker.ToolTip;
			}
			else
			{
				P_0.Handled = true;
			}
		}

		static nwHDuSIs3qkwK6wu6g8()
		{
			U5mIb0tdjq = TeDIL0GyFL();
		}
	}

	private readonly TextEditor editor;

	private readonly TextMarkerService yZ6GBxjarA;

	private nwHDuSIs3qkwK6wu6g8 GSnGFoadNr;

	private bool PyDGrKICnd;

	public EnhancedScrollBar(TextEditor editor, TextMarkerService textMarkerService)
	{
		if (editor == null)
		{
			throw new ArgumentNullException("editor");
		}
		this.editor = editor;
		yZ6GBxjarA = textMarkerService;
		editor.Loaded += gceGhVZs1a;
		if (editor.IsLoaded)
		{
			gceGhVZs1a(null, null);
		}
	}

	public void Dispose()
	{
		editor.Loaded -= gceGhVZs1a;
		if (GSnGFoadNr != null)
		{
			GSnGFoadNr.eEfIncKYHH();
			GSnGFoadNr = null;
		}
	}

	private void gceGhVZs1a(object P_0, RoutedEventArgs P_1)
	{
		if (PyDGrKICnd)
		{
			return;
		}
		PyDGrKICnd = true;
		editor.ApplyTemplate();
		ScrollViewer scrollViewer = (ScrollViewer)editor.Template.FindName("PART_ScrollViewer", editor);
		if (scrollViewer == null)
		{
			return;
		}
		scrollViewer.ApplyTemplate();
		ScrollBar scrollBar = (ScrollBar)scrollViewer.Template.FindName("PART_VerticalScrollBar", scrollViewer);
		if (scrollBar == null)
		{
			return;
		}
		Track track = (Track)scrollBar.Template.FindName("PART_Track", scrollBar);
		if (track != null && VisualTreeHelper.GetParent((DependencyObject)(object)track) is Grid grid)
		{
			AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(grid);
			if (adornerLayer != null)
			{
				GSnGFoadNr = new nwHDuSIs3qkwK6wu6g8(this, grid);
				adornerLayer.Add(GSnGFoadNr);
			}
		}
	}

	private static Brush ufRGvuupNJ(Color P_0)
	{
		P_0 = Colors.Red;
		SolidColorBrush solidColorBrush = new SolidColorBrush(P_0);
		((Freezable)solidColorBrush).Freeze();
		return solidColorBrush;
	}
}
