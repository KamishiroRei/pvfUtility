using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_login;

[SugarTable("auto_punish_first_user")]
public class auto_punish_first_user
{
	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	public DateTime occ_time { get; set; }

	public string ip { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public short hack_type { get; set; }

	public int cnt { get; set; }

	public byte punish_flag { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public short hack_sub_type { get; set; }

	public int hack_sub_cnt { get; set; }
}
