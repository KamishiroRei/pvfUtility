using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PvfCode.Converts.PvfTree;

public class ConverterImportFileIsNewFile : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		Visibility visibility = Visibility.Collapsed;
		PvfTreeFileBase pvfTreeFileBase = (PvfTreeFileBase)value;
		if (!pvfTreeFileBase.IsFile)
		{
			return visibility;
		}
		PvfGroup pVF = AppCore.ViewModelBase.PVF;
		if (pVF.PvfIsOpen)
		{
			visibility = (pVF.FileAny(pvfTreeFileBase.FullPath) ? Visibility.Collapsed : Visibility.Visible);
		}
		return visibility;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return null;
	}

	public ConverterImportFileIsNewFile()
	{
	}
}
