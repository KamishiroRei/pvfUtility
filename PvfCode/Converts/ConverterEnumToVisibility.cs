using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PvfCode.Converts;

public class ConverterEnumToVisibility : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value != null)
		{
			return (!value.Equals(parameter)) ? Visibility.Collapsed : Visibility.Visible;
		}
		return null;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null || !value.Equals(Visibility.Visible))
		{
			return Binding.DoNothing;
		}
		return parameter;
	}

	public ConverterEnumToVisibility()
	{
	}
}
