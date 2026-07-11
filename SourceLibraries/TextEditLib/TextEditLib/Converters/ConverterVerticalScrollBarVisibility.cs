using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TextEditLib.Converters;

public class ConverterVerticalScrollBarVisibility : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return Visibility.Visible;
		}
		_ = (TextEditorType)value;
		return Visibility.Visible;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
