using System;
using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("geo_allow")]
public class geo_allow
{
	[SugarColumn(IsPrimaryKey = true)]
	public string allow_ip { get; set; }

	public string allow_c_code { get; set; }

	public DateTime allow_date { get; set; }
}
