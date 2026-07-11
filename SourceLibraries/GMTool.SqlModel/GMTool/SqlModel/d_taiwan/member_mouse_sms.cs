using System;
using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("member_mouse_sms")]
public class member_mouse_sms
{
	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	public DateTime occ_time { get; set; }

	public byte cnt { get; set; }
}
