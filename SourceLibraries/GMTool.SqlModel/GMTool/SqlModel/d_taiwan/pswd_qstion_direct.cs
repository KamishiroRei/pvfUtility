using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("pswd_qstion_direct")]
public class pswd_qstion_direct
{
	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	public string q_text { get; set; }
}
