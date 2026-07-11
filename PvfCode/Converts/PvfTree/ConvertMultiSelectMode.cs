using System;
using System.Globalization;
using System.Windows.Data;
using DevExpress.Xpf.Grid;
using PvfCode.ViewModels.TreeFolder;

namespace PvfCode.Converts.PvfTree;

public class ConvertMultiSelectMode : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		MultiSelectMode multiSelectMode = MultiSelectMode.Row;
		if (value == null)
		{
			return multiSelectMode;
		}
		if ((TreeViewType)value == TreeViewType.SelectFolder)
		{
			multiSelectMode = MultiSelectMode.None;
		}
		return multiSelectMode;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return null;
	}

	public ConvertMultiSelectMode()
	{
	}
}
