using System;
using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("limit_create_character")]
public class limit_create_character
{
	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	public int count { get; set; }

	public DateTime last_access_time { get; set; }
}
