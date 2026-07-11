using SqlSugar;

namespace GMTool.SqlModel.d_guild;

[SugarTable("power_war_statue_ranker")]
public class power_war_statue_ranker
{
	[SugarColumn(IsPrimaryKey = true)]
	public byte server_id { get; set; }

	public int first_ranker { get; set; }

	public int second_ranker { get; set; }

	public int third_ranker { get; set; }
}
