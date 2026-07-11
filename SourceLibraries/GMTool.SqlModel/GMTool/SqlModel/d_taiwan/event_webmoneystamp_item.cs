using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("event_webmoneystamp_item")]
public class event_webmoneystamp_item
{
	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public int occ_time { get; set; }

	public byte server_id { get; set; }

	public byte charac_no { get; set; }

	public int item_no { get; set; }

	public int item_check { get; set; }
}
