using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain_2nd;

[SugarTable("fair_pvp_score")]
public class fair_pvp_score
{
	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	public int private_win { get; set; }

	public int private_lose { get; set; }

	public int private_draw { get; set; }

	public int relay_battle_win { get; set; }

	public int relay_battle_lose { get; set; }

	public int relay_battle_draw { get; set; }

	public int relay_battle_2kill { get; set; }

	public int successive_win { get; set; }

	public byte[] job_score { get; set; }

	public int? relay_battle_3kill { get; set; }

	public int? max_successive_win { get; set; }

	public int daily_play_count { get; set; }

	public DateTime last_play_time { get; set; }

	public byte[] pvp_mission_info { get; set; }

	public byte give_item { get; set; }
}
