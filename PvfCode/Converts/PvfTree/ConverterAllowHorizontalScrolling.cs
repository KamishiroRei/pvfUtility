using System;
using System.Globalization;
using System.Windows.Data;
using DevExpress.Xpf.Grid;

namespace PvfCode.Converts.PvfTree;

public class ConverterAllowHorizontalScrolling : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return new GridColumnWidth(1000.0, GridColumnUnitType.Pixel);
		}
		if (!System.Convert.ToBoolean(value))
		{
			return new GridColumnWidth(1000.0, GridColumnUnitType.Pixel);
		}
		return "Auto";
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return value;
	}

	public ConverterAllowHorizontalScrolling()
	{
	}
}
