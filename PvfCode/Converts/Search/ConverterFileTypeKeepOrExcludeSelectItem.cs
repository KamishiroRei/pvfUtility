using System;
using System.Globalization;
using System.Windows.Data;
using PvfCode.Models.Pvf.Enums;

namespace PvfCode.Converts.Search;

public class ConverterFileTypeKeepOrExcludeSelectItem : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}
		return (RemoveOrKeepFileType)value switch
		{
			RemoveOrKeepFileType.排除 => AppCore.Logger.GetStr("GlobalSearchWindow_FileTypeOption_Exclude"), 
			RemoveOrKeepFileType.保留 => AppCore.Logger.GetStr("GlobalSearchWindow_FileTypeOption_Keep"), 
			_ => AppCore.Logger.GetStr("GlobalSearchWindow_FileTypeOption_Keep"), 
		};
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}
		if (value.ToString() == AppCore.Logger.GetStr("GlobalSearchWindow_FileTypeOption_Keep"))
		{
			return RemoveOrKeepFileType.保留;
		}
		return RemoveOrKeepFileType.排除;
	}

	public ConverterFileTypeKeepOrExcludeSelectItem()
	{
	}
}
