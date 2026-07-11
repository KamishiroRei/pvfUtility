using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain_auction_crea;

[SugarTable("auction_system_main")]
public class auction_system_main
{
	[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
	public int sys_auction_id { get; set; }

	public DateTime? occ_time { get; set; }

	public int? regist_interval { get; set; }

	public DateTime? regist_time { get; set; }

	public DateTime? start_date { get; set; }

	public DateTime? end_date { get; set; }

	public short? expire_interval { get; set; }

	public DateTime? last_auction_time { get; set; }

	public DateTime? expected_regist_time { get; set; }
}
