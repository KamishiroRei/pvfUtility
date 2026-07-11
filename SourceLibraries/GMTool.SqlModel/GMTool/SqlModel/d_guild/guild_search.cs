using System;
using SqlSugar;

namespace GMTool.SqlModel.d_guild;

[SugarTable("guild_search")]
public class guild_search
{
	[SugarColumn(IsPrimaryKey = true)]
	public int guild_id { get; set; }

	public byte server_id { get; set; }

	public string guild_name { get; set; }

	public string master_name { get; set; }

	public DateTime create_time { get; set; }

	public int lev { get; set; }

	public int member_count { get; set; }

	public int guild_point_acc { get; set; }

	public int guild_exp { get; set; }

	public string guild_url { get; set; }
}
