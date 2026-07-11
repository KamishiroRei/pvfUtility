using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("member_dungeon")]
public class member_dungeon
{
	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	public string dungeon { get; set; }
}
