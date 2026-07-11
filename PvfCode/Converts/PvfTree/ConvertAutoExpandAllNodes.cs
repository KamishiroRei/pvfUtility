using System;
using System.Globalization;
using System.Windows.Data;
using PvfCode.ViewModels.TreeFolder;

namespace PvfCode.Converts.PvfTree;

public class ConvertAutoExpandAllNodes : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return false;
		}
		bool flag = false;
		if ((TreeViewType)value == TreeViewType.SearchResult)
		{
			flag = false;
		}
		return flag;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return null;
	}

	public ConvertAutoExpandAllNodes()
	{
	}
}
