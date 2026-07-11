using System;
using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("member_passwd_mod")]
public class member_passwd_mod
{
	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	public DateTime first_time { get; set; }

	public DateTime last_time { get; set; }

	public byte cnt { get; set; }
}
