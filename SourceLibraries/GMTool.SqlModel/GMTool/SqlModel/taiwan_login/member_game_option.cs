using SqlSugar;

namespace GMTool.SqlModel.taiwan_login;

[SugarTable("member_game_option")]
public class member_game_option
{
	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	public byte[] option_1 { get; set; }

	public byte[] option_2 { get; set; }

	public byte[] option_3 { get; set; }

	public byte[] shortcut_emoticon { get; set; }
}
