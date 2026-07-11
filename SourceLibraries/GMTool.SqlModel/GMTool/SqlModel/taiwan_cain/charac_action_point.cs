using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("charac_action_point")]
public class charac_action_point
{
	[SugarColumn(IsPrimaryKey = true)]
	public DateTime occ_date { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	public int ap_sum { get; set; }

	public byte is_reward_medal { get; set; }

	public byte is_reward_item_1 { get; set; }

	public byte is_reward_item_2 { get; set; }

	public byte is_reward_item_3 { get; set; }

	public byte is_reward_item_4 { get; set; }

	public byte[] ap_clear_state { get; set; }
}
