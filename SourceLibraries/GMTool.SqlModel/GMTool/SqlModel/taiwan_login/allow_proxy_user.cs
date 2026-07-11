using SqlSugar;

namespace GMTool.SqlModel.taiwan_login;

[SugarTable("allow_proxy_user")]
public class allow_proxy_user
{
	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }
}
