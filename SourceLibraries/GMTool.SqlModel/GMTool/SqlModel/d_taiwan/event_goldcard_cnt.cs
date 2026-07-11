using System;
using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("event_goldcard_cnt")]
public class event_goldcard_cnt
{
	[SugarColumn(IsPrimaryKey = true)]
	public int item_no { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public DateTime occ_date { get; set; }

	public int cnt { get; set; }
}
