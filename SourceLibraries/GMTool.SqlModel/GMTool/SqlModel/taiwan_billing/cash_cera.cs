using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_billing;

[SugarTable("taiwan_billing.cash_cera")]
public class cash_cera
{
	[SugarColumn(IsIgnore = true)]
	public string accountname { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public string account { get; set; }

	public int cera { get; set; }

	public long mod_tran { get; set; }

	public DateTime mod_date { get; set; }

	public DateTime reg_date { get; set; }
}
