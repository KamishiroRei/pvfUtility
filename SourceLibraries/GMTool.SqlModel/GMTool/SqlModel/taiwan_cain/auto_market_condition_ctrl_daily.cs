using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("auto_market_condition_ctrl_daily")]
public class auto_market_condition_ctrl_daily
{
	[SugarColumn(IsPrimaryKey = true)]
	public DateTime occ_time { get; set; }

	public long total_gold { get; set; }

	public long auction_gold { get; set; }

	public long over_gold { get; set; }

	public long optimum_gold_supply { get; set; }

	public int gold_phase { get; set; }

	public int item_phase { get; set; }

	public int durability_phase { get; set; }
}
