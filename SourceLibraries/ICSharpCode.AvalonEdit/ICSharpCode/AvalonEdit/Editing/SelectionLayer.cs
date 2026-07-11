using System;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Editing;

internal sealed class SelectionLayer : Layer, IWeakEventListener
{
	private readonly TextArea textArea;

	public SelectionLayer(TextArea textArea)
		: base(textArea.TextView, KnownLayer.Selection)
	{
		base.IsHitTestVisible = false;
		this.textArea = textArea;
		WeakEventManagerBase<TextViewWeakEventManager.VisualLinesChanged, TextView>.AddListener(textView, this);
		WeakEventManagerBase<TextViewWeakEventManager.ScrollOffsetChanged, TextView>.AddListener(textView, this);
	}

	bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
	{
		if (managerType == typeof(TextViewWeakEventManager.VisualLinesChanged) || managerType == typeof(TextViewWeakEventManager.ScrollOffsetChanged))
		{
			InvalidateVisual();
			return true;
		}
		return false;
	}

	protected override void OnRender(DrawingContext drawingContext)
	{
		base.OnRender(drawingContext);
		Pen selectionBorder = textArea.SelectionBorder;
		BackgroundGeometryBuilder backgroundGeometryBuilder = new BackgroundGeometryBuilder();
		backgroundGeometryBuilder.AlignToWholePixels = true;
		backgroundGeometryBuilder.BorderThickness = selectionBorder?.Thickness ?? 0.0;
		backgroundGeometryBuilder.ExtendToFullWidthAtLineEnd = textArea.Selection.EnableVirtualSpace;
		backgroundGeometryBuilder.CornerRadius = textArea.SelectionCornerRadius;
		foreach (SelectionSegment segment in textArea.Selection.Segments)
		{
			backgroundGeometryBuilder.AddSegment(textView, segment);
		}
		Geometry geometry = backgroundGeometryBuilder.CreateGeometry();
		if (geometry != null)
		{
			drawingContext.DrawGeometry(textArea.SelectionBrush, selectionBorder, geometry);
		}
	}
}
