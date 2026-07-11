using System;
using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("dnf_restrict_state")]
public class dnf_restrict_state
{
	[SugarColumn(IsPrimaryKey = true)]
	public int server_group { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public int category { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public int restrict_code { get; set; }

	public string restrict_value { get; set; }

	public DateTime mod_date { get; set; }

	public DateTime reg_date { get; set; }
}
