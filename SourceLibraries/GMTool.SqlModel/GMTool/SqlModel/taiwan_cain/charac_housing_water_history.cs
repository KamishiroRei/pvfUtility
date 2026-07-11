using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("charac_housing_water_history")]
public class charac_housing_water_history
{
	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public DateTime give_time { get; set; }

	public string give_charac_name { get; set; }
}
