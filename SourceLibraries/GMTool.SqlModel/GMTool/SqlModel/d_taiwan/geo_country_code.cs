using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("geo_country_code")]
public class geo_country_code
{
	[SugarColumn(IsPrimaryKey = true)]
	public int code_no { get; set; }

	public string country_code_a2 { get; set; }

	public string country_code_a3 { get; set; }

	public string country { get; set; }
}
