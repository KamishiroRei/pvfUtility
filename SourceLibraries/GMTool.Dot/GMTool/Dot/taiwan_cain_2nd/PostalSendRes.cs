using GMTool.SqlModel.Enums;
using GMTool.SqlModel.taiwan_cain_2nd;
using Newtonsoft.Json;
using SqlSugar;

namespace GMTool.Dot.taiwan_cain_2nd;

[JsonObject(MemberSerialization.OptOut)]
[SugarTable("taiwan_cain_2nd.postal")]
public class PostalSendRes : Postal
{
	[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
	public int postal_id { get; set; }

	[SugarColumn(IsIgnore = true)]
	public bool Best
	{
		get
		{
			return GetProperty(() => Best);
		}
		set
		{
			SetProperty(() => Best, value);
		}
	}

	[SugarColumn(IsIgnore = true)]
	public PostalType PostalType
	{
		get
		{
			return GetProperty(() => PostalType);
		}
		set
		{
			SetProperty(() => PostalType, value);
		}
	}

	[SugarColumn(IsIgnore = true)]
	public string SendText
	{
		get
		{
			return GetProperty(() => SendText);
		}
		set
		{
			SetProperty<string>(() => SendText, value);
		}
	}

	[SugarColumn(IsIgnore = true)]
	public string SendTite
	{
		get
		{
			return GetProperty(() => SendTite);
		}
		set
		{
			SetProperty<string>(() => SendTite, value);
		}
	}

	[SugarColumn(IsIgnore = true)]
	public bool? IsEqu { get; set; }

	[SugarColumn(IsIgnore = true)]
	public string ItemName { get; set; }

	public PostalSendRes()
	{
		PostalType = PostalType.普通邮件;
		base.amplify_option = Amplify_optionStyle.无红字;
	}

	public void SetBest()
	{
		if (Best)
		{
			base.add_info = 1013038648;
		}
	}

	public void Set_avata_flag()
	{
		base.avata_flag = true;
	}

	public PostalSendRes Clone()
	{
		return new PostalSendRes
		{
			Best = Best,
			PostalType = PostalType,
			SendText = SendText,
			SendTite = SendTite,
			amplify_option = base.amplify_option,
			avata_flag = base.avata_flag,
			creature_flag = base.creature_flag,
			postal_id = postal_id,
			seal_flag = base.seal_flag,
			add_info = base.add_info,
			item_id = base.item_id,
			receive_charac_no = base.receive_charac_no,
			amplify_value = base.amplify_value,
			delete_flag = base.delete_flag,
			endurance = base.endurance,
			gold = base.gold,
			hidden_option = base.hidden_option,
			IsEqu = IsEqu,
			occ_time = base.occ_time,
			Parameter = base.Parameter,
			postal = base.postal,
			receive_time = base.receive_time,
			send_charac_name = base.send_charac_name,
			send_charac_no = base.send_charac_no,
			seperate_upgrade = base.seperate_upgrade,
			upgrade = base.upgrade,
			ItemName = ItemName
		};
	}

	public void Init()
	{
		switch (PostalType)
		{
		case PostalType.普通邮件:
			if (IsEqu.HasValue)
			{
				if (IsEqu.Value)
				{
					SetBest();
				}
			}
			else
			{
				SetBest();
			}
			break;
		case PostalType.时装邮件:
			base.avata_flag = true;
			break;
		case PostalType.宠物:
			base.creature_flag = true;
			if (IsEqu.HasValue)
			{
				if (IsEqu.Value)
				{
					SetBest();
				}
			}
			else
			{
				SetBest();
			}
			break;
		case PostalType.宠物蛋:
			base.seal_flag = true;
			base.creature_flag = true;
			break;
		}
	}

	public string GetSendLog(string characName)
	{
		return $"为：'{characName}' 发送：'{PostalType}' {ItemName}<{base.item_id}> 成功";
	}
}
