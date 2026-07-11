using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Utools;

public static class StrHelper
{
	public static float ToFloat(this string value)
	{
		if (!float.TryParse(value, out var result))
		{
			return 0f;
		}
		return result;
	}

	public static string ToFloat2(this string value)
	{
		if (value == null)
		{
			return value;
		}
		if (!float.TryParse(value, out var result))
		{
			return value;
		}
		return result.ToString("f2");
	}

	public static double ToDouble(this string value)
	{
		if (!double.TryParse(value, out var result))
		{
			return 0.0;
		}
		return result;
	}

	public static string Mask(this string s, char mask = '*')
	{
		if (string.IsNullOrWhiteSpace(s?.Trim()))
		{
			return s;
		}
		s = s.Trim();
		string text = mask.ToString().PadLeft(4, mask);
		int length = s.Length;
		if (length < 11)
		{
			return length switch
			{
				10 => Regex.Replace(s, "(.{3}).*(.{3})", "$1" + text + "$2"), 
				9 => Regex.Replace(s, "(.{2}).*(.{3})", "$1" + text + "$2"), 
				8 => Regex.Replace(s, "(.{2}).*(.{2})", "$1" + text + "$2"), 
				7 => Regex.Replace(s, "(.{1}).*(.{2})", "$1" + text + "$2"), 
				6 => Regex.Replace(s, "(.{1}).*(.{1})", "$1" + text + "$2"), 
				_ => Regex.Replace(s, "(.{1}).*", "$1" + text), 
			};
		}
		return Regex.Replace(s, "(.{3}).*(.{4})", "$1" + text + "$2");
	}

	public static bool HasChinese(this string str)
	{
		return Regex.IsMatch(str, "[\\u4e00-\\u9fa5]");
	}

	public static string TextEncrypt(this string encryptStr, string key)
	{
		string s = Convert.ToBase64String(Encoding.Default.GetBytes(encryptStr));
		key = GLAVNrNBYd(key);
		byte[] bytes = Encoding.UTF8.GetBytes(key);
		byte[] bytes2 = Encoding.UTF8.GetBytes(s);
		Aes aes = Aes.Create();
		aes.Key = bytes;
		aes.Mode = CipherMode.ECB;
		aes.Padding = PaddingMode.PKCS7;
		byte[] array = aes.CreateEncryptor().TransformFinalBlock(bytes2, 0, bytes2.Length);
		return Convert.ToBase64String(array, 0, array.Length);
	}

	public static string TextDecrypt(this string decryptStr, string key)
	{
		key = GLAVNrNBYd(key);
		byte[] bytes = Encoding.UTF8.GetBytes(key);
		byte[] array = Convert.FromBase64String(decryptStr);
		Aes aes = Aes.Create();
		aes.Key = bytes;
		aes.Mode = CipherMode.ECB;
		aes.Padding = PaddingMode.PKCS7;
		byte[] bytes2 = aes.CreateDecryptor().TransformFinalBlock(array, 0, array.Length);
		byte[] bytes3 = Convert.FromBase64String(Encoding.UTF8.GetString(bytes2));
		return Encoding.Default.GetString(bytes3);
	}

	private static byte[] pWOVvLF7N5(string P_0)
	{
		MD5 mD = MD5.Create();
		byte[] bytes = Encoding.UTF8.GetBytes(P_0);
		byte[] result = mD.ComputeHash(bytes, 0, bytes.Length);
		mD.Clear();
		((IDisposable)mD).Dispose();
		return result;
	}

	private static string GLAVNrNBYd(string P_0)
	{
		byte[] array = pWOVvLF7N5(P_0);
		string text = "";
		for (int i = 0; i < array.Length; i++)
		{
			text += array[i].ToString("X").PadLeft(2, '0');
		}
		return text.ToLower();
	}

	public static IEnumerable<string> SubstringMultiple(string source, string startStr, string endStr)
	{
		MatchCollection matchCollection = new Regex("(?<=(" + startStr + "))[.\\s\\S]*?(?=(" + endStr + "))", RegexOptions.Multiline | RegexOptions.Singleline).Matches(source);
		List<string> list = new List<string>();
		foreach (Match item in matchCollection)
		{
			list.Add(item.Value);
		}
		return list;
	}

	private static bool ooGVTEH7a4(int P_0, out int P_1, string P_2, StringComparison P_3, string P_4, string P_5, string P_6, out string P_7)
	{
		if (P_2 == null || P_2.Length == 0)
		{
			P_7 = null;
			P_1 = -1;
			return false;
		}
		if (P_0 > P_2.Length)
		{
			P_1 = -1;
			P_7 = P_2;
			return false;
		}
		P_1 = P_2.IndexOf(P_4, P_0, P_3);
		if (P_1 != -1)
		{
			int num = P_2.IndexOf(P_5, P_1 + P_4.Length, P_3);
			if (num != -1)
			{
				num += P_5.Length;
				P_2 = P_2.Remove(P_1, num - P_1);
				P_7 = P_2.Insert(P_1, P_6);
				return true;
			}
		}
		P_7 = P_2;
		return false;
	}

	public static bool TraitFindReplceMain(string text, StringComparison stringComparison, string startText, string endText, string replaceText, out string newText)
	{
		int num = 0;
		bool flag = ooGVTEH7a4(0, out num, text, stringComparison, startText, endText, replaceText, out newText);
		int num2 = 0;
		while (flag)
		{
			num2++;
			flag = ooGVTEH7a4(num + replaceText.Length, out num, newText, stringComparison, startText, endText, replaceText, out newText);
		}
		return num2 > 0;
	}

	public static string Transform(string data)
	{
		return data.Replace("\\r\\n", Environment.NewLine).Replace("\\r", '\r'.ToString()).Replace("\\n", '\n'.ToString())
			.Replace("\\t", '\t'.ToString())
			.Replace("\\0", '\0'.ToString());
	}

	public static string GetRight(string str, string s)
	{
		return str.Substring(str.IndexOf(s), str.Length - str.Substring(0, str.IndexOf(s)).Length);
	}

	public static DateTime ConvertStringToDateTime2(string timeStamp)
	{
		DateTime dateTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
		long ticks = long.Parse(timeStamp + "0000");
		TimeSpan value = new TimeSpan(ticks);
		return dateTime.Add(value);
	}

	public static string Between(string text, string left, string right)
	{
		if (text == "" || text == null)
		{
			return "";
		}
		if (string.IsNullOrEmpty(left))
		{
			return "";
		}
		if (string.IsNullOrEmpty(right))
		{
			return "";
		}
		if (string.IsNullOrEmpty(text))
		{
			return "";
		}
		int num = text.IndexOf(left);
		if (num == -1)
		{
			return "";
		}
		num += left.Length;
		int num2 = text.IndexOf(right, num);
		if (num2 == -1)
		{
			return "";
		}
		int num3 = num;
		return text.Substring(num3, num2 - num3);
	}

	public static string GetRandomStr(int lengTh)
	{
		string text = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
		Random random = new Random((int)DateTime.Now.Ticks);
		string text2 = "";
		for (int i = 0; i < lengTh; i++)
		{
			text2 += text[random.Next(text.Length)];
		}
		return text2;
	}

	public static string GetRandomString(int length, bool useNum = false, bool useLow = false, bool useUpp = false, bool useSpe = false, string custom = null)
	{
		byte[] array = new byte[4];
		new RNGCryptoServiceProvider().GetBytes(array);
		Random random = new Random(BitConverter.ToInt32(array, 0));
		string text = null;
		string text2 = custom;
		if (useNum)
		{
			text2 += "0123456789";
		}
		if (useLow)
		{
			text2 += "abcdefghijklmnopqrstuvwxyz";
		}
		if (useUpp)
		{
			text2 += "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		}
		if (useSpe)
		{
			text2 += "_*";
		}
		for (int i = 0; i < length; i++)
		{
			text += text2.Substring(random.Next(0, text2.Length - 1), 1);
		}
		return text;
	}

	public static string GetMiddleStr(string oldStr, string strLeft, string strRight)
	{
		if (oldStr == null || oldStr.Length == 0)
		{
			return string.Empty;
		}
		int num = oldStr.IndexOf(strLeft);
		if (num == -1)
		{
			return string.Empty;
		}
		int num2 = oldStr.IndexOf(strRight, num + 1);
		if (num2 == -1)
		{
			return string.Empty;
		}
		int num3 = num2 - num - 1;
		if (num3 < 0)
		{
			return string.Empty;
		}
		return oldStr.Substring(num + 1, num3);
	}

	public static int IndexOfCharIsStrNumber(string Str, string Search)
	{
		int num = 0;
		for (int i = 0; i < Str.Length - Search.Length; i++)
		{
			if (Str.Substring(i, Search.Length) == Search)
			{
				num++;
			}
		}
		return num;
	}

	public static string ExtractANumberFromString(string str)
	{
		return Regex.Replace(str, "[^0-9]+", "");
	}
}
