using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("charac_expert_job")]
public class charac_expert_job
{
	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	public byte expert_job_giveup_cnt { get; set; }

	public int expert_job_info { get; set; }

	public int expert_job_info_ex { get; set; }

	public byte[] recipe { get; set; }
}
