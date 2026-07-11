using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("charac_friends")]
public class charac_friends
{
	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public int friend_no { get; set; }
}
