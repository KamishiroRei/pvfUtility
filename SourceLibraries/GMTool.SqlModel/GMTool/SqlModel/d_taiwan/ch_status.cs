using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("ch_status")]
public class ch_status
{
	public byte gc_group { get; set; }

	public byte gc_status { get; set; }
}
