using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain_auction_crea;

[SugarTable("auction_roi_constraint")]
public class auction_roi_constraint
{
	public int db_inf_max_price { get; set; }

	public int db_inf_min_price { get; set; }

	public int db_inf_prob { get; set; }

	public int db_inf_limit_count { get; set; }

	public int db_inf_base_mul_min_a { get; set; }

	public int db_inf_base_mul_max_b { get; set; }

	public DateTime last_update_date { get; set; }
}
