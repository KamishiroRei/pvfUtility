using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_login;

[SugarTable("village_attacked_charac_point_rank")]
public class village_attacked_charac_point_rank
{
	[SugarColumn(IsPrimaryKey = true)]
	public byte server_info { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public DateTime occ_date { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	public int hunting_point { get; set; }

	public byte rank { get; set; }
}
