using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PvfCode.Converts.PvfTree;

public class ConverterImportFilesTreeRowBack : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}
		PvfTreeFileBase value2 = ((KeyValuePair<string, PvfTreeFileBase>)value).Value;
		if (value2.IsFile && value2.FileIsNull())
		{
			return new SolidColorBrush(Colors.Red);
		}
		return null;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}

	public ConverterImportFilesTreeRowBack()
	{
	}
}
