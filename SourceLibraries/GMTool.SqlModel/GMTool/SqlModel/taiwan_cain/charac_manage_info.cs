using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("charac_manage_info")]
public class charac_manage_info
{
	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	public int tag_charac_no { get; set; }

	public byte striker_skill_index { get; set; }

	public short max_equip_level { get; set; }
}
