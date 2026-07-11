using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_login;

[SugarTable("auto_punish_hack_full_ip")]
public class auto_punish_hack_full_ip
{
	[SugarColumn(IsPrimaryKey = true)]
	public DateTime occ_date { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public short hack_type { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public short hack_sub_type { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public string full_ip { get; set; }

	public int cnt { get; set; }
}
