using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("event_1106_idol_bring_up")]
public class event_1106_idol_bring_up
{
	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	public int m_id { get; set; }

	public byte pot_type { get; set; }

	public byte water_cnt { get; set; }

	public byte give_title_flag { get; set; }

	public DateTime occ_date { get; set; }

	public byte give_title_flag2 { get; set; }
}
