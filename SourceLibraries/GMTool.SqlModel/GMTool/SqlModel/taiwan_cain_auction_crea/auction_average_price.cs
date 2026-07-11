using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain_auction_crea;

[SugarTable("auction_average_price")]
public class auction_average_price
{
	[SugarColumn(IsPrimaryKey = true)]
	public int item_id { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public byte upgrade { get; set; }

	public int? average_price { get; set; }

	public byte seperate_upgrade { get; set; }
}
