using System.Runtime.CompilerServices;
using PvfCode.Models.Pvf.Enums;

namespace PvfCode.Services.PreviewPvfFileFolder;

public class SkillDataUp
{
	[CompilerGenerated]
	private string hcs8hKVjfd;

	[CompilerGenerated]
	private int pfq8dF99DJ;

	[CompilerGenerated]
	private string zu489Xj8To;

	[CompilerGenerated]
	private string dVi8qXcbLC;

	[CompilerGenerated]
	private int fWD8g845yt;

	[CompilerGenerated]
	private string RyQ8zjYfRN;

	[CompilerGenerated]
	private int Q10juEOLJT;

	public string JobDefaultTypeStr
	{
		[CompilerGenerated]
		get
		{
			return hcs8hKVjfd;
		}
		[CompilerGenerated]
		set
		{
			hcs8hKVjfd = value;
		}
	}

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

	public int JobTypeIndex
	{
		[CompilerGenerated]
		get
		{
			return pfq8dF99DJ;
		}
		[CompilerGenerated]
		set
		{
			pfq8dF99DJ = value;
		}
	}

	public string DungeonType
	{
		[CompilerGenerated]
		get
		{
			return zu489Xj8To;
		}
		[CompilerGenerated]
		set
		{
			zu489Xj8To = value;
		}
	}

	public string SkillStyle
	{
		[CompilerGenerated]
		get
		{
			return dVi8qXcbLC;
		}
		[CompilerGenerated]
		set
		{
			dVi8qXcbLC = value;
		}
	}

	public int SkillStyleValue
	{
		[CompilerGenerated]
		get
		{
			return fWD8g845yt;
		}
		[CompilerGenerated]
		set
		{
			fWD8g845yt = value;
		}
	}

	public string SkillAddType
	{
		[CompilerGenerated]
		get
		{
			return RyQ8zjYfRN;
		}
		[CompilerGenerated]
		set
		{
			RyQ8zjYfRN = value;
		}
	}

	public int SkillAddValue
	{
		[CompilerGenerated]
		get
		{
			return Q10juEOLJT;
		}
		[CompilerGenerated]
		set
		{
			Q10juEOLJT = value;
		}
	}

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
