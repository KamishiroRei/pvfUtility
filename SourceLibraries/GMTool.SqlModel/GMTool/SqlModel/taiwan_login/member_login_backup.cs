using SqlSugar;

namespace GMTool.SqlModel.taiwan_login;

[SugarTable("member_login_backup")]
public class member_login_backup
{
	public int m_id { get; set; }

	public int login_time { get; set; }

	public int expire_time { get; set; }

	public int last_play_time { get; set; }

	public int total_account_fail { get; set; }

	public byte account_fail { get; set; }

	public int report_cnt { get; set; }

	public byte reliable_flag { get; set; }

	public int trade_gold_daily { get; set; }

	public int last_gift_time { get; set; }

	public short gift_cnt { get; set; }

	public string login_ip { get; set; }

	public byte security_flag { get; set; }

	public byte power_side { get; set; }

	public int dungeon_gain_gold { get; set; }

	public int school_id { get; set; }

	public float rating { get; set; }

	public int cleanpad_point { get; set; }

	public string tutorial_skipable { get; set; }
}
