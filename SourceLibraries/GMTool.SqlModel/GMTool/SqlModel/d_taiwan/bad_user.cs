using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("bad_user")]
public class bad_user
{
	[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
	public int no { get; set; }

	public int m_id { get; set; }

	public int bad_code { get; set; }

	public int create_day { get; set; }

	public int exit_day { get; set; }

	public int admin_n { get; set; }
}
