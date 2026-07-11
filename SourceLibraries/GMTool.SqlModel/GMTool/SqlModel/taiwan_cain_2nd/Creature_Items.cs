using System;
using GMTool.SqlModel.Enums;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain_2nd;

[SugarTable("taiwan_cain_2nd.creature_items")]
public class Creature_Items
{
	[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
	public int ui_id { get; set; }

	public int charac_no { get; set; }

	public byte slot { get; set; }

	public int it_id { get; set; }

	[SugarColumn(InsertServerTime = true)]
	public DateTime reg_date { get; set; }

	public int stomach { get; set; }

	public int exp { get; set; }

	public byte endurance { get; set; }

	public Creature_ItemsStyle creature_type { get; set; }

	public byte no_charge { get; set; }

	public bool stat { get; set; }

	public byte item_lock_key { get; set; }

	public DateTime expire_date { get; set; }

	public DateTime delete_date { get; set; }
}
