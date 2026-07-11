using System;
using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("dnf_game_message")]
public class dnf_game_message
{
	[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
	public int no { get; set; }

	public string message { get; set; }

	public byte display_type { get; set; }

	public byte start_h { get; set; }

	public byte end_h { get; set; }

	public DateTime occ_date { get; set; }
}
