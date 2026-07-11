using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("limit_npc_item")]
public class limit_npc_item
{
	[SugarColumn(IsPrimaryKey = true)]
	public int item_index { get; set; }

	public int max_count { get; set; }

	public int sell_count { get; set; }
}
