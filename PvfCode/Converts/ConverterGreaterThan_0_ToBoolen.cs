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

	public object ConvertBack(object P_0, Type P_1, object P_2, CultureInfo P_3)
	{
		throw new NotImplementedException();
	}

	public ConverterGreaterThan_0_ToBoolen()
	{
	}
}
