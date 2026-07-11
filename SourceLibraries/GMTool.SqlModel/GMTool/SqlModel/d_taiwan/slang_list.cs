using SqlSugar;
using Utools;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("slang_list")]
public class slang_list
{
	[SugarColumn(IsPrimaryKey = true)]
	public string slang { get; set; }

	[SugarColumn(IsIgnore = true)]
	public string GridName => StrConvert.Convert(CodingHelper.Latin1ToGbkNew(slang), StrConvert.ConvertType.ToSimplified);

	public void SetValue(string value)
	{
		slang = StrConvert.Convert(value, StrConvert.ConvertType.ToTraditional);
	}
}
