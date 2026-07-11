using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("party_rank_avg")]
public class party_rank_avg
{
	[SugarColumn(IsPrimaryKey = true)]
	public short dungeon_index { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public short party_level { get; set; }

	public long clear_count { get; set; }

	public int average { get; set; }
}
