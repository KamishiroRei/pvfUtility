using SqlSugar;

namespace GMTool.SqlModel.d_guild;

[SugarTable("guild_exp_ref")]
public class guild_exp_ref
{
	[SugarColumn(IsPrimaryKey = true)]
	public int grade { get; set; }

	public int exp { get; set; }
}
