using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("dnf_pcroom")]
public class dnf_pcroom
{
	[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
	public int ip_no { get; set; }

	public string district { get; set; }

	public string firm_name { get; set; }

	public string telephone { get; set; }

	public string address { get; set; }

	public string leader { get; set; }

	public string start_ip { get; set; }

	public string end_ip { get; set; }
}
