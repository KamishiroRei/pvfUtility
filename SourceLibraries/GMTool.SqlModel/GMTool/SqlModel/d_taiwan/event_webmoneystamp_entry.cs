using System;
using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("event_webmoneystamp_entry")]
public class event_webmoneystamp_entry
{
	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	public DateTime occ_time { get; set; }

	public short attend_point { get; set; }

	public DateTime last_attend_time { get; set; }

	public byte return_flag { get; set; }

	public byte entry_item { get; set; }
}
