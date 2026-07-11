using System;
using SqlSugar;

namespace GMTool.SqlModel.d_guild;

[SugarTable("d_guild.guild_info")]
public class guild_info
{
	[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
	public int guild_id { get; set; }

	public byte server_id { get; set; }

	public string guild_name { get; set; }

	public int master_id { get; set; }

	public int master_no { get; set; }

	public string master_name { get; set; }

	public string guild_url { get; set; }

	public byte guild_icon { get; set; }

	public DateTime create_time { get; set; }

	public int lev { get; set; }

	public byte ability { get; set; }

	public byte expire_flag { get; set; }

	public DateTime expire_time { get; set; }

	public DateTime member_secede_time { get; set; }

	public int member_count { get; set; }

	public byte recommend_flag { get; set; }

	public DateTime recommend_time { get; set; }

	public int guild_point { get; set; }

	public int guild_point_acc { get; set; }

	public int guild_point_prev { get; set; }

	public int guild_rank { get; set; }

	public int guild_war_point { get; set; }

	public short final_entry { get; set; }

	public short final_win { get; set; }

	public byte guild_icon_auth { get; set; }

	public int guild_exp { get; set; }

	public byte power_side { get; set; }

	public byte guild_agit_flag { get; set; }

	public DateTime lev_up_time { get; set; }

	public DateTime power_secede_time { get; set; }

	public int power_war_point { get; set; }

	public byte power_join_count { get; set; }

	public int guild_fund { get; set; }
}
