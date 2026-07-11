using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("taiwan_cain.account_cargo")]
public class account_cargo
{
	[SugarColumn(IsPrimaryKey = true)]
	public long m_id { get; set; }

	public int money { get; set; }

	public byte capacity { get; set; }

	public byte[] cargo { get; set; }

	public DateTime occ_time { get; set; }
}
