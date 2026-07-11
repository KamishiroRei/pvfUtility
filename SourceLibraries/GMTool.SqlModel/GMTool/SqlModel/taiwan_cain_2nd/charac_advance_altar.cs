using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain_2nd;

[SugarTable("charac_advance_altar")]
public class charac_advance_altar
{
	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public int ridable_id { get; set; }

	public short ticket_free { get; set; }

	public short ticket_cera { get; set; }

	public int star_game { get; set; }

	public int star_cera { get; set; }

	public int star_usable { get; set; }

	public short survival_best { get; set; }

	public short star_reset_count { get; set; }

	public short is_unlock_stage_effect { get; set; }

	public byte[] stage_list { get; set; }

	public byte[] slot_list { get; set; }

	public byte[] buy_item_list { get; set; }

	public byte[] reward_list { get; set; }
}
