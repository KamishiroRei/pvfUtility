using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("charac_tower_rank")]
public class charac_tower_rank
{
	[SugarColumn(IsPrimaryKey = true)]
	public byte tower_index { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public byte part_type { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	public string member_info { get; set; }

	public short rank { get; set; }
}
