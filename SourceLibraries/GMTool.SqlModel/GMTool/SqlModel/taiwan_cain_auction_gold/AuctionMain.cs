using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain_auction_gold;

[Serializable]
[SugarTable("auction_main")]
public class AuctionMain : auction_main
{
	[SugarColumn(IsPrimaryKey = true, IsIdentity = false)]
	public int auction_id { get; set; }
}
