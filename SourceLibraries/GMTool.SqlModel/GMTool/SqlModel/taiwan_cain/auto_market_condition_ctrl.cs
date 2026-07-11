using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("auto_market_condition_ctrl")]
public class auto_market_condition_ctrl
{
	public long optimum_gold_supply { get; set; }

	public long over_gold { get; set; }
}
