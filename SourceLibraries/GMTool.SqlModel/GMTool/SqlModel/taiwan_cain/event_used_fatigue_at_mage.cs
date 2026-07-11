using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("event_used_fatigue_at_mage")]
public class event_used_fatigue_at_mage
{
	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	public int fatigue_quantity { get; set; }
}
