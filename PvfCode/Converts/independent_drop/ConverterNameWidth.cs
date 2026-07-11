using System;
using System.Globalization;
using System.Windows.Data;

namespace PvfCode.Converts.independent_drop;

public class ConverterNameWidth : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return 100;
		}
		return (double)value * 0.7;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}

	public ConverterNameWidth()
	{
	}
}
