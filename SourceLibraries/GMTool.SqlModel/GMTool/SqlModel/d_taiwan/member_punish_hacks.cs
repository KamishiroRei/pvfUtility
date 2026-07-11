using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("member_punish_hacks")]
public class member_punish_hacks
{
	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	public int occ_time { get; set; }

	public int period { get; set; }

	public byte now_flag { get; set; }

	public byte auto_flag { get; set; }

	public string reason { get; set; }

	public string hack_ip { get; set; }
}
