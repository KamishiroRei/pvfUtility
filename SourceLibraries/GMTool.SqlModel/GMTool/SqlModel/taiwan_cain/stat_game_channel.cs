using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("stat_game_channel")]
public class stat_game_channel
{
	public string gc_channel { get; set; }

	public DateTime gc_up_time { get; set; }

	public short gc_now { get; set; }
}
