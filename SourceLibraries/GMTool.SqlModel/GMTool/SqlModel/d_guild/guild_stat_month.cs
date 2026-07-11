using System;
using SqlSugar;

namespace GMTool.SqlModel.d_guild;

[SugarTable("guild_stat_month")]
public class guild_stat_month
{
	[SugarColumn(IsPrimaryKey = true)]
	public DateTime occ_date { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public byte lev { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public byte server_id { get; set; }

	public int avg_guild_point { get; set; }

	public int avg_guild_point_acc { get; set; }
}
