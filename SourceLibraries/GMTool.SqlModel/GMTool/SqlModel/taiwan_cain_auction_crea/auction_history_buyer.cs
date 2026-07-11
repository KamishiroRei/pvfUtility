using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain_auction_crea;

[SugarTable("auction_history_buyer")]
public class auction_history_buyer
{
	public long? auction_id { get; set; }

	public DateTime? occ_time { get; set; }

	public int? pre_buyer_id { get; set; }

	public int? buyer_id { get; set; }

	public int? pre_price { get; set; }

	public int? price { get; set; }

	public int? pre_buyer_postal_id { get; set; }

	public int commission { get; set; }
}
