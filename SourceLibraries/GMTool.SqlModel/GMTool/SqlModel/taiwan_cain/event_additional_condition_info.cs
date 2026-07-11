using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("event_additional_condition_info")]
public class event_additional_condition_info
{
	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	public byte current_step { get; set; }

	public byte reward_step { get; set; }

	public DateTime update_time { get; set; }
}
