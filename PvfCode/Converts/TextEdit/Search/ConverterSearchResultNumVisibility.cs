using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using PvfCode.ViewModels.DocumentFolder.SearchViewModels.Enums;

namespace PvfCode.Converts.TextEdit.Search;

public class ConverterSearchResultNumVisibility : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return Visibility.Collapsed;
		}
		return ((SourceType)value == SourceType.所有打开的文档) ? Visibility.Collapsed : Visibility.Visible;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}

	public ConverterSearchResultNumVisibility()
	{
	}
}
