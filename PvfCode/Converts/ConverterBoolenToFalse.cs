using System;
using System.Globalization;
using System.Windows.Data;

namespace PvfCode.Converts;

public class ConverterBoolenToFalse : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return true;
		}
		return !(bool)value;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return null;
	}

	public ConverterBoolenToFalse()
	{
	}
}
