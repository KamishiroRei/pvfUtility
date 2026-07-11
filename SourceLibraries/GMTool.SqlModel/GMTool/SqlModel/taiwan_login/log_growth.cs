using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_login;

[SugarTable("log_growth")]
public class log_growth
{
	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public byte server_info { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	public string charac_name { get; set; }

	public byte job { get; set; }

	public byte grow_type { get; set; }

	public DateTime occ_time { get; set; }
}
