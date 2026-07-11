using System;
using System.ComponentModel.DataAnnotations;
using GMTool.SqlModel.Enums;
using SqlSugar;
using Utools;

namespace GMTool.SqlModel.taiwan_cain_auction_gold;

[Serializable]
public class auction_main
{
	private int _add_info;

	[Display(Name = "上架时间")]
	[SugarColumn(IsIgnore = true)]
	public string OccTimeStr => occ_time.ToString("yyyy-MM-dd HH:mm:ss");

	[SugarColumn(IsNullable = true)]
	public DateTime occ_time { get; set; }

	public int expire_time { get; set; }

	public int owner_id { get; set; }

	private string _owner_name { get; set; }

	[Display(Name = "上架人")]
	[SugarColumn(IsNullable = true)]
	public string owner_name
	{
		get
		{
			return StrConvert.Convert(CodingHelper.Latin1ToGbkNew(_owner_name), StrConvert.ConvertType.ToSimplified);
		}
		set
		{
			_owner_name = value;
		}
	}

	[SugarColumn(IsNullable = true)]
	public string owner_nexon_id { get; set; }

	public int buyer_id { get; set; }

	[SugarColumn(IsNullable = true)]
	public string buyer_name { get; set; }

	public long price { get; set; }

	[Display(Name = "总价")]
	public long instant_price { get; set; }

	public bool seal_flag { get; set; }

	[Display(Name = "物品代码")]
	public int item_id { get; set; }

	[Display(Name = "数量")]
	public int add_info
	{
		get
		{
			return _add_info;
		}
		set
		{
			_add_info = value;
		}
	}

	[Display(Name = "强化")]
	public int upgrade { get; set; }

	[Display(Name = "红字类型")]
	public Amplify_optionStyle amplify_option { get; set; }

	[Display(Name = "红字数值")]
	public int amplify_value { get; set; }

	public int seal_cnt { get; set; }

	[Display(Name = "耐久")]
	public int endurance { get; set; }

	public int extend_info { get; set; }

	[Display(Name = "单价")]
	public int unit_price { get; set; }

	[Display(Name = "锻造等级")]
	public int seperate_upgrade { get; set; }
}
