using System;
using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("event_mage_2years")]
public class event_mage_2years
{
	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public byte server_info { get; set; }

	[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
	public int charac_no { get; set; }

	public string charac_name { get; set; }

	public DateTime create_time { get; set; }

	public DateTime delete_time { get; set; }

	public byte delete_flag { get; set; }
}
