using System;
using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("member_white_account")]
public class member_white_account
{
	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	public DateTime reg_date { get; set; }
}
