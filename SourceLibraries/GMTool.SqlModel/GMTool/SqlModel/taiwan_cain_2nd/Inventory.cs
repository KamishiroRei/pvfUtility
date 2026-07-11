using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain_2nd;

[SugarTable("inventory")]
public class Inventory
{
	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	public int money { get; set; }

	public int coin { get; set; }

	public byte[] inventory { get; set; }

	public byte[] equipslot { get; set; }

	public int pay_coin { get; set; }

	public int event_coin { get; set; }

	public byte[] creature { get; set; }

	public byte creature_flag { get; set; }

	public byte[] katagaki { get; set; }

	public int inventory_capacity { get; set; }

	public int avatar_coin { get; set; }
}
