using System;
using System.Globalization;
using System.Windows.Data;

namespace PvfCode.Converts.PvfTree;

public class ConvertTreeItemCode : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}
		PvfTreeFileBase pvfTreeFileBase = (PvfTreeFileBase)value;
		if (!pvfTreeFileBase.IsFile)
		{
			return null;
		}
		PvfFile file = AppCore.ViewModelBase.PVF.GetFile(pvfTreeFileBase.FullPath);
		if (file == null)
		{
			return null;
		}
		if (!file.ItemCode.HasValue)
		{
			return null;
		}
		return "(" + file.ItemCode + ")";
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return null;
	}

	public ConvertTreeItemCode()
	{
	}
}
