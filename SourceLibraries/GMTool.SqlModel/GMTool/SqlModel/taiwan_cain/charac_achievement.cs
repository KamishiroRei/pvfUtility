using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("charac_achievement")]
public class charac_achievement
{
	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	public byte[] achievement { get; set; }

	public DateTime last_update_time { get; set; }
}
