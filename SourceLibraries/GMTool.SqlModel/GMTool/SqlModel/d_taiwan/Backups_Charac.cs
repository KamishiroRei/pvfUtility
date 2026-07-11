using System;
using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("Backups_Charac")]
public class Backups_Charac
{
	[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
	public int BackupsID { get; set; }

	public int? charac_no { get; set; }

	public string charac_name { get; set; }

	public byte[] inventory { get; set; }

	public byte[] equipslot { get; set; }

	public byte[] creature { get; set; }

	public int? money { get; set; }

	public DateTime? DataTime { get; set; }
}
