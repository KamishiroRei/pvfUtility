using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("dnf_master_charac")]
public class dnf_master_charac
{
	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public byte global_type { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public byte server_id { get; set; }

	public int charac_no { get; set; }

	public string charac_name { get; set; }

	public byte job { get; set; }

	public byte lev { get; set; }
}
