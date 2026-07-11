using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("eco_point")]
public class Eco_point
{
	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	public int eco_point { get; set; }

	public byte point_500 { get; set; }

	public byte point_300 { get; set; }

	public byte point_100 { get; set; }

	public byte point_50 { get; set; }

	public byte point_20 { get; set; }
}
