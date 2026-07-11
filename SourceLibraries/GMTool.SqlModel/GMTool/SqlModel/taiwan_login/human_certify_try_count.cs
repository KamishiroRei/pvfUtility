using SqlSugar;

namespace GMTool.SqlModel.taiwan_login;

[SugarTable("human_certify_try_count")]
public class human_certify_try_count
{
	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	public int count { get; set; }
}
