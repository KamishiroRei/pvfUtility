using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_login;

[SugarTable("taiwan_login.login_account_3")]
public class login_account_3
{
	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	public int m_channel_no { get; set; }

	public byte login_status { get; set; }

	public DateTime last_login_date { get; set; }

	public string login_ip { get; set; }
}
