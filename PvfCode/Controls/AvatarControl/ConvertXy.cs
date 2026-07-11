using System;
using System.Globalization;
using System.Windows.Data;

namespace PvfCode.Controls.AvatarControl;

public class ConvertXy : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return 0.0;
		}
		return double.Parse(value.ToString()) / 2.0;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}

	public ConvertXy()
	{
	}
}
