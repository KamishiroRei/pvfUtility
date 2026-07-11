using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using PvfCode.ViewModels.independent_drop.Enums;

namespace PvfCode.Converts.independent_drop;

public class Converter_independent_drop_DropLstIdPanelVisibility : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return Visibility.Collapsed;
		}
		if ((DropType)value == DropType.DropFile)
		{
			return Visibility.Visible;
		}
		return Visibility.Collapsed;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}

	public Converter_independent_drop_DropLstIdPanelVisibility()
	{
	}
}
