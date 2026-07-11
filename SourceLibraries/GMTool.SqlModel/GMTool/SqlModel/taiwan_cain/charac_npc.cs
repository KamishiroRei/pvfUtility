using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("charac_npc")]
public class charac_npc
{
	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	public byte npc_cnt { get; set; }

	public byte[] npc_data { get; set; }
}
