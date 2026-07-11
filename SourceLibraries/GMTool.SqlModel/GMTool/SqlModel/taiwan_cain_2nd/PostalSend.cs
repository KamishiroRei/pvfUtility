using System;
using System.Threading.Tasks;
using GMTool.SqlModel.Enums;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain_2nd;

[Serializable]
[SugarTable("taiwan_cain_2nd.postal")]
public class PostalSend : Postal, ICloneable
{
	[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
	public int postal_id { get; set; }

	[SugarColumn(IsIgnore = true)]
	public bool Best { get; set; }

	[SugarColumn(IsIgnore = true)]
	public PostalType PostalType { get; set; }

	public PostalSend()
	{
		PostalType = PostalType.普通邮件;
		base.amplify_option = Amplify_optionStyle.无红字;
	}

	internal async Task<bool> SendPostal(SqlSugarClient db, bool isEcho = true, bool isSetName = true)
	{
		return true;
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

	public object Clone()
	{
		return MemberwiseClone();
	}
}
