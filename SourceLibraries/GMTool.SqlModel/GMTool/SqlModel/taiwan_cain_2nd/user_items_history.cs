using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain_2nd;

[SugarTable("user_items_history")]
public class user_items_history
{
	[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
	public int ui_id { get; set; }

	public int charac_no { get; set; }

	public int slot { get; set; }

	public int it_id { get; set; }

	public DateTime expire_date { get; set; }

	public byte? obtain_from { get; set; }

	public DateTime reg_date { get; set; }

	public string ipg_agency_no { get; set; }

	public byte ability_no { get; set; }

	public byte stat { get; set; }

	public int clear_avatar_id { get; set; }

	public byte[] jewel_socket { get; set; }

	public byte item_lock_key { get; set; }

	public string to_ipg_agency_no { get; set; }

	public DateTime m_time { get; set; }

	public short hidden_option { get; set; }

	public short emblem_endurance { get; set; }

	public int? color1 { get; set; }

	public int? color2 { get; set; }

	public int? trade_restrict { get; set; }
}
