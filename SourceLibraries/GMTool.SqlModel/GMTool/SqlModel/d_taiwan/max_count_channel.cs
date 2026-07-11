using System;
using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("max_count_channel")]
public class max_count_channel
{
	public byte server_info { get; set; }

	public string gc_channeltype { get; set; }

	public int mc_max { get; set; }

	public DateTime mc_date { get; set; }
}
