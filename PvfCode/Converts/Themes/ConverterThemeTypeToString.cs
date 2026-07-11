using System;
using System.Globalization;
using System.Windows.Data;
using PvfCode.Models.Enums;

namespace PvfCode.Converts.Themes;

public class ConverterThemeTypeToString : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}
		return (ThemeType)value switch
		{
			ThemeType.VS2019Blue => AppCore.Logger.GetStr("ViewGlobalOptions_Label_ColorTheme_Blue"), 
			ThemeType.VS2019Dark => AppCore.Logger.GetStr("ViewGlobalOptions_Label_ColorTheme_Dark"), 
			_ => AppCore.Logger.GetStr("ViewGlobalOptions_Label_ColorTheme_Light"), 
		};
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}
		if (value.ToString() == AppCore.Logger.GetStr("ViewGlobalOptions_Label_ColorTheme_Blue"))
		{
			return ThemeType.VS2019Blue;
		}
		if (value.ToString() == AppCore.Logger.GetStr("ViewGlobalOptions_Label_ColorTheme_Dark"))
		{
			return ThemeType.VS2019Dark;
		}
		return ThemeType.VS2019Light;
	}

	public ConverterThemeTypeToString()
	{
	}
}
