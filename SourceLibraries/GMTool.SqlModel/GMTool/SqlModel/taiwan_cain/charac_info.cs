using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("taiwan_cain.charac_info")]
public class charac_info
{
	public int m_id { get; set; }

	[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
	public int charac_no { get; set; }

	public string charac_name { get; set; }

	public byte village { get; set; }

	public byte job { get; set; }

	public byte lev { get; set; }

	public int exp { get; set; }

	public byte grow_type { get; set; }

	public byte HP { get; set; }

	public int maxHP { get; set; }

	public int maxMP { get; set; }

	public int phy_attack { get; set; }

	public int phy_defense { get; set; }

	public int mag_attack { get; set; }

	public int mag_defense { get; set; }

	public byte[] element_resist { get; set; }

	public byte[] spec_property { get; set; }

	public int inven_weight { get; set; }

	public int hp_regen { get; set; }

	public int mp_regen { get; set; }

	public int move_speed { get; set; }

	public int attack_speed { get; set; }

	public int cast_speed { get; set; }

	public int hit_recovery { get; set; }

	public int jump { get; set; }

	public int charac_weight { get; set; }

	public int fatigue { get; set; }

	public int max_fatigue { get; set; }

	public int premium_fatigue { get; set; }

	public int max_premium_fatigue { get; set; }

	public DateTime create_time { get; set; }

	public DateTime last_play_time { get; set; }

	public int dungeon_clear_point { get; set; }

	public DateTime delete_time { get; set; }

	public byte delete_flag { get; set; }

	public int guild_id { get; set; }

	public byte guild_right { get; set; }

	public byte member_flag { get; set; }

	public byte sex { get; set; }

	public byte expert_job { get; set; }

	public byte skill_tree_index { get; set; }

	public int link_charac_no { get; set; }

	public byte event_charac_level { get; set; }

	public byte guild_secede { get; set; }

	public int start_time { get; set; }

	public int finish_time { get; set; }

	public byte competition_area { get; set; }

	public byte competition_period { get; set; }

	public int mercenary_start_time { get; set; }

	public int mercenary_finish_time { get; set; }

	public byte mercenary_area { get; set; }

	public byte mercenary_period { get; set; }
}
