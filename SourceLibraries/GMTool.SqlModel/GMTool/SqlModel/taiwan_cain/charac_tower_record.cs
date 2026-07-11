using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("charac_tower_record")]
public class charac_tower_record
{
	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	public byte tower_index { get; set; }

	public string member_info_1 { get; set; }

	public byte stage_1 { get; set; }

	public int play_time_1 { get; set; }

	public DateTime occ_time_1 { get; set; }

	public string member_info_2 { get; set; }

	public byte stage_2 { get; set; }

	public int play_time_2 { get; set; }

	public DateTime occ_time_2 { get; set; }

	public string member_info_3 { get; set; }

	public byte stage_3 { get; set; }

	public int play_time_3 { get; set; }

	public DateTime occ_time_3 { get; set; }

	public string member_info_4 { get; set; }

	public byte stage_4 { get; set; }

	public int play_time_4 { get; set; }

	public DateTime occ_time_4 { get; set; }
}
