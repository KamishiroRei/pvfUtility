using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("channel_occ_info")]
public class channel_occ_info
{
	[SugarColumn(IsPrimaryKey = true)]
	public int gc_no { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public byte age { get; set; }

	public short occ_num { get; set; }
}
