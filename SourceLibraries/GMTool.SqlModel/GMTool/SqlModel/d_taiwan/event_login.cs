using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("event_login")]
public class event_login
{
	[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
	public int id { get; set; }

	public string item_id { get; set; }

	public int item_num { get; set; }

	public int Db { get; set; }

	public int yxb_num { get; set; }

	public string name { get; set; }
}
