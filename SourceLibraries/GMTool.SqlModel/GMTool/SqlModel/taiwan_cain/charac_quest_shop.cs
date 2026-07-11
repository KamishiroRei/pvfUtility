using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("charac_quest_shop")]
public class charac_quest_shop
{
	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	public int qp { get; set; }

	public short max_hp { get; set; }

	public short max_mp { get; set; }

	public short psy_attack { get; set; }

	public short psy_defense { get; set; }

	public short mag_attack { get; set; }

	public short mag_defence { get; set; }

	public short move_speed { get; set; }

	public short attack_speed { get; set; }

	public short hp_regen { get; set; }

	public short mp_regen { get; set; }

	public short all_element_resist { get; set; }

	public short fire_element_resist { get; set; }

	public short water_element_resist { get; set; }

	public short light_element_resist { get; set; }

	public short dark_element_resist { get; set; }

	public short all_element_attack { get; set; }

	public short fire_element_attack { get; set; }

	public short water_element_attack { get; set; }

	public short light_element_attack { get; set; }

	public short dark_element_attack { get; set; }

	public short psy_critical { get; set; }

	public short mag_critical { get; set; }

	public short good_hit { get; set; }

	public short evasion { get; set; }

	public short hit_recovery { get; set; }

	public short init_count { get; set; }

	public short separate_psy_mag_attack { get; set; }

	public short quest_piece { get; set; }
}
