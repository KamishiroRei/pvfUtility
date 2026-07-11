using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("member_join_info")]
public class member_join_info
{
	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	public int reg_date { get; set; }

	public string ip { get; set; }

	public byte contry_code { get; set; }

	public int login_time { get; set; }

	public byte error_type { get; set; }

	public string login_ip { get; set; }

	public byte game_use_history { get; set; }
}
