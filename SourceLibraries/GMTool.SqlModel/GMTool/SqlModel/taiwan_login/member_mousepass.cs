using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_login;

[SugarTable("member_mousepass")]
public class member_mousepass
{
	public int m_id { get; set; }

	public string mousepass { get; set; }

	public DateTime occ_time { get; set; }

	public byte fail_cnt { get; set; }

	public short cancel_cnt { get; set; }

	public string version_info { get; set; }

	public int validity_time { get; set; }

	public int reward_time { get; set; }

	public string enable_flag { get; set; }
}
