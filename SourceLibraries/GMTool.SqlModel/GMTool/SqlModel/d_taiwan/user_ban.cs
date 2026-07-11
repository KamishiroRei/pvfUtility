using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("user_ban")]
public class user_ban
{
	[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
	public int no { get; set; }

	public byte category { get; set; }

	public int m_id { get; set; }

	public short ban_term { get; set; }

	public byte ban_reason { get; set; }

	public string detail_reason { get; set; }

	public int ban_date { get; set; }

	public string cancel_reason { get; set; }

	public int cancel_date { get; set; }

	public int admin_id { get; set; }

	public byte status { get; set; }

	public string first_ssn { get; set; }

	public string second_ssn { get; set; }
}
