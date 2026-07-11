using System;
using System.Globalization;
using System.Windows.Data;
using GMTool.Dot.taiwan_billing;

namespace PvfCode.Converts.GMTool;

public class ConverterTopUpValueType : IValueConverter
{
	private string XhSaaxdIiQ;

	private string sefag4vtqV;

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}
		return (TopUpValueType)value switch
		{
			TopUpValueType.元 => XhSaaxdIiQ, 
			TopUpValueType.点 => sefag4vtqV, 
			_ => "语言包内未找到", 
		};
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		value.ToString();
		if (value.ToString() == XhSaaxdIiQ)
		{
			return TopUpValueType.元;
		}
		if (value.ToString() == sefag4vtqV)
		{
			return TopUpValueType.点;
		}
		return null;
	}

	public ConverterTopUpValueType()
	{
		XhSaaxdIiQ = AppSetting.Instance.GetIlogger()?.GetStr("RechargeOption_ValueType1");
		sefag4vtqV = AppSetting.Instance.GetIlogger()?.GetStr("RechargeOption_ValueType2");
	}
}
