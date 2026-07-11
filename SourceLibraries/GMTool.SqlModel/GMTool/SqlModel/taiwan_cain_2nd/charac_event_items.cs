using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain_2nd;

[SugarTable("charac_event_items")]
public class charac_event_items
{
	[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
	public int id { get; set; }

	public int charac_no { get; set; }

	public int it_id { get; set; }

	public int event_code { get; set; }

	public DateTime reg_time { get; set; }

	public DateTime delete_time { get; set; }

	public byte delete_flag { get; set; }

	public int stack_count { get; set; }
}
