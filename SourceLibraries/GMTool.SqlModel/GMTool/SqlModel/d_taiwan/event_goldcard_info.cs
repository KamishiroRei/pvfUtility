using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("event_goldcard_info")]
public class event_goldcard_info
{
	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	public short coupon { get; set; }
}
