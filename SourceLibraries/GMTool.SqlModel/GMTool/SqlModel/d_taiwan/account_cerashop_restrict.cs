using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("account_cerashop_restrict")]
public class account_cerashop_restrict
{
	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public int ipg_no { get; set; }

	public int count { get; set; }

	public int next_date { get; set; }

	public int end_date { get; set; }

	public int last_access_date { get; set; }
}
