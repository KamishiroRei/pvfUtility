using System;
using System.Globalization;
using System.Windows.Data;

namespace PvfCode.Converts.PvfTree;

public class ConvertTreeItemName : IValueConverter
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
		return AppCore.ViewModelBase.PVF.GetItemName(pvfTreeFileBase.FullPath);
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return null;
	}

	public ConvertTreeItemName()
	{
	}
}
