using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain_auction_crea;

[SugarTable("auction_roi_average_price")]
public class auction_roi_average_price
{
	[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
	public int no { get; set; }

	public int item_id { get; set; }

	public byte upgrade { get; set; }

	public long roi_high_key { get; set; }

	public int roi_low_key { get; set; }

	public short roi_index1 { get; set; }

	public short roi_index2 { get; set; }

	public short roi_index3 { get; set; }

	public int average_price { get; set; }

	public int real_purchase_count { get; set; }

	public byte seperate_upgrade { get; set; }
}
