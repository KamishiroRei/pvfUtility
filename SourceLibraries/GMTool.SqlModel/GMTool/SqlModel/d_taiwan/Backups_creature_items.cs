using System;
using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("Backups_creature_items")]
public class Backups_creature_items
{
	[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
	public int ui_id { get; set; }

	public int charac_no { get; set; }

	public byte slot { get; set; }

	public int it_id { get; set; }

	public DateTime reg_date { get; set; }

	public string name { get; set; }

	public int stomach { get; set; }

	public int exp { get; set; }

	public byte endurance { get; set; }

	public byte creature_type { get; set; }

	public byte no_charge { get; set; }

	public byte stat { get; set; }

	public byte item_lock_key { get; set; }

	public string ipg_agency_no { get; set; }

	public DateTime expire_date { get; set; }

	public DateTime delete_date { get; set; }

	public DateTime? DataTime { get; set; }

	public int? FromID { get; set; }
}
