using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("under_age_consent")]
public class under_age_consent
{
	public int m_id { get; set; }

	public byte consent_type { get; set; }

	public int limit_money { get; set; }

	public string parent_name { get; set; }

	public long parent_jumin { get; set; }

	public byte parent_phone1 { get; set; }

	public short parent_phone2 { get; set; }

	public short parent_phone3 { get; set; }

	public string parent_email { get; set; }

	public byte parent_consent_type { get; set; }

	public byte notice_type { get; set; }

	public string notice_addr { get; set; }

	public int create_date { get; set; }

	public int consent_date { get; set; }

	public byte consent_yn { get; set; }

	public byte history_yn { get; set; }
}
