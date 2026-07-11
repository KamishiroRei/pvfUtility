using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_login;

[SugarTable("auto_punish_second_log")]
public class auto_punish_second_log
{
	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public int hack_m_id { get; set; }

	public DateTime occ_time { get; set; }

	public int trade_cnt { get; set; }

	public long trade_gold { get; set; }
}
