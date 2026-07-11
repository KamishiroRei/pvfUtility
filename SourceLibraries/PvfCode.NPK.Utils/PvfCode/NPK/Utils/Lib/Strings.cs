using System;
using System.Linq;
using System.Text;

namespace PvfCode.NPK.Utils.Lib;

public static class Strings
{
	public static string[] Split(this string str, params string[] pattern)
	{
		return str.Split(pattern, StringSplitOptions.RemoveEmptyEntries);
	}

	public static string GetSuffix(this string str)
	{
		return str.LastSubstring('\\', '/');
	}

	public static string RemoveSuffix(this string s)
	{
		int num = s.IndexOf(".", StringComparison.Ordinal);
		if (num < 0)
		{
			num = s.Length;
		}
		return s.Substring(0, num);
	}

	public static string RemoveSuffix(this string s, string c)
	{
		int num = s.LastIndexOf(c, StringComparison.Ordinal);
		if (num <= 0)
		{
			return s;
		}
		return s.Substring(0, num);
	}

	public static string RemovePrefix(this string s, string c)
	{
		int num = s.IndexOf(c, StringComparison.Ordinal);
		if (num <= 0)
		{
			return s;
		}
		return s.Substring(num);
	}

	public static string Complete(this string s1, string s2)
	{
		char[] array = s1.ToCharArray();
		char[] array2 = s2.ToCharArray();
		string text = string.Empty;
		StringBuilder stringBuilder = new StringBuilder();
		int num = array.Length - 1;
		int num2 = 0;
		while (num > 0 && num2 < array2.Length)
		{
			text = array[num] + text;
			stringBuilder.Append(array2[num2]);
			if (text.Equals(stringBuilder.ToString()))
			{
				s2 = s2.Substring(num2 + 1);
				break;
			}
			num--;
			num2++;
		}
		return s1 + s2;
	}

	public static string LastSubstring(this string str, params char[] split)
	{
		int num = split.Select((char c) => str.LastIndexOf(c)).Aggregate(-1, (int current, int index2) => (current <= index2 && index2 != -1) ? index2 : current);
		return str.Substring(num + 1);
	}
}
