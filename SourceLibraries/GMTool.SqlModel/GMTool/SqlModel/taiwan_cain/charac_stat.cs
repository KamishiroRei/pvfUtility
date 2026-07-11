using System;
using SqlSugar;
using Utools;

namespace GMTool.SqlModel.taiwan_cain;

[SugarTable("taiwan_cain.charac_stat")]
public class charac_stat
{
	private string _characName;

	[SugarColumn(IsPrimaryKey = true)]
	public int charac_no { get; set; }

	public int village { get; set; }

	public int fatigue { get; set; }

	public int used_fatigue { get; set; }

	public int premium_fatigue { get; set; }

	public long trade_gold_total { get; set; }

	public DateTime last_play_time { get; set; }

	[SugarColumn(IsIgnore = true)]
	public string CharacName
	{
		get
		{
			return CodingHelper.Latin1ToGbkNew(_characName);
		}
		set
		{
			_characName = value;
		}
	}

	public void AddPremium(int addNumber)
	{
		int num = premium_fatigue - addNumber;
		if (num < 0)
		{
			num = 0;
		}
		premium_fatigue = num;
	}
}
