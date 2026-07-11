using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain_2nd;

[SugarTable("charac_black_list")]
public class charac_black_list
{
	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	public string charac_name { get; set; }

	public DateTime occ_time { get; set; }
}
