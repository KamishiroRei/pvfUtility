using SqlSugar;

namespace GMTool.SqlModel.d_guild;

[SugarTable("guild_visit")]
public class guild_visit
{
	[SugarColumn(IsPrimaryKey = true)]
	public int guild_id { get; set; }

	public byte server_id { get; set; }

	public int total_visit { get; set; }

	public int today_visit { get; set; }
}
