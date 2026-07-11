using System.ComponentModel.DataAnnotations;
using SqlSugar;
using Utools;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("dnf_event_log")]
public class dnf_event_log
{
	private string _event_explain;

	[Display(Name = "活动说明")]
	[SugarColumn(IsIgnore = true)]
	public string event_explain
	{
		get
		{
			return StrConvert.Convert(_event_explain, StrConvert.ConvertType.ToSimplified);
		}
		set
		{
			_event_explain = value;
		}
	}

	[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
	public int log_id { get; set; }

	public int occ_time { get; set; }

	[Display(Name = "活动编号")]
	public byte event_type { get; set; }

	[Display(Name = "参数1")]
	public int parameter1 { get; set; }

	[Display(Name = "参数2")]
	public int parameter2 { get; set; }

	public byte server_id { get; set; }

	public byte? event_flag { get; set; }

	public int start_time { get; set; }

	public int end_time { get; set; }

	public int m_id { get; set; }

	public string expl { get; set; }

	public string etc { get; set; }
}
