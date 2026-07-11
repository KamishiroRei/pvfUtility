using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain_2nd;

[SugarTable("tmp_charac")]
public class tmp_charac
{
	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }
}
