using SqlSugar;

namespace GMTool.SqlModel.d_guild;

[SugarTable("zdydmb")]
public class zdydmb
{
	[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
	public int id { get; set; }

	public string name { get; set; }

	public string dm { get; set; }
}
