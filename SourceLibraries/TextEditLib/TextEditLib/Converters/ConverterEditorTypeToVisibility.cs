using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TextEditLib.Converters;

public class ConverterEditorTypeToVisibility : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return Visibility.Visible;
		}
		TextEditorType textEditorType = (TextEditorType)value;
		if ((uint)textEditorType <= 2u)
		{
			return Visibility.Visible;
		}
		return Visibility.Collapsed;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
