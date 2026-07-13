using System;
using System.Globalization;
using System.Windows.Data;

namespace PvfCode.Converts;

public class ConverterGreaterThan_0_ToBoolen : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return false;
		}
		if (value is int num && num > 0)
		{
			return true;
		}
		return false;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}

	public ConverterGreaterThan_0_ToBoolen()
	{
	}
}
