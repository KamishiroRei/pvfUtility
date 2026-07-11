using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("charac_members")]
public class charac_members
{
	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	public int master_no { get; set; }

	public int exp { get; set; }

	public DateTime create_time { get; set; }

	public DateTime delete_time { get; set; }
}
