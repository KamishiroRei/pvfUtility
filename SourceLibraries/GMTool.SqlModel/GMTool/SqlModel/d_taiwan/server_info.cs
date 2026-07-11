using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("server_info")]
public class server_info
{
	public string server_ip { get; set; }

	public string server_name { get; set; }

	public string server_notice { get; set; }

	public string pvf_md5 { get; set; }

	public int? pvf_update { get; set; }

	public string pvf_url { get; set; }

	public string exe_md5 { get; set; }

	public int? exe_update { get; set; }

	public string exe_url { get; set; }

	public int? anti_hung { get; set; }

	public int? simplified { get; set; }

	public int? exempt_input { get; set; }

	public int? soak_point { get; set; }

	public int? give_db { get; set; }

	public string mainhome_url { get; set; }

	public string download_url { get; set; }

	public string paycheck_url { get; set; }

	public int? reglimit { get; set; }

	public int? autolock { get; set; }

	public int? soak_point_type { get; set; }

	public int? give_type { get; set; }

	public string ban_talk { get; set; }

	public int? use_hung { get; set; }

	public int? cancel_limit { get; set; }

	public string map_name_1 { get; set; }

	public string map_name_2 { get; set; }

	public string map_name_3 { get; set; }

	public string map_name_4 { get; set; }

	public string map_name_5 { get; set; }

	public string map_name_6 { get; set; }

	public string map_name_7 { get; set; }

	public string map_name_8 { get; set; }

	public string map_coor_1 { get; set; }

	public string map_coor_2 { get; set; }

	public string map_coor_3 { get; set; }

	public string map_coor_4 { get; set; }

	public string map_coor_5 { get; set; }

	public string map_coor_6 { get; set; }

	public string map_coor_7 { get; set; }

	public string map_coor_8 { get; set; }

	public string login_ver { get; set; }
}
