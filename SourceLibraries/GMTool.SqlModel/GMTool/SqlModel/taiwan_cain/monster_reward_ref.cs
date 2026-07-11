using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("monster_reward_ref")]
public class monster_reward_ref
{
	public short level { get; set; }

	public int exp { get; set; }
}
