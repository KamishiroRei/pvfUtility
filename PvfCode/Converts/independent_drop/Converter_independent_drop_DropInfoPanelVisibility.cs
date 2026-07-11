using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using PvfCode.ViewModels.independent_drop.Enums;

namespace PvfCode.Converts.independent_drop;

public class Converter_independent_drop_DropInfoPanelVisibility : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return Visibility.Collapsed;
		}
		DropType dropType = (DropType)value;
		if (parameter == null)
		{
			return Visibility.Collapsed;
		}
		if (dropType == DropType.Default && parameter.ToString() == "Default")
		{
			return Visibility.Visible;
		}
		if ((dropType == DropType.List || dropType == DropType.DropFile) && parameter.ToString() == "List")
		{
			return Visibility.Visible;
		}
		return Visibility.Collapsed;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}

	public Converter_independent_drop_DropInfoPanelVisibility()
	{
	}
}
