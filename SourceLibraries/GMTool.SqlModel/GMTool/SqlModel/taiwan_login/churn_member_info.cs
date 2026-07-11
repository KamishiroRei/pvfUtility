using SqlSugar;

namespace GMTool.SqlModel.taiwan_login;

[SugarTable("taiwan_login.churn_member_info")]
public class churn_member_info
{
	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	[SugarColumn(IsIgnore = true)]
	public string accountname { get; set; }

	public int accrue_cera { get; set; }

	public string play_info { get; set; }

	public int first_reward_time { get; set; }

	public int last_reward_time { get; set; }

	public int server_id { get; set; }

	public int charac_no { get; set; }

	public int item_id { get; set; }

	public int add_info { get; set; }

	public int luck_point { get; set; }

	public int last_update_time { get; set; }

	public int second_reward_time { get; set; }

	public int quest_time { get; set; }
}
