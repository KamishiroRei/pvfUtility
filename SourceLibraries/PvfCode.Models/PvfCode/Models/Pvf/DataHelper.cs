using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace PvfCode.Models.Pvf;

public static class DataHelper
{
	public static int GetResolve(int num)
	{
		if (num > 500000)
		{
			return 768;
		}
		if (num > 400000)
		{
			return 512;
		}
		if (num > 300000)
		{
			return 384;
		}
		if (num > 200000)
		{
			return 256;
		}
		if (num > 100000)
		{
			return 128;
		}
		if (num > 50000)
		{
			return 64;
		}
		if (num > 25000)
		{
			return 32;
		}
		if (num > 10000)
		{
			return 16;
		}
		if (num <= 5000)
		{
			return 2;
		}
		return 8;
	}

	public static uint GetFileNameHashCode(IEnumerable<byte> dataBytes)
	{
		return dataBytes.Aggregate(5381u, (uint current, byte t) => 33 * current + t) * 33;
	}

	public static bool BytesEquals(byte[] b1, byte[] b2)
	{
		if ((b1 == null) & (b2 == null))
		{
			return true;
		}
		if ((b1 != null) & (b2 == null))
		{
			return false;
		}
		if ((b1 == null) & (b2 != null))
		{
			return false;
		}
		if (b1.Length != b2.Length)
		{
			return false;
		}
		return !b1.Where((byte value, int index) => value != b2[index]).Any();
	}

	public static string GetDataFromFormat(string source, string header, string ending)
	{
		int num = ((header != "") ? source.IndexOf(header, StringComparison.Ordinal) : 0);
		if (num == -1)
		{
			return "";
		}
		num += header.Length;
		string text = source.Substring(num, source.Length - num);
		if (ending == "")
		{
			return text;
		}
		int num2 = text.IndexOf(ending, StringComparison.Ordinal);
		if (num2 != -1)
		{
			return text.Substring(0, num2);
		}
		return "";
	}

	public static string FormatFloat(float f)
	{
		string text = f.ToString(CultureInfo.InvariantCulture);
		if (text.IndexOf('.') > 0)
		{
			return text;
		}
		return text + ".0";
	}

	public static string FormatFloat(double f)
	{
		string text = f.ToString(CultureInfo.InvariantCulture);
		if (text.IndexOf('.') > 0)
		{
			return text;
		}
		return text + ".0";
	}

	public static string Transform(string data)
	{
		return data.Replace("\\r\\n", Environment.NewLine).Replace("\\r", '\r'.ToString()).Replace("\\n", '\n'.ToString())
			.Replace("\\t", '\t'.ToString())
			.Replace("\\0", '\0'.ToString());
	}

	public static string SizeToText(long size)
	{
		if (size > 1048576)
		{
			return ((double)size / 1024.0 / 1024.0).ToString("f3") + " MiB";
		}
		if (size > 1024)
		{
			return ((double)size / 1024.0).ToString("f3") + " KiB";
		}
		return ((double)size).ToString("f3") + " Bytes";
	}
}
