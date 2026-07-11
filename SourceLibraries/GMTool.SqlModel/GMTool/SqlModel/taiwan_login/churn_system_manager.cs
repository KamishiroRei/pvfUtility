using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_login;

[SugarTable("churn_system_manager")]
public class churn_system_manager
{
	[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
	public int no { get; set; }

	public int weekday_var_a { get; set; }

	public int weekday_var_b { get; set; }

	public int weekday_var_c { get; set; }

	public int weekend_var_x { get; set; }

	public int weekend_var_y { get; set; }

	public int weekend_var_z { get; set; }

	public int next_reward_day { get; set; }

	public int admin_id { get; set; }

	public DateTime reg_time { get; set; }

	public byte state_flag { get; set; }
}
