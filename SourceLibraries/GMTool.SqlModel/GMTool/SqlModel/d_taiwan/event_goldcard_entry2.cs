using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("event_goldcard_entry2")]
public class event_goldcard_entry2
{
	[SugarColumn(IsPrimaryKey = true)]
	public int occ_date { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	public byte server_id { get; set; }

	public int charac_no { get; set; }

	public int item_no { get; set; }

	public int item_check { get; set; }
}
