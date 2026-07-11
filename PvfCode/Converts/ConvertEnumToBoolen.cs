using System;
using System.Globalization;
using System.Windows.Data;

namespace PvfCode.Converts;

public class ConvertEnumToBoolen : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value != null)
		{
			return value.Equals(parameter);
		}
		return null;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null || !value.Equals(true))
		{
			return Binding.DoNothing;
		}
		return parameter;
	}

	public ConvertEnumToBoolen()
	{
	}
}
