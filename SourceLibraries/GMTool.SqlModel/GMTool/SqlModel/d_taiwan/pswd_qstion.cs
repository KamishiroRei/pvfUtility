using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("pswd_qstion")]
public class pswd_qstion
{
	[SugarColumn(IsPrimaryKey = true)]
	public byte q_no { get; set; }

	public string q_text { get; set; }
}
