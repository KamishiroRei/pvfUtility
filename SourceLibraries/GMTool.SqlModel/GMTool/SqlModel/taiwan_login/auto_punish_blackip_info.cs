using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_login;

[SugarTable("auto_punish_blackip_info")]
public class auto_punish_blackip_info
{
	[SugarColumn(IsPrimaryKey = true)]
	public string ip { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public byte start_ip { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public byte end_ip { get; set; }

	public DateTime reg_date { get; set; }

	public byte apply_flag { get; set; }
}
