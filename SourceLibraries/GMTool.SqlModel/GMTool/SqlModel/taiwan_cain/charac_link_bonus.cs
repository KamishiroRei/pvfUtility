using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("charac_link_bonus")]
public class charac_link_bonus
{
	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	public int exp { get; set; }

	public int gold { get; set; }

	public int mercenary_start_time { get; set; }

	public int mercenary_finish_time { get; set; }

	public byte mercenary_area { get; set; }

	public byte mercenary_period { get; set; }
}
