using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("guild_bbs")]
public class guild_bbs
{
	[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
	public int gno { get; set; }

	public byte bd_id { get; set; }

	public byte empyn { get; set; }

	public int mgno { get; set; }

	public byte open { get; set; }

	public byte main { get; set; }

	public int reg_date { get; set; }

	public int mod_date { get; set; }

	public int hits { get; set; }

	public string body_type { get; set; }

	public int m_id { get; set; }

	public string reg_id { get; set; }

	public string subject { get; set; }
}
