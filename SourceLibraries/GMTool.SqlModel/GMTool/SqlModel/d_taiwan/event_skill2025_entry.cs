using System;
using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("event_skill2025_entry")]
public class event_skill2025_entry
{
	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	public byte server_id { get; set; }

	public int charac_no { get; set; }

	public DateTime occ_time { get; set; }
}
