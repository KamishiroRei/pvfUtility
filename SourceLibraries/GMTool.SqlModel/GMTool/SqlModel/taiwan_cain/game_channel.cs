using System;
using System.ComponentModel.DataAnnotations;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("taiwan_cain.game_channel")]
public class game_channel
{
	[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
	public int gc_no { get; set; }

	public short gc_now { get; set; }

	[Display(Name = "服务器IP")]
	public string gc_ip { get; set; }

	[Display(Name = "端口")]
	public short gc_port { get; set; }

	public short gc_max { get; set; }

	public byte gc_game { get; set; }

	[Display(Name = "频道名称")]
	public string gc_channel { get; set; }

	[Display(Name = "频道ID")]
	public short gc_ch_group { get; set; }

	[Display(Name = "更新时间")]
	public string gc_channeltype { get; set; }

	public DateTime gc_up_time { get; set; }

	[Display(Name = "鬼剑士在线人数")]
	public short gc_swordman_cnt { get; set; }

	[Display(Name = "格斗家在线人数")]
	public short gc_fighter_cnt { get; set; }

	[Display(Name = "男神枪手在线人数")]
	public short gc_gunner_cnt { get; set; }

	[Display(Name = "魔法师女在线人数")]
	public short gc_mage_cnt { get; set; }

	[Display(Name = "圣职者在线人数")]
	public short gc_priest_cnt { get; set; }

	[Display(Name = "神枪手女在线人数")]
	public short gc_at_gunner_cnt { get; set; }

	[Display(Name = "暗夜使者在线人数")]
	public short gc_thief_cnt { get; set; }

	[Display(Name = "当前频道在线总数")]
	public short gc_hangame { get; set; }

	public short gc_nexon { get; set; }

	public byte gc_type { get; set; }
}
