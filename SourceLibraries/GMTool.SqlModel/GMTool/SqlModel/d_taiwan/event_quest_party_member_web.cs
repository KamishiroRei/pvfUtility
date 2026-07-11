using System;
using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("event_quest_party_member_web")]
public class event_quest_party_member_web
{
	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public byte server_id { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public int quest_no { get; set; }

	public DateTime occ_time { get; set; }

	public int send_charac_no { get; set; }
}
