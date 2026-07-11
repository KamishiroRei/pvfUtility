using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("item_making_skill_info")]
public class item_making_skill_info
{
	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	public short weapon { get; set; }

	public short cloth { get; set; }

	public short leather { get; set; }

	public short light_armor { get; set; }

	public short heavy_armor { get; set; }

	public short plate { get; set; }

	public short amulet { get; set; }

	public short wrist { get; set; }

	public short ring { get; set; }

	public short support { get; set; }

	public short magic_stone { get; set; }
}
