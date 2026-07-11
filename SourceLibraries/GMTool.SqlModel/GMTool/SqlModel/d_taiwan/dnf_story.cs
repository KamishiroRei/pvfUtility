using System;
using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("dnf_story")]
public class dnf_story
{
	[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
	public int no { get; set; }

	public byte story_type { get; set; }

	public byte notice_flag { get; set; }

	public int m_id { get; set; }

	public string reg_id { get; set; }

	public string title { get; set; }

	public string url { get; set; }

	public string img_name { get; set; }

	public byte opt { get; set; }

	public string open_flag { get; set; }

	public DateTime reg_date { get; set; }

	public int hits { get; set; }

	public int reserve_time { get; set; }

	public string content { get; set; }
}
