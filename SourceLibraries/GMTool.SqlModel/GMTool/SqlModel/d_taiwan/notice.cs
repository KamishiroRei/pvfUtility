using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("notice")]
public class notice
{
	public string bbs_name { get; set; }

	[SugarColumn(IsIdentity = true)]
	public int no { get; set; }

	public byte category { get; set; }

	public string m_nickname { get; set; }

	public int m_id { get; set; }

	public string m_user_id { get; set; }

	public string m_sex { get; set; }

	public string title { get; set; }

	public int create_day { get; set; }

	public short comment { get; set; }

	public int view { get; set; }

	public int recom { get; set; }

	public byte adorn { get; set; }

	public byte adorn_color1 { get; set; }

	public byte adorn_color2 { get; set; }

	public byte depth { get; set; }

	public object sequence { get; set; }

	public string content { get; set; }

	public string content_type { get; set; }

	public string ip { get; set; }

	public short ring { get; set; }

	public string sms { get; set; }
}
