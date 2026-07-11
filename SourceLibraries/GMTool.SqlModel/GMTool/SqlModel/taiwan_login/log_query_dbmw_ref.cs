using SqlSugar;

namespace GMTool.SqlModel.taiwan_login;

[SugarTable("log_query_dbmw_ref")]
public class log_query_dbmw_ref
{
	public string query_hash { get; set; }

	[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
	public short q_id { get; set; }

	public string query { get; set; }
}
