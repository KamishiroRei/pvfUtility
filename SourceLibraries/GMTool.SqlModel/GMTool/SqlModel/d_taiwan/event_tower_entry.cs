using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("event_tower_entry")]
public class event_tower_entry
{
	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	public int occ_date { get; set; }

	public int occ_check { get; set; }

	public byte server_id { get; set; }

	public int charac_no { get; set; }

	public int item1_no { get; set; }

	public int item1_check { get; set; }

	public int item2_no { get; set; }

	public int item2_check { get; set; }

	public int item3_no { get; set; }

	public int item3_check { get; set; }
}
