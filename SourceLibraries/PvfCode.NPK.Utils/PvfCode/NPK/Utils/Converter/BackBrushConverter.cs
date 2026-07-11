using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PvfCode.NPK.Utils.Converter;

public class BackBrushConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value != null)
		{
			return new SolidColorBrush((Color)ColorConverter.ConvertFromString(value.ToString()));
		}
		return new SolidColorBrush(Colors.Gray);
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return value;
	}
}
