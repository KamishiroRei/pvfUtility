using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain_2nd;

[SugarTable("skill_index")]
public class skill_index
{
	[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
	public int no { get; set; }

	public int job { get; set; }

	public int skill_idx { get; set; }

	public string skill_name { get; set; }
}
