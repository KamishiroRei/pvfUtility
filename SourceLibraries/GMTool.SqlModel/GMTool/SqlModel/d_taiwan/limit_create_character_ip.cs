using System;
using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("limit_create_character_ip")]
public class limit_create_character_ip
{
	[SugarColumn(IsPrimaryKey = true)]
	public int ip { get; set; }

	public string ip_str { get; set; }

	public DateTime last_access_time { get; set; }

	public int count { get; set; }

	public int last_access_mid { get; set; }
}
