using System;
using System.ComponentModel.DataAnnotations;
using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("dnf_event_info")]
public class dnf_event_info
{
	[Display(Name = "活动编号")]
	[SugarColumn(IsPrimaryKey = true)]
	public int event_id { get; set; }

	public string event_name { get; set; }

	[Display(Name = "活动说明")]
	public string event_explain { get; set; }

	public byte apply_type { get; set; }

	public DateTime start_date { get; set; }

	public DateTime end_date { get; set; }
}
