using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("auto_market_condition_ctrl_change")]
public class auto_market_condition_ctrl_change
{
	[SugarColumn(IsPrimaryKey = true)]
	public DateTime occ_time { get; set; }

	public long total_gold_old { get; set; }

	public long over_gold_old { get; set; }

	public long total_gold_new { get; set; }

	public long over_gold_new { get; set; }

	public string MNG_user_id { get; set; }

	public string memo { get; set; }
}
