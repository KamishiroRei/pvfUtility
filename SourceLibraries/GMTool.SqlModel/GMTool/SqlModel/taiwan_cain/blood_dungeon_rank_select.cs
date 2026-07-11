using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("blood_dungeon_rank_select")]
public class blood_dungeon_rank_select
{
	[SugarColumn(IsPrimaryKey = true)]
	public long min_amount { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public long max_amount { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public byte rank { get; set; }

	public int reward_item_id { get; set; }

	public int reward_gold { get; set; }

	public int winner_count { get; set; }
}
