using System.Runtime.CompilerServices;

namespace PvfCode.Services.PreviewPvfFileFolder;

public class avatar_select_ability_Skill : avatar_select_ability_Base
{
	[CompilerGenerated]
	private string Pja8Xejtqq;

	[CompilerGenerated]
	private string jwO8FvtC5t;

	[CompilerGenerated]
	private int eX88DakgGj;

	[CompilerGenerated]
	private int ESA8mS4q4d;

	public string JobTypeStr
	{
		[CompilerGenerated]
		get
		{
			return Pja8Xejtqq;
		}
		[CompilerGenerated]
		set
		{
			Pja8Xejtqq = value;
		}
	}

	public string SkillName
	{
		[CompilerGenerated]
		get
		{
			return jwO8FvtC5t;
		}
		[CompilerGenerated]
		set
		{
			jwO8FvtC5t = value;
		}
	}

	public int SkillId
	{
		[CompilerGenerated]
		get
		{
			return eX88DakgGj;
		}
		[CompilerGenerated]
		set
		{
			eX88DakgGj = value;
		}
	}

	public int Level
	{
		[CompilerGenerated]
		get
		{
			return ESA8mS4q4d;
		}
		[CompilerGenerated]
		set
		{
			ESA8mS4q4d = value;
		}
	}

	public override string? Text
	{
		get
		{
			string value = (string.IsNullOrEmpty(SkillName) ? SkillId.ToString() : SkillName);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(8, 2);
			defaultInterpolatedStringHandler.AppendLiteral("[");
			defaultInterpolatedStringHandler.AppendFormatted(value);
			defaultInterpolatedStringHandler.AppendLiteral("]技能lv +");
			defaultInterpolatedStringHandler.AppendFormatted(Level);
			return defaultInterpolatedStringHandler.ToStringAndClear();
		}
	}

	public avatar_select_ability_Skill(string command, string jobTypeStr, int skillId, int level)
	{
		base.Command = command;
		JobTypeStr = jobTypeStr;
		SkillId = skillId;
		Level = level;
	}
}
