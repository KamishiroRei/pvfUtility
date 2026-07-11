using System;
using System.Globalization;
using System.Windows.Data;
using GMTool.Dot.taiwan_billing;

namespace PvfCode.Converts.GMTool;

public class ConverterTopUpType : IValueConverter
{
	private string Nefax3Sb6w;

	private string nWBaQ3obkq;

	private string SP;

	private string TP;

	private string QP;

	private string 决斗胜点;

	private string 疲劳值;

	private string 胜利场次;

	private string 失败场次;

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}
		return (TopUpType)value switch
		{
			TopUpType.点券 => Nefax3Sb6w, 
			TopUpType.代币券 => nWBaQ3obkq, 
			TopUpType.SP => SP, 
			TopUpType.TP => TP, 
			TopUpType.QP技能点 => QP, 
			TopUpType.胜点 => 决斗胜点, 
			TopUpType.疲劳值 => 疲劳值, 
			TopUpType.胜利场次 => 胜利场次, 
			TopUpType.失败场次 => 失败场次, 
			_ => "语言包内未找到", 
		};
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value.ToString() == Nefax3Sb6w)
		{
			return TopUpType.点券;
		}
		if (value.ToString() == nWBaQ3obkq)
		{
			return TopUpType.代币券;
		}
		if (value.ToString() == SP)
		{
			return TopUpType.SP;
		}
		if (value.ToString() == SP)
		{
			return TopUpType.SP;
		}
		if (value.ToString() == TP)
		{
			return TopUpType.TP;
		}
		if (value.ToString() == QP)
		{
			return TopUpType.QP技能点;
		}
		if (value.ToString() == 决斗胜点)
		{
			return TopUpType.胜点;
		}
		if (value.ToString() == 疲劳值)
		{
			return TopUpType.疲劳值;
		}
		if (value.ToString() == 胜利场次)
		{
			return TopUpType.胜利场次;
		}
		if (value.ToString() == 失败场次)
		{
			return TopUpType.失败场次;
		}
		return null;
	}

	public ConverterTopUpType()
	{
		Nefax3Sb6w = AppSetting.Instance.GetIlogger()?.GetStr("RechargeOption_Type_cash_cera");
		nWBaQ3obkq = AppSetting.Instance.GetIlogger()?.GetStr("RechargeOption_Type_cash_cera_point");
		SP = AppSetting.Instance.GetIlogger()?.GetStr("RechargeOption_Type_SP");
		TP = AppSetting.Instance.GetIlogger()?.GetStr("RechargeOption_Type_tp");
		QP = AppSetting.Instance.GetIlogger()?.GetStr("RechargeOption_Type_qp");
		决斗胜点 = AppSetting.Instance.GetIlogger()?.GetStr("RechargeOption_Type_duel");
		疲劳值 = AppSetting.Instance.GetIlogger()?.GetStr("RechargeOption_Type_fatigue");
		胜利场次 = AppSetting.Instance.GetIlogger()?.GetStr("RechargeOption_Type_Type_win");
		失败场次 = AppSetting.Instance.GetIlogger()?.GetStr("RechargeOption_Type_lose");
	}
}
