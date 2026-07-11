using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PvfCode.Controls.AvatarControl;

public class ConvertCenter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}
		double num = double.Parse(value.ToString()) / 2.0;
		return (object)new Point(num, num);
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return null;
	}

	public ConvertCenter()
	{
	}
}
