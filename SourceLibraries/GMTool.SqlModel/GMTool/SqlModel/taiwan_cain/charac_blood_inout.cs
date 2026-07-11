using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("charac_blood_inout")]
public class charac_blood_inout
{
	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	public byte dungeon1 { get; set; }

	public byte dungeon2 { get; set; }

	public byte dungeon3 { get; set; }

	public byte dungeon4 { get; set; }

	public byte dungeon5 { get; set; }

	public byte dungeon6 { get; set; }

	public byte dungeon7 { get; set; }

	public byte dungeon8 { get; set; }

	public byte dungeon9 { get; set; }

	public byte dungeon10 { get; set; }
}
