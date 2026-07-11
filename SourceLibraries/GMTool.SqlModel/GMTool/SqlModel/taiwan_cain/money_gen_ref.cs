using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("money_gen_ref")]
public class money_gen_ref
{
	public int grade { get; set; }

	public int bottom_grade { get; set; }

	public int money { get; set; }

	public int random_value { get; set; }
}
