using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("charac_tower_despair")]
public class charac_tower_despair
{
	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	public DateTime first_layer_start_date { get; set; }

	public byte today_enter_count { get; set; }

	public byte last_clear_layer { get; set; }

	public int enter_count_by_week { get; set; }

	public DateTime m_date { get; set; }

	public DateTime last_clear_date { get; set; }
}
