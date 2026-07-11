using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("member_abnomal")]
public class member_abnomal
{
	[SugarColumn(IsPrimaryKey = true)]
	public string user_id { get; set; }

	public short overlab_count { get; set; }
}
