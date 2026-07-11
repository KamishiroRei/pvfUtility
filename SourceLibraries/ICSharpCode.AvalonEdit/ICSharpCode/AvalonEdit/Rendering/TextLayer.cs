using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace ICSharpCode.AvalonEdit.Rendering;

internal sealed class TextLayer : Layer
{
	internal int index;

	private List<VisualLineDrawingVisual> visuals = new List<VisualLineDrawingVisual>();

	protected override int VisualChildrenCount => visuals.Count;

	public TextLayer(TextView textView)
		: base(textView, KnownLayer.Text)
	{
	}

	internal void SetVisualLines(ICollection<VisualLine> visualLines)
	{
		foreach (VisualLineDrawingVisual visual in visuals)
		{
			if (visual.VisualLine.IsDisposed)
			{
				RemoveVisualChild(visual);
			}
		}
		visuals.Clear();
		foreach (VisualLine visualLine in visualLines)
		{
			VisualLineDrawingVisual visualLineDrawingVisual = visualLine.Render();
			if (!visualLineDrawingVisual.IsAdded)
			{
				AddVisualChild(visualLineDrawingVisual);
				visualLineDrawingVisual.IsAdded = true;
			}
			visuals.Add(visualLineDrawingVisual);
		}
		InvalidateArrange();
	}

	protected override Visual GetVisualChild(int index)
	{
		return visuals[index];
	}

	protected override void ArrangeCore(Rect finalRect)
	{
		textView.ArrangeTextLayer(visuals);
	}
}
