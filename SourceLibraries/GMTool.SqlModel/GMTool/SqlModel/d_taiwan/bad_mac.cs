using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("bad_mac")]
public class bad_mac
{
	[SugarColumn(IsPrimaryKey = true)]
	public string mac { get; set; }
}
