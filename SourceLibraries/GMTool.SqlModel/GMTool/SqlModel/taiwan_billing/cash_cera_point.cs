using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_billing;

[SugarTable("cash_cera_point")]
public class cash_cera_point
{
	[SugarColumn(IsPrimaryKey = true)]
	public string account { get; set; }

	[SugarColumn(IsIgnore = true)]
	public string accountname { get; set; }

	public int cera_point { get; set; }

	public DateTime reg_date { get; set; }

	public DateTime mod_date { get; set; }
}
