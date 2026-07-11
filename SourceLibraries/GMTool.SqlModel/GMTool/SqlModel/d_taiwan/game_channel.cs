using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("game_channel")]
public class game_channel
{
	[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
	public int gc_no { get; set; }

	public short gc_now { get; set; }

	public string gc_ip { get; set; }

	public short gc_port { get; set; }

	public short gc_max { get; set; }

	public byte gc_game { get; set; }

	public string gc_channel { get; set; }

	public short gc_ch_group { get; set; }

	public string gc_channeltype { get; set; }
}
