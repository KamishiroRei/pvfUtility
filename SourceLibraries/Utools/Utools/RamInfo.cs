namespace Utools;

public class RamInfo
{
	public double MemoryAvailable { get; set; }

	public double PhysicalMemory { get; set; }

	public double TotalPageFile { get; set; }

	public double AvailablePageFile { get; set; }

	public double TotalVirtual { get; set; }

	public double AvailableVirtual { get; set; }

	public double MemoryUsage => (1.0 - MemoryAvailable / PhysicalMemory) * 100.0;

	public RamInfo()
	{
	}
}
