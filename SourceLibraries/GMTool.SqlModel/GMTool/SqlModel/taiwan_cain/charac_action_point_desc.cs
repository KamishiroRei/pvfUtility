using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("charac_action_point_desc")]
public class charac_action_point_desc
{
	[SugarColumn(IsPrimaryKey = true)]
	public int action_group_index { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public int action_index { get; set; }

	public string action_group_name { get; set; }
}
