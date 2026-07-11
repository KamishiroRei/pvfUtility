using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Rendering;
using PvfCode.Controls.TextEditorFolder;

namespace PvfCode.ViewModels.DocumentFolder.BackgroundRenderers;

public class ErrorMarkerService : IBackgroundRenderer, IVisualLineTransformer
{
	private readonly TextEditorBase Editor;

	public KnownLayer Layer => KnownLayer.Selection;

	public ErrorMarkerService(TextEditorBase textEditorBase)
	{
		Editor = textEditorBase;
	}

	public void Draw(TextView textView, DrawingContext drawingContext)
	{
	}

	private IEnumerable<Point> NwhGWLEocD(Point P_0, Point P_1, double P_2, int P_3)
	{
		for (int i = 0; i < P_3; i++)
		{
			yield return new Point(P_0.X + (double)i * P_2, P_0.Y - (((i + 1) % 2 == 0) ? P_2 : 0.0));
		}
	}

	public void Transform(ITextRunConstructionContext context, IList<VisualLineElement> elements)
	{
	}
}
