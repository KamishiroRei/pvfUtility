using System;
using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("dnf_restrict_info")]
public class dnf_restrict_info
{
	[SugarColumn(IsPrimaryKey = true)]
	public int category { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public int restrict_code { get; set; }

	public string restrict_str { get; set; }

	public DateTime reg_date { get; set; }
}
