using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain_auction_gold;

[SugarTable("auction_history_201608")]
public class auction_history_201608
{
	[SugarColumn(IsPrimaryKey = true)]
	public long auction_id { get; set; }

	public DateTime? start_time { get; set; }

	public DateTime? occ_time { get; set; }

	public byte? event_type { get; set; }

	public int? owner_id { get; set; }

	public int? buyer_id { get; set; }

	public int? price { get; set; }

	public byte? seal_flag { get; set; }

	public int? item_id { get; set; }

	public int? add_info { get; set; }

	public byte? upgrade { get; set; }

	[SugarColumn(IsNullable = true)]
	public byte amplify_option { get; set; }

	[SugarColumn(IsNullable = true)]
	public int amplify_value { get; set; }

	public byte? seal_cnt { get; set; }

	public short? endurance { get; set; }

	public int? extend_info { get; set; }

	public int? owner_postal_id { get; set; }

	public int? buyer_postal_id { get; set; }

	[SugarColumn(IsNullable = true)]
	public int expire_time { get; set; }

	[SugarColumn(IsNullable = true)]
	public int unit_price { get; set; }

	[SugarColumn(IsNullable = true)]
	public string random_option { get; set; }

	[SugarColumn(IsNullable = true)]
	public long roi_high_key { get; set; }

	[SugarColumn(IsNullable = true)]
	public int roi_low_key { get; set; }

	[SugarColumn(IsNullable = true)]
	public byte seperate_upgrade { get; set; }

	[SugarColumn(IsNullable = true)]
	public int commission { get; set; }

	[SugarColumn(IsNullable = true)]
	public byte owner_type { get; set; }

	[SugarColumn(IsNullable = true)]
	public byte[] item_guid { get; set; }
}
