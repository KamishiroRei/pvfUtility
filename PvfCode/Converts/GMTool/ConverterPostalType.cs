using System;
using System.Globalization;
using System.Windows.Data;
using GMTool.SqlModel.Enums;

namespace PvfCode.Converts.GMTool;

public class ConverterPostalType : IValueConverter
{
	private string 普通邮件;

	private string 时装邮件;

	private string 宠物;

	private string 宠物蛋;

	private string 点券;

	private string 代币券;

	private string 金币;

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}
		return (PostalType)value switch
		{
			PostalType.普通邮件 => 普通邮件, 
			PostalType.时装邮件 => 时装邮件, 
			PostalType.宠物 => 宠物, 
			PostalType.宠物蛋 => 宠物蛋, 
			PostalType.点券 => 点券, 
			PostalType.代币券 => 代币券, 
			PostalType.金币 => 金币, 
			_ => null, 
		};
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		string text = value.ToString();
		if (text != null)
		{
			switch (text.Length)
			{
			case 4:
				switch (text[0])
				{
				case '普':
					if (!(text == "普通邮件"))
					{
						break;
					}
					return PostalType.普通邮件;
				case '时':
					if (!(text == "时装邮件"))
					{
						break;
					}
					return PostalType.时装邮件;
				}
				break;
			case 2:
				switch (text[0])
				{
				case '宠':
					if (!(text == "宠物"))
					{
						break;
					}
					return PostalType.宠物;
				case '点':
					if (!(text == "点券"))
					{
						break;
					}
					return PostalType.点券;
				case '金':
					if (!(text == "金币"))
					{
						break;
					}
					return PostalType.金币;
				}
				break;
			case 3:
				switch (text[0])
				{
				case '宠':
					if (!(text == "宠物蛋"))
					{
						break;
					}
					return PostalType.宠物蛋;
				case '代':
					if (!(text == "代币券"))
					{
						break;
					}
					return PostalType.代币券;
				}
				break;
			}
		}
		return null;
	}

	public ConverterPostalType()
	{
		普通邮件 = AppSetting.Instance.GetIlogger()?.GetStr("PostalType_Normal");
		时装邮件 = AppSetting.Instance.GetIlogger()?.GetStr("PostalType_Fashion");
		宠物 = AppSetting.Instance.GetIlogger()?.GetStr("PostalType_Pet");
		宠物蛋 = AppSetting.Instance.GetIlogger()?.GetStr("PostalType_PetEgg");
		点券 = AppSetting.Instance.GetIlogger()?.GetStr("PostalType_Cera");
		代币券 = AppSetting.Instance.GetIlogger()?.GetStr("PostalType_CeraPoint");
		金币 = AppSetting.Instance.GetIlogger()?.GetStr("PostalType_Gold");
	}
}
