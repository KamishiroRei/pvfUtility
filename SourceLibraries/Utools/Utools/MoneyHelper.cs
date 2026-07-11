using System;
using System.Text.RegularExpressions;

namespace Utools;

public static class MoneyHelper
{
	public class CalculationIncomeMoenyData
	{
		private decimal hmwMqToS7d;

		private decimal OPfMjO5Mpv;

		public decimal IncomeMoeny
		{
			get
			{
				return hmwMqToS7d;
			}
			set
			{
				hmwMqToS7d = value.ToString(2);
			}
		}

		public decimal Brokerage
		{
			get
			{
				return OPfMjO5Mpv;
			}
			set
			{
				OPfMjO5Mpv = value.ToString(2);
			}
		}

		public CalculationIncomeMoenyData()
		{
		}
	}

	public static decimal ToString(this decimal num, int scale)
	{
		string text = num.ToString();
		int num2 = text.IndexOf(".");
		int length = text.Length;
		if (num2 != -1)
		{
			return Convert.ToDecimal(string.Format("{0}.{1}", text.Substring(0, num2), text.Substring(num2 + 1, Math.Min(length - num2 - 1, scale))));
		}
		return Convert.ToDecimal(num.ToString());
	}

	public static CalculationIncomeMoenyData CalculationIncomeMoeny(decimal money, decimal commission)
	{
		CalculationIncomeMoenyData calculationIncomeMoenyData = new CalculationIncomeMoenyData();
		if (commission > 0m)
		{
			calculationIncomeMoenyData.Brokerage = commission / 1000m * money;
			calculationIncomeMoenyData.IncomeMoeny = money - calculationIncomeMoenyData.Brokerage;
		}
		else
		{
			calculationIncomeMoenyData.Brokerage = 0m;
			calculationIncomeMoenyData.IncomeMoeny = money;
		}
		return calculationIncomeMoenyData;
	}

	public static string ToRMB(object value)
	{
		try
		{
			string input = double.Parse(value.ToString()).ToString("#L#E#D#C#K#E#D#C#J#E#D#C#I#E#D#C#H#E#D#C#G#E#D#C#F#E#D#C#.0B0A");
			input = Regex.Replace(Regex.Replace(input, "((?<=-|^)[^1-9]*)|((?'z'0)[0A-E]*((?=[1-9])|(?'-z'(?=[F-L.]|$))))|((?'b'[F-L])(?'z'0)[0A-L]*((?=[1-9])|(?'-z'(?=[.]|$))))", "${b}${z}"), ".", (Match m) => "负圆空零壹贰叁肆伍陆柒捌玖空空空空空空空分角拾佰仟万億兆京垓秭穰"[m.Value[0] - 45].ToString());
			if (input.Substring(input.Length - 1, 1) == "圆")
			{
				input += "整";
			}
			return input;
		}
		catch (Exception)
		{
			return "零";
		}
	}

	public static string ToUpper(object value)
	{
		try
		{
			string input = double.Parse(value.ToString()).ToString("#L#E#D#C#K#E#D#C#J#E#D#C#I#E#D#C#H#E#D#C#G#E#D#C#F#E#D#C#.0B0A");
			input = Regex.Replace(Regex.Replace(input, "((?<=-|^)[^1-9]*)|((?'z'0)[0A-E]*((?=[1-9])|(?'-z'(?=[F-L.]|$))))|((?'b'[F-L])(?'z'0)[0A-L]*((?=[1-9])|(?'-z'(?=[.]|$))))", "${b}${z}"), ".", (Match m) => "负点空〇一二三四五六七八九空空空空空空空分角十百千万亿兆京垓秭穰"[m.Value[0] - 45].ToString());
			if (input.Substring(input.Length - 1, 1) == "点")
			{
				return input.Replace("点", "");
			}
			return input.Replace("角", "").Replace("分", "");
		}
		catch (Exception)
		{
			return "〇";
		}
	}
}
