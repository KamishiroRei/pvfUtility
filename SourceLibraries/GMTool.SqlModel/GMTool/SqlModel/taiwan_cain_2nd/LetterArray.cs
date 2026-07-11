using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain_2nd;

[SugarTable("taiwan_cain_2nd.letter")]
public class LetterArray
{
	public int letter_id { get; set; }

	public int charac_no { get; set; }

	public int send_charac_no { get; set; }

	public string send_charac_name { get; set; }

	public string letter_text { get; set; }

	public DateTime reg_date => DateTime.Now;

	public int stat => 1;
}
