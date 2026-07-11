using System;
using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("geo_reject")]
public class geo_reject
{
	[SugarColumn(IsPrimaryKey = true)]
	public string rej_ip { get; set; }

	public string rej_c_code { get; set; }

	public int rej_ip_count { get; set; }

	public DateTime rej_last_date { get; set; }

	public string rej_chk { get; set; }

	public string rej_src { get; set; }
}
