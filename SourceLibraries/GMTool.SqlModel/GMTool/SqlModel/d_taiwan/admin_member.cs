using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("admin_member")]
public class admin_member
{
	[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
	public int no { get; set; }

	public string user_id { get; set; }

	public string password { get; set; }

	public string name { get; set; }

	public string email { get; set; }

	public string phone { get; set; }

	public string msn { get; set; }

	public string comment { get; set; }

	public int? reg_date { get; set; }

	public string confirm { get; set; }

	public string level { get; set; }

	public string level_group1 { get; set; }

	public string level_group2 { get; set; }

	public string level_group3 { get; set; }

	public string level_group4 { get; set; }

	public string level_group5 { get; set; }

	public string level_group6 { get; set; }
}
