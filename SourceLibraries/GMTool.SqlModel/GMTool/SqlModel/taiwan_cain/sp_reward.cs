using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("sp_reward")]
public class sp_reward
{
	public int grade { get; set; }

	public int sp { get; set; }
}
