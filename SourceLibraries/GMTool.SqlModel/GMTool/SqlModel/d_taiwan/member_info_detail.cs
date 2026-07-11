using System;
using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("member_info_detail")]
public class member_info_detail
{
	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	public string zipcode { get; set; }

	public string address { get; set; }

	public string address_detail { get; set; }

	public DateTime occ_date { get; set; }
}
