using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace ViewModels.DocumentFolder.OffsetColorizers;

public class LinkHoverStyle : DocumentColorizingTransformer
{
	public readonly TextSegment TextSeg;

	public LinkHoverStyle(TextSegment textSeg)
	{
		TextSeg = textSeg;
	}

	protected override void ColorizeLine(DocumentLine line)
	{
		if (line.Offset <= TextSeg.StartOffset && line.EndOffset >= TextSeg.EndOffset)
		{
			ChangeLinePart(TextSeg.StartOffset, TextSeg.EndOffset, wbswNHRuV);
		}
	}

	private void wbswNHRuV(VisualLineElement P_0)
	{
		SolidColorBrush solidColorBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5191c5"));
		((Freezable)solidColorBrush).Freeze();
		P_0.TextRunProperties.SetForegroundBrush(solidColorBrush);
		P_0.TextRunProperties.SetTextDecorations(TextDecorations.Underline);
	}
}
