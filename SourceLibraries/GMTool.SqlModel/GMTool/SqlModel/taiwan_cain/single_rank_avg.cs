using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("single_rank_avg")]
public class single_rank_avg
{
	[SugarColumn(IsPrimaryKey = true)]
	public short dungeon_index { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public short level { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public short job { get; set; }

	public long clear_count { get; set; }

	public int average { get; set; }
}
