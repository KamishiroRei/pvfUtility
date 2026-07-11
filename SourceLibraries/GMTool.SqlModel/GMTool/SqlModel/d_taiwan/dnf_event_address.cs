using System;
using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("dnf_event_address")]
public class dnf_event_address
{
	[SugarColumn(IsPrimaryKey = true)]
	public int event_id { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	public DateTime occ_date { get; set; }

	public string zipcode { get; set; }

	public string address { get; set; }

	public string phone_no { get; set; }
}
