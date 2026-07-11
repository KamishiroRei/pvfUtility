using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain_2nd;

[SugarTable("village_attack_dungeon")]
public class village_attack_dungeon
{
	[SugarColumn(IsPrimaryKey = true)]
	public DateTime occ_date { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	public byte attack_count { get; set; }

	public byte revenge_dungeon { get; set; }
}
