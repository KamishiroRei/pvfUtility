using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("charac_cerashop_restrict")]
public class charac_cerashop_restrict
{
	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public int ipg_no { get; set; }

	public int count { get; set; }

	public int next_date { get; set; }

	public int end_date { get; set; }

	public int last_access_date { get; set; }
}
