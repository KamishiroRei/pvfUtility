using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("charac_kill_monster_info")]
public class charac_kill_monster_info
{
	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	public byte[] boss_info { get; set; }

	public byte[] named_info { get; set; }

	public byte[] apc_boss_info { get; set; }
}
