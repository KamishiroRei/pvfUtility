using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("charac_housing_tree_info")]
public class charac_housing_tree_info
{
	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	public int tree_id { get; set; }

	public DateTime expire_date { get; set; }

	public short current_point { get; set; }

	public short leaf_point { get; set; }

	public short day_water_count { get; set; }
}
