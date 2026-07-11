using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("event_detective_goblin")]
public class event_detective_goblin
{
	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	public int point { get; set; }
}
