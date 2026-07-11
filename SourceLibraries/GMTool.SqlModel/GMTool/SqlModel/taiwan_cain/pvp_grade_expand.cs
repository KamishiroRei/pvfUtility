using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("pvp_grade_expand")]
public class pvp_grade_expand
{
	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	public int pvp_grade { get; set; }

	public int pvp_point { get; set; }

	public DateTime last_play_time { get; set; }
}
