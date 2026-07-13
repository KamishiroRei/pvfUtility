using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PvfCode.Converts;

public class ConvertBoolVisibility : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return (Visibility)value switch
		{
			Visibility.Visible => true, 
			Visibility.Hidden => false, 
			Visibility.Collapsed => false, 
			_ => false, 
		};
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if ((bool)value)
		{
			return Visibility.Visible;
		}
		return Visibility.Collapsed;
	}

	public ConvertBoolVisibility()
	{
	}
}
