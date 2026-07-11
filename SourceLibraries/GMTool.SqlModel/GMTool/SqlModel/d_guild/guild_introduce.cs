using SqlSugar;

namespace GMTool.SqlModel.d_guild;

[SugarTable("guild_introduce")]
public class guild_introduce
{
	[SugarColumn(IsPrimaryKey = true)]
	public int guild_id { get; set; }

	public byte server_id { get; set; }

	public string introduce { get; set; }
}
