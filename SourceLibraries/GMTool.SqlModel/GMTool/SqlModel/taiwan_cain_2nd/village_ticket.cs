using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain_2nd;

[SugarTable("village_ticket")]
public class village_ticket
{
	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public short village { get; set; }
}
