using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Snippets;

internal sealed class ReplaceableActiveElement : IReplaceableActiveElement, IActiveElement, IWeakEventListener
{
	private sealed class Renderer : IBackgroundRenderer
	{
		private static readonly Brush backgroundBrush = CreateBackgroundBrush();

		private static readonly Pen activeBorderPen = CreateBorderPen();

		internal ReplaceableActiveElement element;

		public KnownLayer Layer { get; set; }

		private static Brush CreateBackgroundBrush()
		{
			SolidColorBrush solidColorBrush = new SolidColorBrush(Colors.LimeGreen);
			solidColorBrush.Opacity = 0.4;
			solidColorBrush.Freeze();
			return solidColorBrush;
		}

		private static Pen CreateBorderPen()
		{
			Pen pen = new Pen(Brushes.Black, 1.0);
			pen.DashStyle = DashStyles.Dot;
			pen.Freeze();
			return pen;
		}

		public void Draw(TextView textView, DrawingContext drawingContext)
		{
			ISegment segment = element.Segment;
			if (segment == null)
			{
				return;
			}
			BackgroundGeometryBuilder backgroundGeometryBuilder = new BackgroundGeometryBuilder();
			backgroundGeometryBuilder.AlignToWholePixels = true;
			backgroundGeometryBuilder.BorderThickness = ((activeBorderPen != null) ? activeBorderPen.Thickness : 0.0);
			if (Layer == KnownLayer.Background)
			{
				backgroundGeometryBuilder.AddSegment(textView, segment);
				drawingContext.DrawGeometry(backgroundBrush, null, backgroundGeometryBuilder.CreateGeometry());
			}
			else
			{
				if (!element.isCaretInside)
				{
					return;
				}
				backgroundGeometryBuilder.AddSegment(textView, segment);
				foreach (BoundActiveElement item in element.context.ActiveElements.OfType<BoundActiveElement>())
				{
					if (item.targetElement == element)
					{
						backgroundGeometryBuilder.AddSegment(textView, item.Segment);
						backgroundGeometryBuilder.CloseFigure();
					}
				}
				drawingContext.DrawGeometry(null, activeBorderPen, backgroundGeometryBuilder.CreateGeometry());
			}
		}
	}

	private readonly InsertionContext context;

	private readonly int startOffset;

	private readonly int endOffset;

	private TextAnchor start;

	private TextAnchor end;

	private bool isCaretInside;

	private Renderer background;

	private Renderer foreground;

	public string Text { get; private set; }

	public bool IsEditable => true;

	public ISegment Segment
	{
		get
		{
			if (start.IsDeleted || end.IsDeleted)
			{
				return null;
			}
			return new SimpleSegment(start.Offset, Math.Max(0, end.Offset - start.Offset));
		}
	}

	public event EventHandler TextChanged;

	public ReplaceableActiveElement(InsertionContext context, int startOffset, int endOffset)
	{
		this.context = context;
		this.startOffset = startOffset;
		this.endOffset = endOffset;
	}

	private void AnchorDeleted(object sender, EventArgs e)
	{
		context.Deactivate(new SnippetEventArgs(DeactivateReason.Deleted));
	}

	public void OnInsertionCompleted()
	{
		start = context.Document.CreateAnchor(startOffset);
		start.MovementType = AnchorMovementType.BeforeInsertion;
		end = context.Document.CreateAnchor(endOffset);
		end.MovementType = AnchorMovementType.AfterInsertion;
		start.Deleted += AnchorDeleted;
		end.Deleted += AnchorDeleted;
		WeakEventManagerBase<TextDocumentWeakEventManager.TextChanged, TextDocument>.AddListener(context.Document, this);
		background = new Renderer
		{
			Layer = KnownLayer.Background,
			element = this
		};
		foreground = new Renderer
		{
			Layer = KnownLayer.Text,
			element = this
		};
		context.TextArea.TextView.BackgroundRenderers.Add(background);
		context.TextArea.TextView.BackgroundRenderers.Add(foreground);
		context.TextArea.Caret.PositionChanged += Caret_PositionChanged;
		Caret_PositionChanged(null, null);
		Text = GetText();
	}

	public void Deactivate(SnippetEventArgs e)
	{
		WeakEventManagerBase<TextDocumentWeakEventManager.TextChanged, TextDocument>.RemoveListener(context.Document, this);
		context.TextArea.TextView.BackgroundRenderers.Remove(background);
		context.TextArea.TextView.BackgroundRenderers.Remove(foreground);
		context.TextArea.Caret.PositionChanged -= Caret_PositionChanged;
	}

	private void Caret_PositionChanged(object sender, EventArgs e)
	{
		ISegment segment = Segment;
		if (segment != null)
		{
			bool flag = segment.Contains(context.TextArea.Caret.Offset, 0);
			if (flag != isCaretInside)
			{
				isCaretInside = flag;
				context.TextArea.TextView.InvalidateLayer(foreground.Layer);
			}
		}
	}

	private string GetText()
	{
		if (start.IsDeleted || end.IsDeleted)
		{
			return string.Empty;
		}
		return context.Document.GetText(start.Offset, Math.Max(0, end.Offset - start.Offset));
	}

	bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
	{
		if (managerType == typeof(TextDocumentWeakEventManager.TextChanged))
		{
			string text = GetText();
			if (Text != text)
			{
				Text = text;
				if (this.TextChanged != null)
				{
					this.TextChanged(this, e);
				}
			}
			return true;
		}
		return false;
	}
}
