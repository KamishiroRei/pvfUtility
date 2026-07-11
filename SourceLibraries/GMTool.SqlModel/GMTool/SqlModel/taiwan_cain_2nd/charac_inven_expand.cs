using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain_2nd;

[SugarTable("charac_inven_expand")]
public class charac_inven_expand
{
	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	public byte[] cargo { get; set; }

	public int cargo_capacity { get; set; }

	public byte[] jewel { get; set; }

	public string current_equipslot { get; set; }

	public byte[] switch_equipslot { get; set; }

	public byte[] expand_equipslot { get; set; }

	public byte[] redeem_info { get; set; }
}
