using System;
using System.Globalization;
using System.Windows.Data;
using PvfCode;
using PvfCode.Services.SearchModel.Enums;

namespace Converts.Search;

internal class ConvertScriptContentSearchMode : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}
		return (ScriptContentSearchMode)value switch
		{
			ScriptContentSearchMode.二进制 => AppCore.Logger.GetStr("GlobalSearchWindow_ScriptFileSearchTypeComboBoxItem1"), 
			ScriptContentSearchMode.基于文本 => AppCore.Logger.GetStr("GlobalSearchWindow_ScriptFileSearchTypeComboBoxItem2"), 
			_ => AppCore.Logger.GetStr("GlobalSearchWindow_ScriptFileSearchTypeComboBoxItem2"), 
		};
	}

	public object ConvertBack(object P_0, Type P_1, object P_2, CultureInfo P_3)
	{
		if (P_0 == null)
		{
			return null;
		}
		if (P_0.ToString().Contains(AppCore.Logger.GetStr("GlobalSearchWindow_ScriptFileSearchTypeComboBoxItem1")))
		{
			return ScriptContentSearchMode.二进制;
		}
		return ScriptContentSearchMode.基于文本;
	}

	public ConvertScriptContentSearchMode()
	{
	}
}
