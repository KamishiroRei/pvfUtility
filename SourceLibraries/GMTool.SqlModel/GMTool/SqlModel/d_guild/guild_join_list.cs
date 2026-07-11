using System;
using SqlSugar;

namespace GMTool.SqlModel.d_guild;

[SugarTable("guild_join_list")]
public class guild_join_list
{
	[SugarColumn(IsPrimaryKey = true)]
	public int guild_id { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	public byte server_group { get; set; }

	public int m_id { get; set; }

	public string born_year { get; set; }

	public string memo { get; set; }

	public DateTime? occ_time { get; set; }
}
