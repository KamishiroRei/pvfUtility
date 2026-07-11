using System;
using DevExpress.Mvvm;
using GMTool.SqlModel.Enums;
using Newtonsoft.Json;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain_2nd;

[Serializable]
[JsonObject(MemberSerialization.OptOut)]
public class Postal : ViewModelBase
{
	[SugarColumn(InsertServerTime = true)]
	public DateTime occ_time { get; set; }

	public int send_charac_no { get; set; }

	[SugarColumn(IsNullable = true)]
	public string send_charac_name
	{
		get
		{
			return GetProperty(() => send_charac_name);
		}
		set
		{
			SetProperty<string>(() => send_charac_name, value);
		}
	}

	public int receive_charac_no { get; set; }

	public int item_id
	{
		get
		{
			return GetProperty(() => item_id);
		}
		set
		{
			SetProperty(() => item_id, value);
		}
	}

	public int add_info
	{
		get
		{
			return GetProperty(() => add_info);
		}
		set
		{
			SetProperty(() => add_info, value);
		}
	}

	public int endurance
	{
		get
		{
			return GetProperty(() => endurance);
		}
		set
		{
			SetProperty(() => endurance, value);
		}
	}

	public int upgrade
	{
		get
		{
			return GetProperty(() => upgrade);
		}
		set
		{
			SetProperty(() => upgrade, value);
		}
	}

	public Amplify_optionStyle amplify_option
	{
		get
		{
			return GetProperty(() => amplify_option);
		}
		set
		{
			SetProperty(() => amplify_option, value);
			if (amplify_option == Amplify_optionStyle.无红字)
			{
				amplify_value = 0;
			}
		}
	}

	public int amplify_value
	{
		get
		{
			return GetProperty(() => amplify_value);
		}
		set
		{
			SetProperty(() => amplify_value, value);
		}
	}

	public int gold
	{
		get
		{
			return GetProperty(() => gold);
		}
		set
		{
			SetProperty(() => gold, value);
		}
	}

	public DateTime receive_time { get; set; }

	public bool delete_flag { get; set; }

	public bool avata_flag
	{
		get
		{
			return GetProperty(() => avata_flag);
		}
		set
		{
			SetProperty(() => avata_flag, value);
		}
	}

	public bool unlimit_flag => true;

	public bool seal_flag
	{
		get
		{
			return GetProperty(() => seal_flag);
		}
		set
		{
			SetProperty(() => seal_flag, value);
		}
	}

	public bool creature_flag
	{
		get
		{
			return GetProperty(() => creature_flag);
		}
		set
		{
			SetProperty(() => creature_flag, value);
		}
	}

	public int postal { get; set; }

	public int letter_id { get; set; }

	public int seperate_upgrade
	{
		get
		{
			return GetProperty(() => seperate_upgrade);
		}
		set
		{
			SetProperty(() => seperate_upgrade, value);
		}
	}

	[SugarColumn(IsIgnore = true)]
	public Amplify_optionStyle hidden_option
	{
		get
		{
			return GetProperty(() => hidden_option);
		}
		set
		{
			SetProperty(() => hidden_option, value);
		}
	}
}
