using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("charac_blood_dungeon_reward")]
public class charac_blood_dungeon_reward
{
	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public DateTime week_occ_date { get; set; }

	public int week_point { get; set; }

	public int week_enter_count { get; set; }

	public int week_use_gold { get; set; }

	public DateTime last_play_date { get; set; }

	public int enter_count { get; set; }

	public byte rank { get; set; }

	public byte reward { get; set; }

	public int reward_item_id { get; set; }

	public int reward_gold { get; set; }
}
