using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_login;

[SugarTable("auto_punish_hack_info")]
public class auto_punish_hack_info
{
	[SugarColumn(IsPrimaryKey = true)]
	public short hack_type { get; set; }

	public int cnt { get; set; }

	public long etc { get; set; }

	public DateTime reg_date { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public byte apply_flag { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public short hack_sub_type { get; set; }

	public int hack_sub_cnt { get; set; }

	public int ip_cnt { get; set; }
}
