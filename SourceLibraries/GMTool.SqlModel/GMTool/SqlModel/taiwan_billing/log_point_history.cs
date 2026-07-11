using System;
using SqlSugar;
using Utools;

namespace GMTool.SqlModel.taiwan_billing;

[SugarTable("taiwan_billing.log_point_history")]
public class log_point_history
{
	private string _charac_id;

	[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
	public int no { get; set; }

	public string account_id { get; set; }

	public string charac_id
	{
		get
		{
			return CodingHelper.Latin1ToGbkNew(_charac_id);
		}
		set
		{
			_charac_id = value;
		}
	}

	public int cera_point { get; set; }

	public string command { get; set; }

	public byte charge_type { get; set; }

	public byte free_charge_type { get; set; }

	public int item_id { get; set; }

	public string query_user { get; set; }

	public DateTime reg_date { get; set; }

	[SugarColumn(IsIgnore = true)]
	public string accountname { get; set; }

	[SugarColumn(IsIgnore = true)]
	public int charac_no { get; set; }

	[SugarColumn(IsIgnore = true)]
	public int NowCre { get; set; }

	[SugarColumn(IsIgnore = true)]
	public string occ_dateStr => reg_date.ToString("yyy-MM-dd HH:mm:ss");
}
