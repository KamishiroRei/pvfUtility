using System;
using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("max_count")]
public class max_count
{
	public byte server_info { get; set; }

	public int mc_max { get; set; }

	public DateTime mc_date { get; set; }
}
