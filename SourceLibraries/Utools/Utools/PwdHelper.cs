using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Utools;

public static class PwdHelper
{
	public static string GetMD5(this string myString)
	{
		if (string.IsNullOrEmpty(myString))
		{
			return string.Empty;
		}
		MD5 mD = MD5.Create();
		byte[] bytes = Encoding.UTF8.GetBytes(myString);
		byte[] array = mD.ComputeHash(bytes);
		string text = null;
		for (int i = 0; i < array.Length; i++)
		{
			text += array[i].ToString("x2");
		}
		return text;
	}

	public static string UrlEncode(string str)
	{
		return HttpUtility.UrlEncode(str);
	}
}
