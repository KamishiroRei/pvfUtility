using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain_2nd;

[SugarTable("letter_del")]
public class letter_del
{
	[SugarColumn(IsPrimaryKey = true)]
	public DateTime sdate { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public int letter_id { get; set; }

	public int charac_no { get; set; }

	public int send_charac_no { get; set; }

	public string send_charac_name { get; set; }

	public string letter_text { get; set; }

	public DateTime reg_date { get; set; }

	public byte stat { get; set; }
}
