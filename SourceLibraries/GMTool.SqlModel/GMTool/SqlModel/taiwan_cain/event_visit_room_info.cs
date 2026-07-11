using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("event_visit_room_info")]
public class event_visit_room_info
{
	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	public byte visit_cnt { get; set; }

	public byte[] visit_charac_no { get; set; }

	public DateTime update_time { get; set; }
}
