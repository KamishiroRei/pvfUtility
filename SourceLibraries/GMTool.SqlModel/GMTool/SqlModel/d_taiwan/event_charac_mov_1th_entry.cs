using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("event_charac_mov_1th_entry")]
public class event_charac_mov_1th_entry
{
	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	public int occ_time { get; set; }

	public int it_no { get; set; }

	public int item_check { get; set; }
}
