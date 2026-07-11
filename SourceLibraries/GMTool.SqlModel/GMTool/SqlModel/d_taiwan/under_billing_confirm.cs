using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("under_billing_confirm")]
public class under_billing_confirm
{
	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	public string parent_name { get; set; }

	public long parent_jumin { get; set; }

	public byte parent_phone1 { get; set; }

	public short parent_phone2 { get; set; }

	public short parent_phone3 { get; set; }

	public string parent_email { get; set; }

	public byte parent_consent_type { get; set; }

	public int create_date { get; set; }

	public int consent_date { get; set; }

	public byte consent_yn { get; set; }
}
