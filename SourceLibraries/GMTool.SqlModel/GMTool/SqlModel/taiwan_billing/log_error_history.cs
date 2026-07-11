using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_billing;

[SugarTable("log_error_history")]
public class log_error_history
{
	[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
	public int no { get; set; }

	public int error_id { get; set; }

	public string error_msg { get; set; }

	public string error_query { get; set; }

	public string proc_name { get; set; }

	public int proc_line { get; set; }

	public string query_user { get; set; }

	public DateTime occ_date { get; set; }
}
