using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("dnf_event_prize")]
public class dnf_event_prize
{
	[SugarColumn(IsPrimaryKey = true)]
	public int prize_id { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	public int check_time { get; set; }
}
