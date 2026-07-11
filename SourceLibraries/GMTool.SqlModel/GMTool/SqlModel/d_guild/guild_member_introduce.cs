using SqlSugar;

namespace GMTool.SqlModel.d_guild;

[SugarTable("guild_member_introduce")]
public class guild_member_introduce
{
	[SugarColumn(IsPrimaryKey = true)]
	public int guild_id { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	public string introduce { get; set; }
}
