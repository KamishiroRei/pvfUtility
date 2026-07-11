using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_login;

[SugarTable("member_security_card")]
public class member_security_card
{
	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	public DateTime occ_time { get; set; }

	public string phone { get; set; }

	public string cert_key { get; set; }

	public string server_key { get; set; }

	public string card { get; set; }

	public byte fail_cnt { get; set; }

	public byte re_issue_cnt { get; set; }

	public DateTime last_issue_time { get; set; }

	public int validity_time { get; set; }

	public byte apply_flag { get; set; }

	public short cancel_cnt { get; set; }

	public byte web_flag { get; set; }

	public string cert_flag { get; set; }
}
