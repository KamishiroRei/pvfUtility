using System;
using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("guild_stat")]
public class guild_stat
{
	[SugarColumn(IsPrimaryKey = true)]
	public DateTime occ_date { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public byte lev { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public byte server_id { get; set; }

	public int create_no { get; set; }

	public int acc_create_no { get; set; }

	public int member_no { get; set; }

	public int acc_member_no { get; set; }

	public float avg_lev { get; set; }

	public float avg_master_lev { get; set; }
}
