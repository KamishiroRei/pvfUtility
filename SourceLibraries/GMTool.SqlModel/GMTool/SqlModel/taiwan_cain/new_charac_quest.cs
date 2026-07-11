using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("taiwan_cain.new_charac_quest")]
public class new_charac_quest
{
	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	public byte[] clear_quest { get; set; }

	public byte[] quest_notify { get; set; }

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

	public byte auto_clear { get; set; }

	public short play_11 { get; set; }

	public int play_11_trigger { get; set; }

	public short play_12 { get; set; }

	public int play_12_trigger { get; set; }

	public short play_13 { get; set; }

	public int play_13_trigger { get; set; }

	public short play_14 { get; set; }

	public int play_14_trigger { get; set; }

	public short play_15 { get; set; }

	public int play_15_trigger { get; set; }

	public short play_16 { get; set; }

	public int play_16_trigger { get; set; }

	public short play_17 { get; set; }

	public int play_17_trigger { get; set; }

	public short play_18 { get; set; }

	public int play_18_trigger { get; set; }

	public short play_19 { get; set; }

	public int play_19_trigger { get; set; }

	public short play_20 { get; set; }

	public int play_20_trigger { get; set; }

	public short urgentQuestIndex { get; set; }
}
