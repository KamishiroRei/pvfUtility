using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_login;

[SugarTable("member_security_card_history")]
public class member_security_card_history
{
	[SugarColumn(IsPrimaryKey = true)]
	public DateTime occ_time { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	public byte modify_type { get; set; }
}
