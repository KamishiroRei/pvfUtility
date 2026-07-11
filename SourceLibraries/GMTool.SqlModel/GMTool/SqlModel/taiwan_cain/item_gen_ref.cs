using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("item_gen_ref")]
public class item_gen_ref
{
	public byte item_grade { get; set; }

	public byte rate_type { get; set; }

	public short money_rate { get; set; }

	public short item_rate { get; set; }

	public short free_rate { get; set; }
}
