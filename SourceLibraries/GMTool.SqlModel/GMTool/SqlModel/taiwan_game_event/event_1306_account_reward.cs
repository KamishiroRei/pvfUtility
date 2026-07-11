using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_game_event;

[SugarTable("taiwan_game_event.event_1306_account_reward")]
public class event_1306_account_reward
{
	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	public int charac_no { get; set; }

	public DateTime occ_date { get; set; }
}
