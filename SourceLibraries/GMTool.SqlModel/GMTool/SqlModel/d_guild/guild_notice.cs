using SqlSugar;

namespace GMTool.SqlModel.d_guild;

[SugarTable("guild_notice")]
public class guild_notice
{
	[SugarColumn(IsPrimaryKey = true)]
	public int guild_id { get; set; }

	public string notice { get; set; }

	public int acc_date { get; set; }
}
