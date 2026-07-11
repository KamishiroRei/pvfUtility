using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_login;

[SugarTable("village_attacked_server_time_rank")]
public class village_attacked_server_time_rank
{
	[SugarColumn(IsPrimaryKey = true)]
	public byte server_info { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public DateTime occ_date { get; set; }

	public int clear_time { get; set; }

	public byte rank { get; set; }
}
