using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("charac_ridable_stat")]
public class charac_ridable_stat
{
	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	public byte[] cooltime { get; set; }
}
