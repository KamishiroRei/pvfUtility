using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain_2nd;

[SugarTable("charac_item_lock_info")]
public class charac_item_lock_info
{
	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	public byte[] item_lock_info { get; set; }
}
