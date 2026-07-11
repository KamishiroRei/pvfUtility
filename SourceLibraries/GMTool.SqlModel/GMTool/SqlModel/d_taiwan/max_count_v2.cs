using System;
using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("max_count_v2")]
public class max_count_v2
{
	public byte server_info { get; set; }

	public int num_occupations_charscreen { get; set; }

	public int num_occupations_seriaroom { get; set; }

	public int num_login_per_min { get; set; }

	public int num_logout_per_min { get; set; }

	public DateTime mc_date { get; set; }
}
