using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using PvfCode.ViewModels.TreeFolder;

namespace PvfCode.Converts.PvfTree;

public class ConverterTreeListGroupCurrentPathInputVisibility : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		if (System.Convert.ToBoolean(values[1]))
		{
			if (!(values[0] is TreeViewType))
			{
				return Visibility.Collapsed;
			}
			return ((TreeViewType)values[0] != TreeViewType.FileList) ? Visibility.Collapsed : Visibility.Visible;
		}
		return Visibility.Collapsed;
	}

	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
	{
		return null;
	}

	public ConverterTreeListGroupCurrentPathInputVisibility()
	{
	}
}
