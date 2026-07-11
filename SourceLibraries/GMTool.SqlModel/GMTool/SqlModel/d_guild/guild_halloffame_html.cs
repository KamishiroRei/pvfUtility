using SqlSugar;

namespace GMTool.SqlModel.d_guild;

[SugarTable("guild_halloffame_html")]
public class guild_halloffame_html
{
	[SugarColumn(IsPrimaryKey = true)]
	public int fame_id { get; set; }

	public string title { get; set; }

	public string html { get; set; }
}
