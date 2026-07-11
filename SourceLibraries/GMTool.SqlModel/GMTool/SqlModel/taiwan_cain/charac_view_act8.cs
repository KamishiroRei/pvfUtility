using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("charac_view_act8")]
public class charac_view_act8
{
	[SugarColumn(IsPrimaryKey = true)]
	public long m_id { get; set; }

	public byte[] info { get; set; }

	public byte slot_effect_count { get; set; }

	public byte charac_slot_limit { get; set; }

	public string hash_key { get; set; }
}
