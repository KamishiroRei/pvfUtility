using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain_2nd;

[SugarTable("skill_fair_pvp")]
public class skill_fair_pvp
{
	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	public int remain_sp { get; set; }

	public byte[] skill_slot { get; set; }

	public int sp_garbage { get; set; }

	public int used_sp { get; set; }

	public byte[] skill_slot_lethe { get; set; }

	public byte lethe_flag { get; set; }

	public int remain_sp_2nd { get; set; }

	public byte[] skill_slot_2nd { get; set; }

	public byte[] skill_slot_lethe_2nd { get; set; }

	public byte lethe_flag_2nd { get; set; }

	public short remain_sfp_1st { get; set; }

	public short remain_sfp_2nd { get; set; }

	public byte[] skill_command { get; set; }

	public byte script_version { get; set; }
}
