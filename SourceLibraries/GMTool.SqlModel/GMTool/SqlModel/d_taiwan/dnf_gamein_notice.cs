using System;
using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("dnf_gamein_notice")]
public class dnf_gamein_notice
{
	[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
	public int no { get; set; }

	public string img_name { get; set; }

	public byte server_id { get; set; }

	public DateTime reg_time { get; set; }

	public string open_flag { get; set; }
}
