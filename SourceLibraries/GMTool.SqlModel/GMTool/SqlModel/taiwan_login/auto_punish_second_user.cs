using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_login;

[SugarTable("auto_punish_second_user")]
public class auto_punish_second_user
{
	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	public DateTime occ_time { get; set; }

	public int total_trade_cnt { get; set; }

	public int trade_cnt { get; set; }

	public long total_trade_gold { get; set; }

	public long trade_gold { get; set; }

	public byte punish_flag { get; set; }
}
