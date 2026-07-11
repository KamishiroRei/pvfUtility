using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("account_message")]
public class account_message
{
	[SugarColumn(IsPrimaryKey = true)]
	public int uid { get; set; }

	public string message { get; set; }

	public int? kick { get; set; }
}
