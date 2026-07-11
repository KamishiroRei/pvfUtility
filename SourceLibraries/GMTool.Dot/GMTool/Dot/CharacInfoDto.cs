using System;
using Utools;

namespace GMTool.Dot;

public class CharacInfoDto : ICloneable
{
	public bool CharacOnLine { get; set; }

	public int UID { get; set; }

	public int charac_no { get; set; }

	public string CharName { get; set; }

	public string charac_name
	{
		get
		{
			return CodingHelper.Latin1ToGbkNew(CharName);
		}
		set
		{
			CharName = value;
		}
	}

	public int lev { get; set; }

	public int job { get; set; }

	public int grow_type { get; set; }

	public bool delete_flag { get; set; }

	public string JobTypeStr
	{
		get
		{
			if (delete_flag)
			{
				return "角色已删除";
			}
			return DnfHelper.GetCharGrowType(job, grow_type);
		}
	}

	public string accountname { get; set; }

	public bool UserOnLine { get; set; }

	public DateTime last_play_time { get; set; }

	public object Clone()
	{
		return MemberwiseClone();
	}
}
