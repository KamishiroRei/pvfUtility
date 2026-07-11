using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("limited_shop_manager")]
public class limited_shop_manager
{
	[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
	public int no { get; set; }

	public int occ_time { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public byte server_id { get; set; }

	[SugarColumn(IsPrimaryKey = true)]
	public int ipg_no { get; set; }

	public int item_no { get; set; }

	public int item_cnt { get; set; }

	public int cera_price { get; set; }

	public int gold_price { get; set; }

	public byte avatar_period_type { get; set; }

	public int total_cnt { get; set; }

	public int sell_cnt { get; set; }

	public int restrict_no { get; set; }

	public int start_time { get; set; }

	public int end_time { get; set; }

	public int real_end_time { get; set; }

	public int npc_idx { get; set; }

	public byte cond_charac_job { get; set; }

	public byte cond_lev_begin { get; set; }

	public byte cond_lev_end { get; set; }

	public int cond_acc_create_time_begin { get; set; }

	public int cond_acc_create_time_end { get; set; }

	public int cond_cha_create_time_begin { get; set; }

	public int cond_cha_create_time_end { get; set; }

	public byte status_flag { get; set; }

	public string title { get; set; }

	public byte range_section { get; set; }

	public string reason_etc { get; set; }

	public string reason_stop { get; set; }

	public string pos_flag { get; set; }
}
