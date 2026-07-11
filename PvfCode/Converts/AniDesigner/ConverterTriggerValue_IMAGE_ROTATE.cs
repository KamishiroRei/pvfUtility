using System;
using System.Globalization;
using System.Windows.Data;

namespace PvfCode.Converts.AniDesigner;

public class ConverterTriggerValue_IMAGE_ROTATE : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return 0f;
		}
		if (value is float)
		{
			return value;
		}
		return 0f;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}

	public ConverterTriggerValue_IMAGE_ROTATE()
	{
	}
}
