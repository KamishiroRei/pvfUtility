using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("ip_info")]
public class ip_info
{
	[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
	public int no { get; set; }

	public int m_id { get; set; }

	public string ip { get; set; }

	public byte start_ip { get; set; }

	public byte end_ip { get; set; }

	public DateTime occ_time { get; set; }

	public byte ip_check { get; set; }

	public int vendor_no { get; set; }

	public int speed_no { get; set; }

	public DateTime start_time { get; set; }

	public DateTime end_time { get; set; }

	public byte charge_flag { get; set; }

	public int settle_no { get; set; }
}
