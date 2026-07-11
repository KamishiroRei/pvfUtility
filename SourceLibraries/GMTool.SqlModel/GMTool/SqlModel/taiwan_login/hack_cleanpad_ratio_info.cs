using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_login;

[SugarTable("hack_cleanpad_ratio_info")]
public class hack_cleanpad_ratio_info
{
	[SugarColumn(IsPrimaryKey = true)]
	public short hack_type { get; set; }

	public int value { get; set; }

	public DateTime reg_date { get; set; }
}
