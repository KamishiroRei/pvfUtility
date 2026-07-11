using System;
using System.Globalization;
using System.Windows.Data;

namespace PvfCode.Converts.TextEdit;

public class ConvertEditorTopBarPathWidth : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}
		if (value is double num)
		{
			return num - 30.0;
		}
		return null;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}

	public ConvertEditorTopBarPathWidth()
	{
	}
}
