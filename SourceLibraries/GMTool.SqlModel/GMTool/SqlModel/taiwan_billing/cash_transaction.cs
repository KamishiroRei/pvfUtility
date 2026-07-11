using SqlSugar;

namespace GMTool.SqlModel.taiwan_billing;

[SugarTable("cash_transaction")]
public class cash_transaction
{
	[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
	public long tran_id { get; set; }

	public string dummy { get; set; }
}
