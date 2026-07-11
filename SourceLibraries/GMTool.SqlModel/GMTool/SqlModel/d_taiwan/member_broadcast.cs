using System;
using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("member_broadcast")]
public class member_broadcast
{
	[SugarColumn(IsPrimaryKey = true)]
	public int event_id { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public byte server_id { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	public string charac_name { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public DateTime start_time { get; set; }

	public DateTime end_time { get; set; }
}
