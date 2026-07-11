using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain_auction_crea;

[SugarTable("auction_system_iteminfo")]
public class auction_system_iteminfo
{
	public int? sys_auction_id { get; set; }

	public short? probability { get; set; }

	public int? price { get; set; }

	public byte? seal_flag { get; set; }

	public int? item_id { get; set; }

	public int? add_info { get; set; }

	public byte? upgrade { get; set; }

	public byte? seal_cnt { get; set; }

	public short? endurance { get; set; }

	public int? extend_info { get; set; }
}
