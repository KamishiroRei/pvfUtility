using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_login;

[SugarTable("nexon_none_memner_restriction")]
public class nexon_none_memner_restriction
{
	public int m_id { get; set; }

	public int charac_id { get; set; }

	public DateTime last_trade_time { get; set; }

	public int total_trade_gold { get; set; }

	public short trade_count { get; set; }

	public byte nexon_user { get; set; }
}
