using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("gm_manifest_notuse")]
public class gm_manifest_notuse
{
	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	public byte level { get; set; }
}
