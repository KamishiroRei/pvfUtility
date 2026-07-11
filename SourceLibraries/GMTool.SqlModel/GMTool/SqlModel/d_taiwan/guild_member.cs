using System;
using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("guild_member")]
public class guild_member
{
	[SugarColumn(IsPrimaryKey = true)]
	public int guild_id { get; set; }

	public int m_id { get; set; }

	public byte server_id { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	public string charac_name { get; set; }

	public string nick_name { get; set; }

	public byte grade { get; set; }

	public byte job { get; set; }

	public byte grow_type { get; set; }

	public byte lev { get; set; }

	public byte age { get; set; }

	public string born_year { get; set; }

	public string sex { get; set; }

	public DateTime apply_time { get; set; }

	public DateTime member_time { get; set; }

	public byte member_flag { get; set; }

	public short bbs_cnt { get; set; }

	public DateTime last_visit_time { get; set; }

	public byte secede_type { get; set; }

	public DateTime secede_time { get; set; }

	public int member_point { get; set; }

	public int member_point_prev { get; set; }

	public DateTime last_play_time { get; set; }
}
