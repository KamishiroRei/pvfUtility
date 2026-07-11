using System;
using SqlSugar;
using Utools;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("member_punish_info")]
public class member_punish_info
{
	private string _reason;

	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	[SugarColumn(IsIgnore = true)]
	public string accountname { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public int punish_type { get; set; }

	public DateTime occ_time { get; set; }

	public int punish_value { get; set; }

	public byte apply_flag { get; set; }

	public DateTime start_time { get; set; }

	[SugarColumn(IsIgnore = true)]
	public string start_timeStr => start_time.ToString("yyyy-MM-dd HH:mm:ss");

	public DateTime end_time { get; set; }

	[SugarColumn(IsIgnore = true)]
	public string end_timeStr => end_time.ToString("yyyy-MM-dd HH:mm:ss");

	public string admin_id { get; set; }

	public string reason
	{
		get
		{
			return CodingHelper.Latin1ToGbkNew(_reason);
		}
		set
		{
			_reason = value;
		}
	}
}
