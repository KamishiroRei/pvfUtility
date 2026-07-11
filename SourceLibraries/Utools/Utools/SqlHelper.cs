
namespace Utools;

public static class SqlHelper
{
	public static string SqlLatin1ToGbk(this string label, string left = "")
	{
		return "convert(unhex(hex(convert(" + left + label + " using latin1))) using utf8)";
	}
}
