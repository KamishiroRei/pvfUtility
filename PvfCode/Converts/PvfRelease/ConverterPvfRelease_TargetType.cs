using System;
using System.Globalization;
using System.Windows.Data;
using PvfCode.Models.Pvf.Enums;

namespace PvfCode.Converts.PvfRelease;

public class ConverterPvfRelease_TargetType : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}
		return (PvfReleaseType)value switch
		{
			PvfReleaseType.客户端 => AppCore.Logger.GetStr("ViewPvfRelease_TargetType_Client"), 
			PvfReleaseType.服务端 => AppCore.Logger.GetStr("ViewPvfRelease_TargetType_Server"), 
			_ => AppCore.Logger.GetStr("ViewPvfRelease_TargetType_Server"), 
		};
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}
		if (value.ToString() == AppCore.Logger.GetStr("ViewPvfRelease_TargetType_Client"))
		{
			return PvfReleaseType.客户端;
		}
		return PvfReleaseType.服务端;
	}

	public ConverterPvfRelease_TargetType()
	{
	}
}
