namespace PvfCode.Services.PreviewPvfFileFolder;

public class avatar_select_ability_Skill : avatar_select_ability_Base
{
	public string JobTypeStr { get; set; }

	public string SkillName { get; set; }

	public int SkillId { get; set; }

	public int Level { get; set; }

	public override string? Text
	{
		get
		{
			string value = (string.IsNullOrEmpty(SkillName) ? SkillId.ToString() : SkillName);
			return $"[{value}]技能lv +{Level}";
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
