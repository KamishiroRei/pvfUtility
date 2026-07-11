using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PvfCode.Converts;

public class ConvertEnumToVisibility : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return (value == null) ? Visibility.Collapsed : ((!value.Equals(parameter)) ? Visibility.Collapsed : Visibility.Visible);
	}

	public object ConvertBack(object P_0, Type P_1, object P_2, CultureInfo P_3)
	{
		if (P_0 == null || !P_0.Equals(Visibility.Visible))
		{
			return Binding.DoNothing;
		}
		return P_2;
	}

	public ConvertEnumToVisibility()
	{
	}
}
