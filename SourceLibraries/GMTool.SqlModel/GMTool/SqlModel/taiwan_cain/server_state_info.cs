using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("server_state_info")]
public class server_state_info
{
	[SugarColumn(IsPrimaryKey = true)]
	public int category { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public int code { get; set; }

	public byte[] state { get; set; }

	public DateTime start_time { get; set; }

	public DateTime end_time { get; set; }
}
