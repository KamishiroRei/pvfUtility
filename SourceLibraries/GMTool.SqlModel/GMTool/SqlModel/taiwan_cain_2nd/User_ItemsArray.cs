using System;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain_2nd;

[SugarTable("taiwan_cain_2nd.user_items")]
public class User_ItemsArray
{
	[SugarColumn(IsPrimaryKey = true, IsIdentity = false)]
	public int ui_id { get; set; }

	public int charac_no { get; set; }

	public int slot { get; set; }

	public int it_id { get; set; }

	public DateTime expire_date { get; set; }

	[SugarColumn(IsNullable = true)]
	public int obtain_from { get; set; }

	public DateTime reg_date { get; set; }

	public byte ability_no { get; set; }

	public int stat { get; set; }

	public int clear_avatar_id { get; set; }

	public DateTime m_time { get; set; }

	public short hidden_option { get; set; }

	public byte item_lock_key { get; set; }
}
