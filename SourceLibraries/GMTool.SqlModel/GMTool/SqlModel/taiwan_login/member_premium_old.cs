using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_login;

[SugarTable("member_premium_old")]
public class member_premium_old
{
	[SugarColumn(IsPrimaryKey = true)]
	public int event_id { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public byte pre_type { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public DateTime service_start { get; set; }

	public DateTime service_end { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public byte server_id { get; set; }
}
