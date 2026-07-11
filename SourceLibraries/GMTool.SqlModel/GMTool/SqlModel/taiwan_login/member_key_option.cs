using SqlSugar;

namespace GMTool.SqlModel.taiwan_login;

[SugarTable("member_key_option")]
public class member_key_option
{
	[SugarColumn(IsPrimaryKey = true)]
	public long m_id { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public byte key_type { get; set; }

	public byte[] key_option { get; set; }
}
