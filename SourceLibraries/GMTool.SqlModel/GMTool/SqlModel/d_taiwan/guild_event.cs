using System;
using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("guild_event")]
public class guild_event
{
	[SugarColumn(IsPrimaryKey = true)]
	public int gno { get; set; }

	public DateTime stt_date { get; set; }

	public DateTime end_date { get; set; }

	public DateTime ann_date { get; set; }

	public string page_url { get; set; }
}
