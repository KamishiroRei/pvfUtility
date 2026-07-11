using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("log_num_occupations")]
public class log_num_occupations
{
	[SugarColumn(IsPrimaryKey = true)]
	public DateTime occ_time { get; set; }

	public int num_occupations_charscreen { get; set; }

	public int num_occupations_seriaroom { get; set; }

	public int num_login_per_min { get; set; }

	public int num_logout_per_min { get; set; }
}
