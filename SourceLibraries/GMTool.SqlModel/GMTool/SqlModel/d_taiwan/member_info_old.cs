using System;
using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("member_info_old")]
public class member_info_old
{
	[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
	public int m_id { get; set; }

	public string user_id { get; set; }

	public string user_name { get; set; }

	public string first_ssn { get; set; }

	public string second_ssn { get; set; }

	public string passwd { get; set; }

	public string mobile_no { get; set; }

	public int reg_date { get; set; }

	public string email { get; set; }

	public byte q_no { get; set; }

	public string q_answer { get; set; }

	public DateTime updt_date { get; set; }

	public byte state { get; set; }

	public string nickname { get; set; }

	public string email_yn { get; set; }

	public byte ssn_check { get; set; }

	public DateTime last_play_time { get; set; }
}
