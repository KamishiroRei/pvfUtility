using SqlSugar;

namespace GMTool.SqlModel.d_guild;

[SugarTable("guild_body")]
public class guild_body
{
	[SugarColumn(IsPrimaryKey = true)]
	public int gno { get; set; }

	public string body { get; set; }
}
