using System;
using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("Backups_charac_inven_expand")]
public class Backups_charac_inven_expand
{
	public int charac_no { get; set; }

	public byte[] cargo { get; set; }

	public int cargo_capacity { get; set; }

	public byte[] jewel { get; set; }

	public string current_equipslot { get; set; }

	public byte[] switch_equipslot { get; set; }

	public byte[] expand_equipslot { get; set; }

	public byte[] redeem_info { get; set; }

	[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
	public int ID { get; set; }

	public DateTime? DataTime { get; set; }

	public int? FromID { get; set; }
}
