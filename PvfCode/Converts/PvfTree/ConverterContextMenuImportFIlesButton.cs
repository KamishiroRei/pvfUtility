using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace PvfCode.Converts.PvfTree;

public class ConverterContextMenuImportFIlesButton : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}
		PvfTreeFileBase pvfTreeFileBase = (PvfTreeFileBase)value;
		if (pvfTreeFileBase.IsFile)
		{
			return Path.GetDirectoryName(pvfTreeFileBase.FullPath)?.Replace('\\', '/');
		}
		return pvfTreeFileBase.FullPath;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return null;
	}

	public ConverterContextMenuImportFIlesButton()
	{
	}
}
