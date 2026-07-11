using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("charac_blood_best_record")]
public class charac_blood_best_record
{
	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public int dungeon_index { get; set; }

	public byte best_round { get; set; }

	public int best_time { get; set; }
}
