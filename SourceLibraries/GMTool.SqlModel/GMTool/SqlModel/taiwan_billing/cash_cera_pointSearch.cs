using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_billing;

[SugarTable("cash_cera_point")]
public class cash_cera_pointSearch
{
	[SugarColumn(IsPrimaryKey = true)]
	public string account { get; set; }

	public int cera_point { get; set; }

	public DateTime reg_date { get; set; }

	public DateTime mod_date { get; set; }

	[SugarColumn(ColumnName = "cera_point")]
	public int NowCre { get; set; }
}
