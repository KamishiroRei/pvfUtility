using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("exp_level_ref")]
public class exp_level_ref
{
	public int exp { get; set; }

	public int lev { get; set; }
}
