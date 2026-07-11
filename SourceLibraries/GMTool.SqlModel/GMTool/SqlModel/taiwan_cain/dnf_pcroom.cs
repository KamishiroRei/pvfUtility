using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

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

	public string ip { get; set; }

	public byte start_ip { get; set; }

	public byte end_ip { get; set; }
}
