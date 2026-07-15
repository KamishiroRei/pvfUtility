using PvfCode.Models.Pvf.Enums;

namespace PvfCode.Services.PreviewPvfFileFolder;

public class SkillDataUp
{
	public string JobDefaultTypeStr { get; set; }

	public JobType? JobDefaultType
	{
		get
		{
			if (!PvfFileHelper.JobDefaultStringConvertEnum(JobDefaultTypeStr, out var jobType))
			{
				return null;
			}
			return jobType;
		}
	}

	public int JobTypeIndex { get; set; }

	public string DungeonType { get; set; }

	public string SkillStyle { get; set; }

	public int SkillStyleValue { get; set; }

	public string SkillAddType { get; set; }

	public int SkillAddValue { get; set; }

	public string Text
	{
		get
		{
			JobType? jobDefaultType = JobDefaultType;
			if (jobDefaultType.HasValue)
			{
				jobDefaultType.Value.ToString();
			}
			return string.Empty;
		}
	}

	public SkillDataUp()
	{
	}
}
