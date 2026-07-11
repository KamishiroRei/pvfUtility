using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_login;

[SugarTable("taiwan_login.member_play_info")]
public class member_play_info
{
	[SugarColumn(IsPrimaryKey = true)]
	public DateTime occ_date { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	public int play_time { get; set; }

	[SugarColumn(IsIgnore = true)]
	public long PayTimeLong { get; set; }

	public int play_count { get; set; }

	public int trade_cnt { get; set; }

	public int exp { get; set; }

	public int used_fatigue { get; set; }

	public string ip { get; set; }

	public int last_play_time { get; set; }

	public int pcbang_flag { get; set; }

	public string end_ip { get; set; }

	public int ting_count { get; set; }

	public string mac_addr { get; set; }

	public int server_id { get; set; }
}
