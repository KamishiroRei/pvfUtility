using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("dnf_testr_m_id")]
public class dnf_testr_m_id
{
	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	public byte sex { get; set; }
}
