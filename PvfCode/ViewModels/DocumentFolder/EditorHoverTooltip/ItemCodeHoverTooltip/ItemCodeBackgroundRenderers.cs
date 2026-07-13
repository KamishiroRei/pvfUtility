using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace PvfCode.ViewModels.DocumentFolder.EditorHoverTooltip.ItemCodeHoverTooltip;

public class ItemCodeBackgroundRenderers : IBackgroundRenderer
{
	public TextSegment? ItemTextSegment { get; set; }

	public KnownLayer Layer => KnownLayer.Selection;

	public void Draw(TextView textView, DrawingContext drawingContext)
	{
		if (ItemTextSegment == null)
		{
			return;
		}
		SolidColorBrush highlightBrush = new SolidColorBrush(Color.FromArgb(64, 0, 0, byte.MaxValue));
		((Freezable)highlightBrush).Freeze();
		foreach (Rect segmentRect in BackgroundGeometryBuilder.GetRectsForSegment(textView, ItemTextSegment))
		{
			((Freezable)highlightBrush).Freeze();
			drawingContext.DrawRectangle(highlightBrush, null, new Rect(segmentRect.Location, new Size(segmentRect.Width, segmentRect.Height)));
		}
	}

	public ItemCodeBackgroundRenderers()
	{
	}
}
