using System;
using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("member_safe_ensure")]
public class member_safe_ensure
{
	public int m_id { get; set; }

	public DateTime occ_time { get; set; }

	public string mobile_no { get; set; }

	public byte service_flag { get; set; }

	public byte type1_flag { get; set; }

	public byte type2_flag { get; set; }

	public DateTime expire_time { get; set; }

	public string settle_id { get; set; }
}
