namespace Utools;

public static class ChineseHelper
{
	public static string ToSimplified(string source)
	{
		if (source == null)
		{
			return null;
		}
		return StrConvert.Convert(source, StrConvert.ConvertType.ToSimplified);
	}

	public static string ToTraditional(string source)
	{
		if (source == null)
		{
			return null;
		}
		return StrConvert.Convert(source, StrConvert.ConvertType.ToTraditional);
	}
}
