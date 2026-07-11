using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_login;

[SugarTable("member_mousepass_history")]
public class member_mousepass_history
{
	public DateTime occ_time { get; set; }

	public int m_id { get; set; }

	public string pre_mousepass { get; set; }

	public byte modify_type { get; set; }
}
