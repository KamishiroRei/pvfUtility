using System;
using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("tme_charac")]
public class tme_charac
{
	public int m_id { get; set; }

	public int charac_no { get; set; }

	public string charac_name { get; set; }

	public byte village { get; set; }

	public byte job { get; set; }

	public byte lev { get; set; }

	public int exp { get; set; }

	public byte grow_type { get; set; }

	public byte HP { get; set; }

	public short maxHP { get; set; }

	public short maxMP { get; set; }

	public short phy_attack { get; set; }

	public short phy_defense { get; set; }

	public short mag_attack { get; set; }

	public short mag_defense { get; set; }

	public byte[] element_resist { get; set; }

	public byte[] spec_property { get; set; }

	public int inven_weight { get; set; }

	public short hp_regen { get; set; }

	public short mp_regen { get; set; }

	public short move_speed { get; set; }

	public short attack_speed { get; set; }

	public short cast_speed { get; set; }

	public short hit_recovery { get; set; }

	public short jump { get; set; }

	public int charac_weight { get; set; }

	public short fatigue { get; set; }

	public short max_fatigue { get; set; }

	public short premium_fatigue { get; set; }

	public short max_premium_fatigue { get; set; }

	public DateTime create_time { get; set; }

	public DateTime last_play_time { get; set; }

	public int dungeon_clear_point { get; set; }

	public DateTime delete_time { get; set; }

	public byte delete_flag { get; set; }

	public int guild_id { get; set; }

	public byte guild_right { get; set; }

	public byte member_flag { get; set; }
}
