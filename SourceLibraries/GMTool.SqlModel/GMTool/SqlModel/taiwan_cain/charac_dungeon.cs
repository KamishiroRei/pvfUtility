using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("charac_dungeon")]
public class charac_dungeon
{
	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	public byte[] dungeon { get; set; }

	public byte[] best_clear_time { get; set; }

	public byte blue_marble_enter_count { get; set; }

	public string charac_inform_notice { get; set; }
}
