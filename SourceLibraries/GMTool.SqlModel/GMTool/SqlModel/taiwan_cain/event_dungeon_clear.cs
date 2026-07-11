using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("event_dungeon_clear")]
public class event_dungeon_clear
{
	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	public int clear_cnt { get; set; }

	public DateTime update_time { get; set; }
}
