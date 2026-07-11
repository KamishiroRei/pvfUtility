using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain_2nd;

[SugarTable("charac_advance_altar_item_desc")]
public class charac_advance_altar_item_desc
{
	[SugarColumn(IsPrimaryKey = true)]
	public int ridable_id { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public short item_type { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public int item_id { get; set; }

	public byte[] item_desc { get; set; }
}
