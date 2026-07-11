using System;
using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("dnf_event_entry_notuse")]
public class dnf_event_entry_notuse
{
	[SugarColumn(IsPrimaryKey = true)]
	public int event_id { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	public DateTime occ_date { get; set; }

	public byte server_id { get; set; }

	public int charac_no { get; set; }

	public DateTime obtain_date { get; set; }
}
