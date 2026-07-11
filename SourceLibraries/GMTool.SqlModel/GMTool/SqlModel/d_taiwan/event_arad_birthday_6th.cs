using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("event_arad_birthday_6th")]
public class event_arad_birthday_6th
{
	[SugarColumn(IsPrimaryKey = true)]
	public int server { get; set; }

	public int point { get; set; }
}
