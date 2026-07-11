using System;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Rendering;

namespace ICSharpCode.AvalonEdit.Highlighting;

[Serializable]
public abstract class HighlightingBrush
{
	public abstract Brush GetBrush(ITextRunConstructionContext context);

	public virtual Color? GetColor(ITextRunConstructionContext context)
	{
		if (GetBrush(context) is SolidColorBrush solidColorBrush)
		{
			return solidColorBrush.Color;
		}
		return null;
	}
}
