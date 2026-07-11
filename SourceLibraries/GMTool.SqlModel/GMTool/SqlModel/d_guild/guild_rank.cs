using SqlSugar;

namespace GMTool.SqlModel.d_guild;

[SugarTable("guild_rank")]
public class guild_rank
{
	[SugarColumn(IsPrimaryKey = true)]
	public byte server_id { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public int guild_id { get; set; }

	public string guild_name { get; set; }

	public short guild_Rank { get; set; }

	public int guild_point { get; set; }

	public int guild_acc_point { get; set; }

	public int guild_visit { get; set; }

	public int guild_acc_visit { get; set; }

	public short guild_member { get; set; }

	public short guild_acc_member { get; set; }

	public short guild_avg_lev { get; set; }
}
