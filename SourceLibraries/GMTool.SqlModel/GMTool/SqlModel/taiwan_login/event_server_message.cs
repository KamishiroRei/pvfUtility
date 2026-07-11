using SqlSugar;

namespace GMTool.SqlModel.taiwan_login;

[SugarTable("event_server_message")]
public class event_server_message
{
	public byte server_info { get; set; }

	public byte channel_no { get; set; }

	public string kind { get; set; }

	public string message_index { get; set; }

	public string charac_name { get; set; }

	public string message { get; set; }

	public int update_time { get; set; }
}
