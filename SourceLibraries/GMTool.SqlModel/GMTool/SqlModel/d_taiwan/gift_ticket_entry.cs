using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("gift_ticket_entry")]
public class gift_ticket_entry
{
	[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
	public int id { get; set; }

	public short gift_no { get; set; }

	public int buyer_id { get; set; }

	public int buyer_date { get; set; }

	public string buyer_code { get; set; }

	public int buyer_check { get; set; }

	public int other_id { get; set; }

	public int other_date { get; set; }

	public string other_code { get; set; }

	public int other_check { get; set; }

	public string message { get; set; }
}
