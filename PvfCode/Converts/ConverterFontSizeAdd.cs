using System;
using System.Globalization;
using System.Windows.Data;

namespace PvfCode.Converts;

public class ConverterFontSizeAdd : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null || parameter == null)
		{
			return null;
		}
		if (double.TryParse(parameter.ToString(), out var result))
		{
			if (!double.TryParse(value.ToString(), out var result2))
			{
				return null;
			}
			return result2 + result;
		}
		return null;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}

	public ConverterFontSizeAdd()
	{
	}
}
