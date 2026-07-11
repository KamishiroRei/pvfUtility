using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("member_booster_gage")]
public class member_booster_gage
{
	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	public byte gage { get; set; }
}
