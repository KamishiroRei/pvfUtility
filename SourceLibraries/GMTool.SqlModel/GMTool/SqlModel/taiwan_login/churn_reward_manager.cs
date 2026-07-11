using SqlSugar;

namespace GMTool.SqlModel.taiwan_login;

[SugarTable("churn_reward_manager")]
public class churn_reward_manager
{
	[SugarColumn(IsPrimaryKey = true)]
	public byte min_day { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public byte max_day { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public int min_val { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public int max_val { get; set; }

	public int item_id { get; set; }

	public int add_info { get; set; }

	public int luck_point { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public byte quest_id { get; set; }
}
