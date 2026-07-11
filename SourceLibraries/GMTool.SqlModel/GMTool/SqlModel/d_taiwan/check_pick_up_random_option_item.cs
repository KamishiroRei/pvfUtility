using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("check_pick_up_random_option_item")]
public class check_pick_up_random_option_item
{
	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	public byte check_count { get; set; }
}
