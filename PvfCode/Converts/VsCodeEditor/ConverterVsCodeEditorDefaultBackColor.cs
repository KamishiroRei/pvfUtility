using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Data;
using PvfCode.Models.Enums;

namespace PvfCode.Converts.VsCodeEditor;

public class ConverterVsCodeEditorDefaultBackColor : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return (ThemeType)Enum.Parse(typeof(ThemeType), value.ToString()) switch
		{
			ThemeType.VS2019Blue => Color.FromName("#d6dbe9"), 
			ThemeType.VS2019Dark => Color.FromName("#1e1e1e"), 
			ThemeType.VS2019Light => Color.FromName("#eeeef2"), 
			_ => null, 
		};
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}

	public ConverterVsCodeEditorDefaultBackColor()
	{
	}
}
