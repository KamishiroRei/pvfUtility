using System;
using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("member_security_grade")]
public class member_security_grade
{
	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	public DateTime last_visit_time { get; set; }

	public int pass_fail_cnt { get; set; }

	public DateTime last_vaccine_date { get; set; }

	public DateTime last_window_date { get; set; }

	public DateTime goblin_pass_mod { get; set; }

	public int goblin_fail_cnt { get; set; }

	public DateTime security_card_reg { get; set; }

	public int security_card_fail_cnt { get; set; }

	public DateTime m_opt_reg { get; set; }

	public DateTime pc_opt_reg { get; set; }

	public DateTime black_ip_try_time { get; set; }

	public int linear_pass_fail_cnt { get; set; }

	public int last_pass_fail_time { get; set; }

	public DateTime last_check_time { get; set; }

	public DateTime pass_modify_check { get; set; }

	public DateTime member_pc_reg { get; set; }

	public DateTime gatekeeper_otp_reg { get; set; }

	public int goblin_validity_time { get; set; }

	public int security_card_validity_time { get; set; }

	public string validity_ip { get; set; }

	public byte cargopad_status { get; set; }

	public DateTime cargopad_mod { get; set; }

	public int cargopad_validity_time { get; set; }
}
