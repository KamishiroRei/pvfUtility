using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_login;

[SugarTable("churn_reward_history")]
public class churn_reward_history
{
	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public DateTime occ_time { get; set; }

	public byte server_id { get; set; }

	public int charac_no { get; set; }

	public int item_id { get; set; }

	public int add_info { get; set; }

	public int luck_point { get; set; }

	public int reward_order { get; set; }

	public int cera { get; set; }
}
