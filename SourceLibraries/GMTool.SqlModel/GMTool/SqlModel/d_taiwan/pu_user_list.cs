using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("pu_user_list")]
public class pu_user_list
{
	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }
}
