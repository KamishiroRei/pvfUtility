using SqlSugar;

namespace GMTool.SqlModel.taiwan_login;

[SugarTable("gm_manifest")]
public class gm_manifest
{
	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	public byte level { get; set; }
}
