using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain_2nd;

[SugarTable("charac_black_info")]
public class charac_black_info
{
	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	public short black_point { get; set; }

	public short offset_point { get; set; }

	public DateTime problem_child_time { get; set; }
}
