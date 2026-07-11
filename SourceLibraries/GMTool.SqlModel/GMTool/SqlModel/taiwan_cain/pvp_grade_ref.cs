using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("pvp_grade_ref")]
public class pvp_grade_ref
{
	[SugarColumn(IsPrimaryKey = true)]
	public int grade { get; set; }

	public int limit_pts { get; set; }
}
