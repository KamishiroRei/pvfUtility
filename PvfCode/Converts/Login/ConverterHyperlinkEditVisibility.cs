using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PvfCode.Converts.Login;

public class ConverterHyperlinkEditVisibility : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return Visibility.Visible;
		}
		if (value.ToString() == parameter.ToString())
		{
			return Visibility.Collapsed;
		}
		return Visibility.Visible;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return null;
	}

	public ConverterHyperlinkEditVisibility()
	{
	}
}
