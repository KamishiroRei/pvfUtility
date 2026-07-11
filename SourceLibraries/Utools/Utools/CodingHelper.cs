using System;
using System.Text;
using System.Web;

namespace Utools;

public static class CodingHelper
{
	public static string BytesToBase64Img(byte[] bytes)
	{
		try
		{
			string text = Convert.ToBase64String(bytes);
			return "data:image/jpeg;base64," + text;
		}
		catch (Exception)
		{
			return "";
		}
	}

	public static string B64encode(string str)
	{
		try
		{
			str = Convert.ToBase64String(Encoding.Default.GetBytes(str.ToCharArray()));
		}
		catch
		{
		}
		return str;
	}

	public static string UrlEncode(this string str)
	{
		StringBuilder stringBuilder = new StringBuilder();
		byte[] bytes = Encoding.UTF8.GetBytes(str);
		for (int i = 0; i < bytes.Length; i++)
		{
			stringBuilder.Append("%" + Convert.ToString(bytes[i], 16));
		}
		return stringBuilder.ToString();
	}

	public static string UrlDecode(this string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return string.Empty;
		}
		return HttpUtility.UrlDecode(text, Encoding.UTF8);
	}

	public static string SqlGbkToLatin1(string gbkStr)
	{
		Encoding encoding = Encoding.GetEncoding("ISO-8859-1");
		Encoding uTF = Encoding.UTF8;
		byte[] bytes = uTF.GetBytes(gbkStr);
		byte[] bytes2 = Encoding.Convert(uTF, encoding, bytes);
		return encoding.GetString(bytes2);
	}

	public static string Gb2312ToLatin1(string str)
	{
		if (str == null || str.Length == 0)
		{
			return string.Empty;
		}
		return Encoding.GetEncoding("iso-8859-1").GetString(Encoding.UTF8.GetBytes(str));
	}

	public static string Latin1ToGbkNew(string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return null;
		}
		return Encoding.UTF8.GetString(Encoding.GetEncoding("windows-1252").GetBytes(str));
	}

	public static string GbkToLatin1New(this string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return string.Empty;
		}
		Encoding uTF = Encoding.UTF8;
		return Encoding.GetEncoding("windows-1252").GetString(uTF.GetBytes(str));
	}

	public static string Latin1_gb2312(this string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return string.Empty;
		}
		return Encoding.UTF8.GetString(Encoding.GetEncoding("iso-8859-1").GetBytes(str));
	}
}
