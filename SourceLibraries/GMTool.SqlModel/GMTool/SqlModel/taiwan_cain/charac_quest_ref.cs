using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("charac_quest_ref")]
public class charac_quest_ref
{
	[SugarColumn(IsPrimaryKey = true)]
	public int origin_idx { get; set; }

	public int mapped_idx { get; set; }
}
