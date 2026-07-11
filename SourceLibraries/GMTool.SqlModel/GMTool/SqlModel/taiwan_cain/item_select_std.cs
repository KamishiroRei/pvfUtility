using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("item_select_std")]
public class item_select_std
{
	public int item_grade { get; set; }

	public int top { get; set; }

	public int bottom { get; set; }

	public int weight { get; set; }
}
