using System;
using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("event_quizquiz_stamp")]
public class event_quizquiz_stamp
{
	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public byte degree { get; set; }

	public int stamp { get; set; }

	public DateTime occ_time { get; set; }
}
