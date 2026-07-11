using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("charac_link_message")]
public class charac_link_message
{
	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	public byte message_flag { get; set; }
}
