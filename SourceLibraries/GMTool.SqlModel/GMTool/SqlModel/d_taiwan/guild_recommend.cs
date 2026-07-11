using System;
using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("guild_recommend")]
public class guild_recommend
{
	[SugarColumn(IsPrimaryKey = true)]
	public int no { get; set; }

	public int guild_id { get; set; }

	public byte server_id { get; set; }

	public int charac_no { get; set; }

	public string charac_name { get; set; }

	public string comment { get; set; }

	public DateTime recommend_time { get; set; }
}
