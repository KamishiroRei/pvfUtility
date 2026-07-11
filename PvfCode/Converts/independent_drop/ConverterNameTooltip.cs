using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Data;

namespace PvfCode.Converts.independent_drop;

public class ConverterNameTooltip : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(2, 2);
		defaultInterpolatedStringHandler.AppendFormatted<object>(value);
		defaultInterpolatedStringHandler.AppendLiteral("\r\n");
		defaultInterpolatedStringHandler.AppendFormatted(AppCore.Logger.GetStr("independent_drop_ConverterNameTooltip"));
		return defaultInterpolatedStringHandler.ToStringAndClear();
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}

	public ConverterNameTooltip()
	{
	}
}
