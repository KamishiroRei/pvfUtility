using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace PvfCode.ViewModels.DocumentFolder.EditorHoverTooltip.ItemCodeHoverTooltip;

public class ItemCodeBackgroundRenderers : IBackgroundRenderer
{
	[CompilerGenerated]
	private TextSegment? buZihmdlXi;

	public TextSegment? ItemTextSegment
	{
		[CompilerGenerated]
		get
		{
			return buZihmdlXi;
		}
		[CompilerGenerated]
		set
		{
			buZihmdlXi = value;
		}
	}

	public KnownLayer Layer => KnownLayer.Selection;

	public void Draw(TextView textView, DrawingContext drawingContext)
	{
		if (ItemTextSegment == null)
		{
			return;
		}
		SolidColorBrush solidColorBrush = new SolidColorBrush(Color.FromArgb(64, 0, 0, byte.MaxValue));
		((Freezable)solidColorBrush).Freeze();
		foreach (Rect item in BackgroundGeometryBuilder.GetRectsForSegment(textView, ItemTextSegment))
		{
			Rect current = item;
			((Freezable)solidColorBrush).Freeze();
			drawingContext.DrawRectangle(solidColorBrush, null, new Rect(current.Location, new Size(current.Width, current.Height)));
		}
	}

	public ItemCodeBackgroundRenderers()
	{
	}
}
