using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("quest_category")]
public class quest_category
{
	[SugarColumn(IsPrimaryKey = true)]
	public int quest_idx { get; set; }

	public string quest_name { get; set; }
}
