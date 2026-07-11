using System;
using SqlSugar;

namespace GMTool.SqlModel.d_guild;

[SugarTable("guild_stat_uv")]
public class guild_stat_uv
{
	[SugarColumn(IsPrimaryKey = true)]
	public DateTime occ_date { get; set; }

	public int? pv { get; set; }

	public int new_bbs { get; set; }

	public int total_read_bbs { get; set; }

	public int member_uv { get; set; }

	public int member_uv_week { get; set; }

	public int master_uv { get; set; }

	public int master_uv_week { get; set; }
}
