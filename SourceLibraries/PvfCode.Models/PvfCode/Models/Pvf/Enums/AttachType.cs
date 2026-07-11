using System.ComponentModel.DataAnnotations;

namespace PvfCode.Models.Pvf.Enums;

public enum AttachType
{
	[Display(Name = "不限制")]
	free,
	[Display(Name = "封装")]
	sealing,
	[Display(Name = "不可交易")]
	trade,
	[Display(Name = "帐号绑定")]
	account,
	[Display(Name = "无法交易、删除")]
	trade_delete,
	[Display(Name = "封装且不可交易")]
	sealing_trade
}
