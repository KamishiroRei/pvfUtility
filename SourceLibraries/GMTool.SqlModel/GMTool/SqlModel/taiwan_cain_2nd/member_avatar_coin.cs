using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain_2nd;

[SugarTable("member_avatar_coin")]
public class member_avatar_coin
{
	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	public int avatar_coin { get; set; }
}
