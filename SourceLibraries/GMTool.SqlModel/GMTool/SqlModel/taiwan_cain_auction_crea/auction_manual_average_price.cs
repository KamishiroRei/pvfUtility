using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain_auction_crea;

[SugarTable("auction_manual_average_price")]
public class auction_manual_average_price
{
	[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
	public int no { get; set; }

	public int item_id { get; set; }

	public byte upgrade { get; set; }

	public int average_price { get; set; }

	public byte is_apply { get; set; }
}
