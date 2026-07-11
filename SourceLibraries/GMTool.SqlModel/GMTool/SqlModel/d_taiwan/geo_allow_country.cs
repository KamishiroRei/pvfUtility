using System;
using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("geo_allow_country")]
public class geo_allow_country
{
	[SugarColumn(IsPrimaryKey = true)]
	public byte server_group { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public string country_code { get; set; }

	public DateTime reg_date { get; set; }
}
