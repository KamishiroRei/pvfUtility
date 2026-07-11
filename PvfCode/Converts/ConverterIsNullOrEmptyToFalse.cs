using System;
using System.Globalization;
using System.Windows.Data;

namespace PvfCode.Converts;

public class ConverterIsNullOrEmptyToFalse : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return false;
		}
		return !string.IsNullOrEmpty(value.ToString());
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return null;
	}

	public ConverterIsNullOrEmptyToFalse()
	{
	}
}
