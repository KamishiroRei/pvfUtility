using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

namespace ICSharpCode.AvalonEdit.Utils;

public static class TextFormatterFactory
{
	public static TextFormatter Create(DependencyObject owner)
	{
		if (owner == null)
		{
			throw new ArgumentNullException("owner");
		}
		return TextFormatter.Create(TextOptions.GetTextFormattingMode(owner));
	}

	public static bool PropertyChangeAffectsTextFormatter(DependencyProperty dp)
	{
		return dp == TextOptions.TextFormattingModeProperty;
	}

	public static FormattedText CreateFormattedText(FrameworkElement element, string text, Typeface typeface, double? emSize, Brush foreground)
	{
		if (element == null)
		{
			throw new ArgumentNullException("element");
		}
		if (text == null)
		{
			throw new ArgumentNullException("text");
		}
		if (typeface == null)
		{
			typeface = element.CreateTypeface();
		}
		if (!emSize.HasValue)
		{
			emSize = TextBlock.GetFontSize(element);
		}
		if (foreground == null)
		{
			foreground = TextBlock.GetForeground(element);
		}
		return new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, emSize.Value, foreground, null, TextOptions.GetTextFormattingMode(element), VisualTreeHelper.GetDpi(element).PixelsPerDip);
	}
}
