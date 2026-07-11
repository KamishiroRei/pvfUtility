using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

namespace ICSharpCode.AvalonEdit.Rendering;

internal sealed class VisualLineDrawingVisual : DrawingVisual
{
	public readonly VisualLine VisualLine;

	public readonly double Height;

	internal bool IsAdded;

	public VisualLineDrawingVisual(VisualLine visualLine, FlowDirection flow)
	{
		VisualLine = visualLine;
		DrawingContext drawingContext = RenderOpen();
		double num = 0.0;
		foreach (TextLine textLine in visualLine.TextLines)
		{
			if (flow == FlowDirection.LeftToRight)
			{
				textLine.Draw(drawingContext, new Point(0.0, num), InvertAxes.None);
			}
			else
			{
				textLine.Draw(drawingContext, new Point(0.0, num), InvertAxes.Horizontal);
			}
			num += textLine.Height;
		}
		Height = num;
		drawingContext.Close();
	}

	protected override GeometryHitTestResult HitTestCore(GeometryHitTestParameters hitTestParameters)
	{
		return null;
	}

	protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
	{
		return null;
	}
}
