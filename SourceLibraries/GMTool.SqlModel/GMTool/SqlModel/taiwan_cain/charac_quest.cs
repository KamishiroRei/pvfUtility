using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("charac_quest")]
public class charac_quest
{
	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	public byte[] quest_10 { get; set; }

	public byte[] quest_15 { get; set; }

	public byte[] quest_20 { get; set; }

	public byte[] quest_30 { get; set; }

	public byte[] quest_40 { get; set; }

	public byte[] quest_40_ext { get; set; }

	public byte[] quest_50 { get; set; }

	public byte[] quest_60 { get; set; }

	public byte[] quest_70 { get; set; }

	public byte[] quest_etc { get; set; }

	public short play_1 { get; set; }

	public int play_1_trigger { get; set; }

	public short play_2 { get; set; }

	public int play_2_trigger { get; set; }

	public short play_3 { get; set; }

	public int play_3_trigger { get; set; }

	public short play_4 { get; set; }

	public int play_4_trigger { get; set; }

	public short play_5 { get; set; }

	public int play_5_trigger { get; set; }

	public short play_6 { get; set; }

	public int play_6_trigger { get; set; }

	public short play_7 { get; set; }

	public int play_7_trigger { get; set; }

	public short play_8 { get; set; }

	public int play_8_trigger { get; set; }

	public short play_9 { get; set; }

	public int play_9_trigger { get; set; }

	public short play_10 { get; set; }

	public int play_10_trigger { get; set; }

	public byte[] quest_50_ext { get; set; }

	public byte[] quest_60_ext { get; set; }

	public byte[] quest_etc_ext { get; set; }

	public byte[] quest_60_ext_2nd { get; set; }
}
