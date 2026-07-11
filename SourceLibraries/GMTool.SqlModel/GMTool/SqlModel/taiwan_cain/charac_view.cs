using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("charac_view")]
public class charac_view
{
	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	public byte[] info { get; set; }

	public byte slot_effect_count { get; set; }

	public byte charac_slot_limit { get; set; }

	public string hash_key { get; set; }

	public byte charac_count { get; set; }
}
