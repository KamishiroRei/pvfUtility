using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ICSharpCode.AvalonEdit.Editing;

public static class DottedLineMargin
{
	private static readonly object tag = new object();

	public static UIElement Create()
	{
		return new Line
		{
			X1 = 0.0,
			Y1 = 0.0,
			X2 = 0.0,
			Y2 = 1.0,
			StrokeDashArray = { 0.0, 2.0 },
			Stretch = Stretch.Fill,
			StrokeThickness = 1.0,
			StrokeDashCap = PenLineCap.Round,
			Margin = new Thickness(2.0, 0.0, 2.0, 0.0),
			Tag = tag
		};
	}

	public static bool IsDottedLineMargin(UIElement element)
	{
		if (element is Line line)
		{
			return line.Tag == tag;
		}
		return false;
	}
}
