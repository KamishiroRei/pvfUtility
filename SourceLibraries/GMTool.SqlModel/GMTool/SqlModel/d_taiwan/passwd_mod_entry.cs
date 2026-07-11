using System;
using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("passwd_mod_entry")]
public class passwd_mod_entry
{
	[SugarColumn(IsPrimaryKey = true)]
	public int m_id { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public DateTime occ_time { get; set; }

	public string ip { get; set; }

	public string pre_passwd { get; set; }
}
