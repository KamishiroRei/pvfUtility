using System;
using System.Globalization;
using System.Windows.Data;

namespace PvfCode.Converts.AniDesigner;

public class ConverterImageRate : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return 1;
		}
		if (value is float num)
		{
			if (num == 0f)
			{
				return 1;
			}
			return num;
		}
		return 1;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}

	public ConverterImageRate()
	{
	}
}
