using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("db_connect")]
public class db_connect
{
	public int no { get; set; }

	public string host_name { get; set; }

	public byte? db_server_group { get; set; }

	public int db_type { get; set; }

	public string db_name { get; set; }

	public string db_ip { get; set; }

	public int db_port { get; set; }

	public string db_userid { get; set; }

	public string db_passwd { get; set; }

	public string comments { get; set; }
}
