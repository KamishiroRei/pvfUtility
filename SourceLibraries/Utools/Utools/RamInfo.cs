using System.Runtime.CompilerServices;

namespace Utools;

public class RamInfo
{
	[CompilerGenerated]
	private double SlCVJhwQww;

	[CompilerGenerated]
	private double yWiV0qe6Si;

	[CompilerGenerated]
	private double tRgVzrPRk5;

	[CompilerGenerated]
	private double V0g9YR1QsR;

	[CompilerGenerated]
	private double SuJ9VCUp4K;

	[CompilerGenerated]
	private double RgH99UGDnU;

	public double MemoryAvailable
	{
		[CompilerGenerated]
		get
		{
			return SlCVJhwQww;
		}
		[CompilerGenerated]
		set
		{
			SlCVJhwQww = value;
		}
	}

	public double PhysicalMemory
	{
		[CompilerGenerated]
		get
		{
			return yWiV0qe6Si;
		}
		[CompilerGenerated]
		set
		{
			yWiV0qe6Si = value;
		}
	}

	public double TotalPageFile
	{
		[CompilerGenerated]
		get
		{
			return tRgVzrPRk5;
		}
		[CompilerGenerated]
		set
		{
			tRgVzrPRk5 = value;
		}
	}

	public double AvailablePageFile
	{
		[CompilerGenerated]
		get
		{
			return V0g9YR1QsR;
		}
		[CompilerGenerated]
		set
		{
			V0g9YR1QsR = value;
		}
	}

	public double TotalVirtual
	{
		[CompilerGenerated]
		get
		{
			return SuJ9VCUp4K;
		}
		[CompilerGenerated]
		set
		{
			SuJ9VCUp4K = value;
		}
	}

	public double AvailableVirtual
	{
		[CompilerGenerated]
		get
		{
			return RgH99UGDnU;
		}
		[CompilerGenerated]
		set
		{
			RgH99UGDnU = value;
		}
	}

	public double MemoryUsage => (1.0 - MemoryAvailable / PhysicalMemory) * 100.0;

	public RamInfo()
	{
	}
}
