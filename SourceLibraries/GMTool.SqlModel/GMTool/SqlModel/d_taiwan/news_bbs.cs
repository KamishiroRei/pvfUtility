using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("news_bbs")]
public class news_bbs
{
	[SugarColumn(IsPrimaryKey = true)]
	public byte bbs_code { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public byte emph_yn { get; set; }

	[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
	public int no { get; set; }

	public string user_id { get; set; }

	public int reg_date { get; set; }

	public byte? html_yn { get; set; }

	public string subject { get; set; }

	public string body { get; set; }

	public short hits { get; set; }

	public int prev_no { get; set; }

	public int next_no { get; set; }

	public int? updt_date { get; set; }

	public byte use_yn { get; set; }

	public string file_name { get; set; }
}
