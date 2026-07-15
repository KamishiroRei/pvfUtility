namespace Utools;

public class CpuInfo
{
	public string DeviceID { get; set; }

	public string Type { get; set; }

	public string Manufacturer { get; set; }

	public string MaxClockSpeed { get; set; }

	public string CurrentClockSpeed { get; set; }

	public int NumberOfCores { get; set; }

	public int NumberOfLogicalProcessors { get; set; }

	public double CpuLoad { get; set; }

	public string DataWidth { get; set; }

	public double Temperature { get; set; }

	public string SerialNumber { get; set; }

	public CpuInfo()
	{
	}
}
