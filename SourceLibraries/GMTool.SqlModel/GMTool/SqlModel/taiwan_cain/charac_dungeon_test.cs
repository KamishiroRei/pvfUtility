using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("charac_dungeon_test")]
public class charac_dungeon_test
{
	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	public byte[] dungeon { get; set; }
}
