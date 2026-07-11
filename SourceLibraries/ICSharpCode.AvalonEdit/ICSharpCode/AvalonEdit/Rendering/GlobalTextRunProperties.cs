using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

namespace ICSharpCode.AvalonEdit.Rendering;

internal sealed class GlobalTextRunProperties : TextRunProperties
{
	internal Typeface typeface;

	internal double fontRenderingEmSize;

	internal Brush foregroundBrush;

	internal Brush backgroundBrush;

	internal CultureInfo cultureInfo;

	public override Typeface Typeface => typeface;

	public override double FontRenderingEmSize => fontRenderingEmSize;

	public override double FontHintingEmSize => fontRenderingEmSize;

	public override TextDecorationCollection TextDecorations => null;

	public override Brush ForegroundBrush => foregroundBrush;

	public override Brush BackgroundBrush => backgroundBrush;

	public override CultureInfo CultureInfo => cultureInfo;

	public override TextEffectCollection TextEffects => null;
}
