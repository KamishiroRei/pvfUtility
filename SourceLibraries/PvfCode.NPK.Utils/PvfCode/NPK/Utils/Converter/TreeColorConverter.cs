using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PvfCode.NPK.Utils.Converter;

public class TreeColorConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value != null)
		{
			ColorTranslator.FromHtml(value.ToString());
			return new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(value.ToString()));
		}
		return System.Windows.Media.Color.FromArgb(0, 0, 0, 0);
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value != null)
		{
			System.Windows.Media.Color color = (System.Windows.Media.Color)value;
			return ColorTranslator.ToHtml(System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B));
		}
		return "#000000";
	}
}
