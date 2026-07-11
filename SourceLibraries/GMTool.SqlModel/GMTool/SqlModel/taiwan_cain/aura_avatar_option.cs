using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("aura_avatar_option")]
public class aura_avatar_option
{
	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public byte option_type { get; set; }

	public int value_1 { get; set; }
}
