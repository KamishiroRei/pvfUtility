using SqlSugar;

namespace GMTool.SqlModel.d_guild;

[SugarTable("guild_skill")]
public class guild_skill
{
	[SugarColumn(IsPrimaryKey = true)]
	public int guild_id { get; set; }

	public int remain_sp { get; set; }

	public byte[] skill_slot { get; set; }

	public int used_sp { get; set; }
}
