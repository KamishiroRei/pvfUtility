using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("charac_housing_info")]
public class charac_housing_info
{
	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	public short installed { get; set; }

	public byte[] decoration_inven { get; set; }

	public short version { get; set; }
}
