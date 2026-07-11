using System;
using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("member_handicap")]
public class member_handicap
{
	[SugarColumn(IsPrimaryKey = true)]
	public int event_id { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public byte cap_type { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public byte server_id { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public DateTime start_time { get; set; }

	public DateTime end_time { get; set; }

	public int handicap_value { get; set; }
}
