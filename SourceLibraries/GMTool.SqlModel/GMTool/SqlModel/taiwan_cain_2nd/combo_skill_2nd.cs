using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain_2nd;

[SugarTable("combo_skill_2nd")]
public class combo_skill_2nd
{
	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public int combo_idx { get; set; }

	public short value1 { get; set; }

	public short value2 { get; set; }

	public short value3 { get; set; }

	public short value4 { get; set; }

	public short value5 { get; set; }

	public short value6 { get; set; }
}
