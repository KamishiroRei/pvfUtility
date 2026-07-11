using System;
using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("member_punish_info_history_2016")]
public class member_punish_info_history_2016
{
	[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
	public int no { get; set; }

	public int m_id { get; set; }

	public int punish_type { get; set; }

	public DateTime occ_time { get; set; }

	public int punish_value { get; set; }

	public byte apply_flag { get; set; }

	public DateTime start_time { get; set; }

	public DateTime end_time { get; set; }

	public string admin_id { get; set; }

	public string reason { get; set; }

	public byte? is_kicked { get; set; }

	public string first_ssn { get; set; }

	public string second_ssn { get; set; }
}
