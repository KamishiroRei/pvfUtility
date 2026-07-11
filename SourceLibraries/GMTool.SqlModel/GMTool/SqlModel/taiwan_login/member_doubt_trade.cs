using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_login;

[SugarTable("member_doubt_trade")]
public class member_doubt_trade
{
	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	public DateTime last_update_time { get; set; }

	public short over_count { get; set; }
}
