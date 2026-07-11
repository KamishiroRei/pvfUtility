using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("member_miles")]
public class member_miles
{
	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	public int miles { get; set; }

	public short daily_miles { get; set; }
}
