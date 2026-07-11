using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("event_goldcard_entry1")]
public class event_goldcard_entry1
{
	[SugarColumn(IsPrimaryKey = true)]
	public int occ_date { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	public byte item_no { get; set; }
}
