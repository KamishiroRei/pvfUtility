using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("dnf_charac_mov")]
public class dnf_charac_mov
{
	[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
	public int id { get; set; }

	public int m_id { get; set; }

	public byte server_id { get; set; }

	public int charac_no { get; set; }

	public byte move_server_id { get; set; }

	public int move_charac_no { get; set; }

	public int move_check { get; set; }
}
