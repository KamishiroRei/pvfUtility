using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("event_levelup_support")]
public class event_levelup_support
{
	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public int level { get; set; }

	public int? state { get; set; }
}
