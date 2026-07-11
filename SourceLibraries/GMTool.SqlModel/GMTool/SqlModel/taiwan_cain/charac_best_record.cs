using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("charac_best_record")]
public class charac_best_record
{
	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public short dungeon_no { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public short difficulty { get; set; }

	public int style { get; set; }

	public int technic { get; set; }

	public int attacked { get; set; }

	public int rank { get; set; }
}
