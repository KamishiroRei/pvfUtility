using System;
using SqlSugar;
using Utools;

namespace GMTool.SqlModel.taiwan_billing;

[SugarTable("taiwan_billing.log_purchase_history")]
public class log_purchase_history
{
	private string _charac_id;

	[SugarColumn(IsPrimaryKey = true)]
	public long tran_id { get; set; }

	public byte tran_state { get; set; }

	public string account_id { get; set; }

	public string charac_id
	{
		get
		{
			string text = CodingHelper.Latin1ToGbkNew(_charac_id);
			if (charac_no != 0)
			{
				return text;
			}
			return text + "(已改名)";
		}
		set
		{
			_charac_id = value;
		}
	}

	public int item_id { get; set; }

	public int cera { get; set; }

	public int befor_cera { get; set; }

	public int after_cera { get; set; }

	public string query_user { get; set; }

	public DateTime occ_date { get; set; }

	[SugarColumn(IsIgnore = true)]
	public string accountname { get; set; }

	[SugarColumn(IsIgnore = true)]
	public int charac_no { get; set; }

	[SugarColumn(IsIgnore = true)]
	public int NowCre { get; set; }

	[SugarColumn(IsIgnore = true)]
	public string occ_dateStr => occ_date.ToString("yyy-MM-dd HH:mm:ss");
}
