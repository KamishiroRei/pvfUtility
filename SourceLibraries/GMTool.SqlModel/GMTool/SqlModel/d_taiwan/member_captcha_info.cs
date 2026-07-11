using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("member_captcha_info")]
public class member_captcha_info
{
	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	public int cert_time { get; set; }

	public byte fail_count { get; set; }
}
