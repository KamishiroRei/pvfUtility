using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PvfCode.Converts.WindowAddCodeCompletionData;

public class ConverterWindowAddCodeCompletionDataShareLabelVisibility : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return Visibility.Collapsed;
		}
		if (System.Convert.ToBoolean(value) && !ServiceCloud.Instance.IsLogin)
		{
			return Visibility.Visible;
		}
		return Visibility.Collapsed;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}

	public ConverterWindowAddCodeCompletionDataShareLabelVisibility()
	{
	}
}
