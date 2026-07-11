using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain_auction_gold;

[Serializable]
[SugarTable("auction_main")]
public class AuctionMain_GMTool : auction_main
{
	[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
	public int auction_id { get; set; }
}
