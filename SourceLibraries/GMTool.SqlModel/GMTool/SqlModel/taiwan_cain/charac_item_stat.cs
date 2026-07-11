using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("charac_item_stat")]
public class charac_item_stat
{
	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	public byte[] cooltime_item { get; set; }

	public byte[] effect_item { get; set; }

	public byte[] check_flag { get; set; }
}
