using SqlSugar;

namespace GMTool.SqlModel.d_guild;

[SugarTable("power_war_user_rank")]
public class power_war_user_rank
{
	[SugarColumn(IsPrimaryKey = true)]
	public byte server_id { get; set; }

	public short rank { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	public int power_war_point { get; set; }

	public byte power_side { get; set; }
}
