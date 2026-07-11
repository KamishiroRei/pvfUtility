using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("charac_option")]
public class charac_option
{
	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	public byte[] options { get; set; }

	public byte[] best_clear_time { get; set; }

	public byte blue_marble_enter_count { get; set; }

	public string charac_inform_notice { get; set; }
}
