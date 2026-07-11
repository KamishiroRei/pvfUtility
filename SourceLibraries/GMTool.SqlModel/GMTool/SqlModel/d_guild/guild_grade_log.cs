using System;
using SqlSugar;

namespace GMTool.SqlModel.d_guild;

[SugarTable("guild_grade_log")]
public class guild_grade_log
{
	[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
	public int id { get; set; }

	public int guild_id { get; set; }

	public int m_id { get; set; }

	public byte server_id { get; set; }

	public int charac_no { get; set; }

	public string charac_name { get; set; }

	public DateTime occ_time { get; set; }

	public byte grade_prev { get; set; }

	public byte grade_next { get; set; }

	public string reason { get; set; }

	public int? admin_no { get; set; }

	public string admin_name { get; set; }
}
