using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("pvp_result")]
public class pvp_result
{
	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	public int win { get; set; }

	public int lose { get; set; }

	public int pvp_point { get; set; }

	public int pvp_grade { get; set; }

	public byte pvp_grade_ext { get; set; }

	public int avg_kill_count { get; set; }

	public int avg_buf_count { get; set; }

	public int avg_debuf_count { get; set; }

	public int avg_heal_count { get; set; }

	public int avg_counter_count { get; set; }

	public int avg_back_atk_count { get; set; }

	public int avg_union_hit_count { get; set; }

	public int avg_overkill_count { get; set; }

	public int avg_aerial_count { get; set; }

	public int avg_combo_count { get; set; }

	public int avg_attacked_count { get; set; }

	public int avg_deal_damage { get; set; }

	public int avg_technic { get; set; }

	public int avg_style { get; set; }

	public int avg_hit_penalty { get; set; }

	public int pvp_count { get; set; }

	public int win_point { get; set; }

	public DateTime last_play_time { get; set; }

	public int play_count { get; set; }

	public int play_time { get; set; }

	public DateTime pvp_grade_ext_update_time { get; set; }
}
