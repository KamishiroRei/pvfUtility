
namespace Utools.字符串;

public static class RegexHelper
{
	public static string StrConvertRegexStr(this string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return str;
		}
		if (str.Length >= 2 && str.Substring(0, 2) == "$1")
		{
			bool flag = false;
			for (int i = 1; i < str.Length; i++)
			{
				if (!int.TryParse(str[i].ToString(), out var _))
				{
					flag = false;
				}
			}
			if (flag)
			{
				str = str.Insert(2, "\t");
			}
		}
		return str;
	}
}
