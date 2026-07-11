using System;
using SqlSugar;

namespace GMTool.SqlModel.d_guild;

[SugarTable("power_war")]
public class power_war
{
	[SugarColumn(IsPrimaryKey = true)]
	public byte server_id { get; set; }

	public int a_side_point { get; set; }

	public int b_side_point { get; set; }

	public byte winner_side { get; set; }

	public DateTime occ_time { get; set; }
}
