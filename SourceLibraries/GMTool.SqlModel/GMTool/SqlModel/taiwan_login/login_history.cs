using SqlSugar;

namespace GMTool.SqlModel.taiwan_login;

[SugarTable("login_history")]
public class login_history
{
	public int m_id { get; set; }

	public int occ_time { get; set; }

	public byte trigger { get; set; }
}
