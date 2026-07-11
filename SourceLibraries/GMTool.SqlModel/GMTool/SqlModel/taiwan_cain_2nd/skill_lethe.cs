using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain_2nd;

[SugarTable("skill_lethe")]
public class skill_lethe
{
	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	public byte[] skill_slot { get; set; }

	public byte flag { get; set; }
}
