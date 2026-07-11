using System.Text;
using System.Text.RegularExpressions;

namespace Utools;

public class CookieHelper
{
	public static string GetCookieVlaue(string cookiesString, string cookieName)
	{
		if (string.IsNullOrEmpty(cookiesString))
		{
			return string.Empty;
		}
		return Regex.Match(cookiesString, "(^| )" + cookieName + "=([^;]*)(;|$)").Value;
	}

	public static string GetCookieValue2(string cookies, string cookieName)
	{
		if (!string.IsNullOrEmpty(cookies))
		{
			return StrHelper.Between(cookies, cookieName + "=", ";");
		}
		return "";
	}

	public static string ToCookie(string cookie, string[] names)
	{
		if (string.IsNullOrEmpty(cookie))
		{
			return string.Empty;
		}
		for (int i = 0; i < cookie.Length; i++)
		{
			_ = cookie[i];
		}
		StringBuilder stringBuilder = new StringBuilder();
		foreach (string text in names)
		{
			string value = StrHelper.Between(cookie, text + "=", ";");
			if (!string.IsNullOrEmpty(value))
			{
				StringBuilder stringBuilder2 = stringBuilder;
				StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(3, 2, stringBuilder2);
				handler.AppendFormatted(text);
				handler.AppendLiteral("=");
				handler.AppendFormatted(value);
				handler.AppendLiteral("; ");
				stringBuilder2.Append(ref handler);
			}
		}
		return stringBuilder.ToString();
	}

	public CookieHelper()
	{
	}
}
