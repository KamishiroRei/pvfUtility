using System;
using System.Globalization;
using System.Windows.Data;
using GMTool.SqlModel.Enums;

namespace PvfCode.Converts.GMTool;

public class ConverterAmplify_optionStyle : IValueConverter
{
	private string 无红字;

	private string 体力;

	private string 精神;

	private string 力量;

	private string 智力;

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}
		return (Amplify_optionStyle)value switch
		{
			Amplify_optionStyle.无红字 => 无红字, 
			Amplify_optionStyle.体力 => 体力, 
			Amplify_optionStyle.精神 => 精神, 
			Amplify_optionStyle.力量 => 力量, 
			Amplify_optionStyle.智力 => 智力, 
			_ => null, 
		};
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		string text = value.ToString();
		if (!(text == "无红字"))
		{
			if (!(text == "体力"))
			{
				if (!(text == "精神"))
				{
					if (!(text == "力量"))
					{
						if (text == "智力")
						{
							return Amplify_optionStyle.智力;
						}
						return null;
					}
					return Amplify_optionStyle.力量;
				}
				return Amplify_optionStyle.精神;
			}
			return Amplify_optionStyle.体力;
		}
		return Amplify_optionStyle.无红字;
	}

	public ConverterAmplify_optionStyle()
	{
		无红字 = AppSetting.Instance.GetIlogger()?.GetStr("Amplify_option_Type_Normal");
		体力 = AppSetting.Instance.GetIlogger()?.GetStr("Amplify_option_Type_Fatigue");
		精神 = AppSetting.Instance.GetIlogger()?.GetStr("Amplify_option_Type_Spirit");
		力量 = AppSetting.Instance.GetIlogger()?.GetStr("Amplify_option_Type_Strength");
		智力 = AppSetting.Instance.GetIlogger()?.GetStr("Amplify_option_Type_Intelligence");
	}
}
