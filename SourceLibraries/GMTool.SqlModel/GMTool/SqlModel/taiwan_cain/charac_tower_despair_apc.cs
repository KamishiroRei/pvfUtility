using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("charac_tower_despair_apc")]
public class charac_tower_despair_apc
{
	[SugarColumn(IsPrimaryKey = true)]
	public DateTime reg_date { get; set; }

	public int charac_no { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public int seq { get; set; }
}
