using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_login;

[SugarTable("ip_monitor_punish")]
public class ip_monitor_punish
{
	[SugarColumn(IsPrimaryKey = true)]
	public string ip { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public byte type { get; set; }

	public short m_id_cnt { get; set; }

	public DateTime start_time { get; set; }

	public DateTime end_time { get; set; }
}
