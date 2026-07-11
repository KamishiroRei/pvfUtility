using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("charac_trade_limit_info")]
public class charac_trade_limit_info
{
	public int m_id { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	public DateTime last_trade_time { get; set; }

	public int total_trade_gold { get; set; }

	public short trade_count { get; set; }

	public byte nexon_user { get; set; }
}
