using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_billing;

[SugarTable("log_transaction_history")]
public class log_transaction_history
{
	[SugarColumn(IsPrimaryKey = true)]
	public long tran_id { get; set; }

	public byte tran_type { get; set; }

	public DateTime occ_date { get; set; }
}
