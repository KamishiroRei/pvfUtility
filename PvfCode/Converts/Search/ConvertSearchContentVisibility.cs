using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using PvfCode.Services.SearchModel;

namespace PvfCode.Converts.Search;

public class ConvertSearchContentVisibility : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return Visibility.Visible;
		}
		SearchType searchType = (SearchType)value;
		if (parameter.ToString() == "脚本文件搜索内容")
		{
			return (searchType != SearchType.ScriptContent) ? Visibility.Collapsed : Visibility.Visible;
		}
		return (searchType == SearchType.ScriptContent) ? Visibility.Collapsed : Visibility.Visible;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}

	public ConvertSearchContentVisibility()
	{
	}
}
