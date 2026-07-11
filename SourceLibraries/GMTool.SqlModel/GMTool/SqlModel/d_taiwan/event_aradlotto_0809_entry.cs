using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("event_aradlotto_0809_entry")]
public class event_aradlotto_0809_entry
{
	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	public int occ_date { get; set; }

	public string lotto_num { get; set; }
}
