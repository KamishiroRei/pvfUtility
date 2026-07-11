using SqlSugar;

namespace GMTool.SqlModel.d_guild;

[SugarTable("guild_halloffame")]
public class guild_halloffame
{
	[SugarColumn(IsPrimaryKey = true)]
	public int fame_id { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public byte server_id { get; set; }

	public int guild_id { get; set; }

	public string guild_name { get; set; }

	public string file_url { get; set; }

	public byte open_flag { get; set; }

	public byte main_flag { get; set; }
}
