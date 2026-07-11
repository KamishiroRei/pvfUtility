using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Utools.系统;

namespace Utools;

public static class SystemInfo
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass48_0
	{
		public AddressFamily aigMrmiWCy;

		public _003C_003Ec__DisplayClass48_0()
		{
		}

		internal bool I30MQIhUqt(IPAddress p)
		{
			if (!p.IsIPv6Teredo && !p.IsIPv6LinkLocal && !p.IsIPv6Multicast && !p.IsIPv6SiteLocal)
			{
				return p.AddressFamily == aigMrmiWCy;
			}
			return false;
		}
	}

	private static readonly PerformanceCounter cYy9ATsB5L;

	private static readonly PerformanceCounter Tn29GC4Fde;

	private static readonly PerformanceCounter Hjd9FjwdRD;

	private static readonly PerformanceCounter lak9bpf9jA;

	private static readonly PerformanceCounter vyw9yhu0FV;

	private static readonly string[] E849PCbueB;

	private static readonly PerformanceCounter[] fDF9tBiPdF;

	private static readonly PerformanceCounter[] k1k9pFREp5;

	[CompilerGenerated]
	private static bool jdw9QpUBkV;

	[CompilerGenerated]
	private static readonly int hfk9rOvYLu;

	[CompilerGenerated]
	private static readonly long lvg9qea220;

	private static readonly Lazy<List<ManagementBaseObject>> SsT9jTE9OE;

	private static readonly List<DiskInfo> K7v9ZcPfXg;

	public static int ProcessorCount
	{
		[CompilerGenerated]
		get
		{
			return hfk9rOvYLu;
		}
	}

	public static float CpuLoad => cYy9ATsB5L.NextValue();

	public static long MemoryAvailable
	{
		get
		{
			try
			{
				using ManagementClass managementClass = new ManagementClass("Win32_OperatingSystem");
				using ManagementObjectCollection managementObjectCollection = managementClass.GetInstances();
				foreach (ManagementBaseObject item in managementObjectCollection)
				{
					using (item)
					{
						if (item["FreePhysicalMemory"] != null)
						{
							return 1024 * long.Parse(item["FreePhysicalMemory"].ToString());
						}
					}
				}
				return 0L;
			}
			catch (Exception)
			{
				return 0L;
			}
		}
	}

	public static long PhysicalMemory
	{
		[CompilerGenerated]
		get
		{
			return lvg9qea220;
		}
	}

	static SystemInfo()
	{
		Tn29GC4Fde = new PerformanceCounter();
		Hjd9FjwdRD = new PerformanceCounter();
		lak9bpf9jA = new PerformanceCounter();
		vyw9yhu0FV = new PerformanceCounter();
		SsT9jTE9OE = new Lazy<List<ManagementBaseObject>>(delegate
		{
			using ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
			using ManagementObjectCollection source = managementObjectSearcher.Get();
			return source.AsParallel().Cast<ManagementBaseObject>().ToList();
		});
		K7v9ZcPfXg = new List<DiskInfo>();
		cYy9ATsB5L = new PerformanceCounter("Processor", "% Processor Time", "_Total")
		{
			MachineName = "."
		};
		cYy9ATsB5L.NextValue();
		hfk9rOvYLu = Environment.ProcessorCount;
		try
		{
			using ManagementClass managementClass = new ManagementClass("Win32_ComputerSystem");
			using ManagementObjectCollection managementObjectCollection = managementClass.GetInstances();
			foreach (ManagementBaseObject item in managementObjectCollection)
			{
				using (item)
				{
					if (item["TotalPhysicalMemory"] != null)
					{
						lvg9qea220 = long.Parse(item["TotalPhysicalMemory"].ToString());
					}
				}
			}
			E849PCbueB = new PerformanceCounterCategory("Network Interface").GetInstanceNames();
			fDF9tBiPdF = new PerformanceCounter[E849PCbueB.Length];
			k1k9pFREp5 = new PerformanceCounter[E849PCbueB.Length];
			for (int num = 0; num < E849PCbueB.Length; num++)
			{
				fDF9tBiPdF[num] = new PerformanceCounter();
				k1k9pFREp5[num] = new PerformanceCounter();
			}
			O1n9KH5PRp(false);
		}
		catch (Exception)
		{
		}
	}

	[SpecialName]
	[CompilerGenerated]
	private static bool EfC9MfaLCo()
	{
		return jdw9QpUBkV;
	}

	[SpecialName]
	[CompilerGenerated]
	private static void O1n9KH5PRp(bool P_0)
	{
		jdw9QpUBkV = P_0;
	}

	public static async Task<double> GetCpuUsageForProcess()
	{
		DateTime startTime = DateTime.UtcNow;
		using Process p1 = Process.GetCurrentProcess();
		TimeSpan startCpuUsage = p1.TotalProcessorTime;
		await Task.Delay(500);
		DateTime utcNow = DateTime.UtcNow;
		using Process process = Process.GetCurrentProcess();
		double totalMilliseconds = (process.TotalProcessorTime - startCpuUsage).TotalMilliseconds;
		double totalMilliseconds2 = (utcNow - startTime).TotalMilliseconds;
		return totalMilliseconds / ((double)Environment.ProcessorCount * totalMilliseconds2) * 100.0;
	}

	public static ArrayList FindAllApps(int handle)
	{
		ArrayList arrayList = new ArrayList();
		for (int window = GetWindow(handle, 0); window > 0; window = GetWindow(window, 2))
		{
			int num = 276824064;
			if ((GetWindowLongA(window, -16) & num) == num)
			{
				int windowTextLength = GetWindowTextLength(new IntPtr(window));
				StringBuilder stringBuilder = new StringBuilder(2 * windowTextLength + 1);
				GetWindowText(window, stringBuilder, stringBuilder.Capacity);
				string value = stringBuilder.ToString();
				if (!string.IsNullOrEmpty(value))
				{
					arrayList.Add(value);
				}
			}
		}
		return arrayList;
	}

	public static int GetCpuCount()
	{
		try
		{
			using ManagementClass managementClass = new ManagementClass("Win32_Processor");
			using ManagementObjectCollection managementObjectCollection = managementClass.GetInstances();
			return managementObjectCollection.Count;
		}
		catch (Exception)
		{
			return 0;
		}
	}

	public static List<CpuInfo> GetCpuInfo()
	{
		try
		{
			return SsT9jTE9OE.Value.Select((ManagementBaseObject mo) => new CpuInfo
			{
				CpuLoad = CpuLoad,
				NumberOfLogicalProcessors = ProcessorCount,
				CurrentClockSpeed = mo.Properties["CurrentClockSpeed"].Value.ToString(),
				Manufacturer = mo.Properties["Manufacturer"].Value.ToString(),
				MaxClockSpeed = mo.Properties["MaxClockSpeed"].Value.ToString(),
				Type = mo.Properties["Name"].Value.ToString(),
				DataWidth = mo.Properties["DataWidth"].Value.ToString(),
				SerialNumber = mo.Properties["ProcessorId"].Value.ToString(),
				DeviceID = mo.Properties["DeviceID"].Value.ToString(),
				NumberOfCores = Convert.ToInt32(mo.Properties["NumberOfCores"].Value),
				Temperature = GetCPUTemperature()
			}).ToList();
		}
		catch (Exception)
		{
			return new List<CpuInfo>();
		}
	}

	public static RamInfo GetRamInfo()
	{
		return new RamInfo
		{
			MemoryAvailable = GetFreePhysicalMemory(),
			PhysicalMemory = GetTotalPhysicalMemory(),
			TotalPageFile = GetTotalVirtualMemory(),
			AvailablePageFile = GetTotalVirtualMemory() - GetUsedVirtualMemory(),
			AvailableVirtual = 1f - GetUsageVirtualMemory(),
			TotalVirtual = 1f - GetUsedPhysicalMemory()
		};
	}

	public static float GetCPUTemperature()
	{
		try
		{
			using ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("root\\WMI", "select * from MSAcpi_ThermalZoneTemperature");
			using ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();
			using ManagementObjectCollection.ManagementObjectEnumerator managementObjectEnumerator = managementObjectCollection.GetEnumerator();
			if (managementObjectEnumerator.MoveNext())
			{
				ManagementBaseObject current = managementObjectEnumerator.Current;
				using (current)
				{
					return (float)Math.Round((float.Parse(current.Properties["CurrentTemperature"].Value.ToString()) - 2732f) / 10f, 2);
				}
			}
		}
		catch (Exception)
		{
			return 0f;
		}
		return 0f;
	}

	public static string GetProcessorData()
	{
		float num = P0s97N2mVh(Hjd9FjwdRD, "Processor", "% Processor Time", "_Total");
		if (!EfC9MfaLCo())
		{
			return num.ToString("F") + "%";
		}
		return (int)num + "%";
	}

	public static string GetMemoryVData()
	{
		string text = P0s97N2mVh(Tn29GC4Fde, "Memory", "% Committed Bytes In Use", null).ToString("F") + "% (";
		float num = P0s97N2mVh(Tn29GC4Fde, "Memory", "Committed Bytes", null);
		string text2 = text + FormatBytes(num) + " / ";
		num = P0s97N2mVh(Tn29GC4Fde, "Memory", "Commit Limit", null);
		return text2 + FormatBytes(num) + ") ";
	}

	public static float GetUsageVirtualMemory()
	{
		return P0s97N2mVh(Tn29GC4Fde, "Memory", "% Committed Bytes In Use", null);
	}

	public static float GetUsedVirtualMemory()
	{
		return P0s97N2mVh(Tn29GC4Fde, "Memory", "Committed Bytes", null);
	}

	public static float GetTotalVirtualMemory()
	{
		return P0s97N2mVh(Tn29GC4Fde, "Memory", "Commit Limit", null);
	}

	public static string GetMemoryPData()
	{
		string value = QueryComputerSystem("totalphysicalmemory");
		float num = Convert.ToSingle(value);
		float num2 = P0s97N2mVh(Tn29GC4Fde, "Memory", "Available Bytes", null);
		num2 = num - num2;
		value = (EfC9MfaLCo() ? "%" : ("% (" + FormatBytes(num2) + " / " + FormatBytes(num) + ")"));
		num2 /= num;
		num2 *= 100f;
		if (!EfC9MfaLCo())
		{
			return num2.ToString("F") + value;
		}
		return (int)num2 + value;
	}

	public static float GetTotalPhysicalMemory()
	{
		return QueryComputerSystem("totalphysicalmemory").TryConvertTo(0f);
	}

	public static float GetFreePhysicalMemory()
	{
		return P0s97N2mVh(Tn29GC4Fde, "Memory", "Available Bytes", null);
	}

	public static float GetUsedPhysicalMemory()
	{
		return GetTotalPhysicalMemory() - GetFreePhysicalMemory();
	}

	public static float GetDiskData(DiskData dd)
	{
		return dd switch
		{
			DiskData.ReadAndWrite => P0s97N2mVh(lak9bpf9jA, "PhysicalDisk", "Disk Read Bytes/sec", "_Total") + P0s97N2mVh(vyw9yhu0FV, "PhysicalDisk", "Disk Write Bytes/sec", "_Total"), 
			DiskData.Write => P0s97N2mVh(vyw9yhu0FV, "PhysicalDisk", "Disk Write Bytes/sec", "_Total"), 
			DiskData.Read => P0s97N2mVh(lak9bpf9jA, "PhysicalDisk", "Disk Read Bytes/sec", "_Total"), 
			_ => 0f, 
		};
	}

	public static float GetNetData(NetData nd)
	{
		if (E849PCbueB.Length == 0)
		{
			return 0f;
		}
		float num = 0f;
		for (int i = 0; i < E849PCbueB.Length; i++)
		{
			float num2 = P0s97N2mVh(fDF9tBiPdF[i], "Network Interface", "Bytes Received/sec", E849PCbueB[i]);
			float num3 = P0s97N2mVh(k1k9pFREp5[i], "Network Interface", "Bytes Sent/sec", E849PCbueB[i]);
			num = nd switch
			{
				NetData.Received => num + num2, 
				NetData.Sent => num + num3, 
				NetData.ReceivedAndSent => num + (num2 + num3), 
				_ => num + 0f, 
			};
		}
		return num;
	}

	public static IList<string> GetMacAddress()
	{
		try
		{
			IList<string> list = new List<string>();
			using ManagementClass managementClass = new ManagementClass("Win32_NetworkAdapterConfiguration");
			using ManagementObjectCollection managementObjectCollection = managementClass.GetInstances();
			foreach (ManagementBaseObject item in managementObjectCollection)
			{
				using (item)
				{
					if ((bool)item["IPEnabled"])
					{
						list.Add(item["MacAddress"].ToString());
					}
				}
			}
			return list;
		}
		catch (Exception)
		{
			return new List<string>();
		}
	}

	public static IPAddress GetLocalUsedIP()
	{
		return GetLocalUsedIP(AddressFamily.InterNetwork);
	}

	public static IPAddress GetLocalUsedIP(AddressFamily family)
	{
		_003C_003Ec__DisplayClass48_0 CS_0024_003C_003E8__locals2 = new _003C_003Ec__DisplayClass48_0();
		CS_0024_003C_003E8__locals2.aigMrmiWCy = family;
		return (from p in (from t in NetworkInterface.GetAllNetworkInterfaces()
				orderby t.Speed descending
				where t.NetworkInterfaceType != NetworkInterfaceType.Loopback && t.OperationalStatus == OperationalStatus.Up
				select t.GetIPProperties() into p
				where p.DhcpServerAddresses.Count > 0
				select p).SelectMany((IPInterfaceProperties p) => p.UnicastAddresses)
			select p.Address).FirstOrDefault((IPAddress p) => !p.IsIPv6Teredo && !p.IsIPv6LinkLocal && !p.IsIPv6Multicast && !p.IsIPv6SiteLocal && p.AddressFamily == CS_0024_003C_003E8__locals2.aigMrmiWCy);
	}

	public static List<UnicastIPAddressInformation> GetLocalIPs()
	{
		return (from c in NetworkInterface.GetAllNetworkInterfaces()
			orderby c.Speed descending
			where c.NetworkInterfaceType != NetworkInterfaceType.Loopback && c.OperationalStatus == OperationalStatus.Up
			select c).SelectMany((NetworkInterface n) => n.GetIPProperties().UnicastAddresses).ToList();
	}

	public static string FormatBytes(this double bytes)
	{
		int num = 0;
		while (bytes > 1024.0)
		{
			bytes /= 1024.0;
			num++;
		}
		string obj = (EfC9MfaLCo() ? ((int)bytes).ToString() : (bytes.ToString("F") + " "));
		DataSizeUnit unit = (DataSizeUnit)num;
		return obj + unit;
	}

	public static DateTime BootTime()
	{
		using ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(new SelectQuery("SELECT LastBootUpTime FROM Win32_OperatingSystem WHERE Primary='true'"));
		using ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();
		using (ManagementObjectCollection.ManagementObjectEnumerator managementObjectEnumerator = managementObjectCollection.GetEnumerator())
		{
			if (managementObjectEnumerator.MoveNext())
			{
				ManagementBaseObject current = managementObjectEnumerator.Current;
				using (current)
				{
					return ManagementDateTimeConverter.ToDateTime(current.Properties["LastBootUpTime"].Value.ToString());
				}
			}
		}
		return DateTime.Now - TimeSpan.FromMilliseconds(Environment.TickCount & 0x7FFFFFFF);
	}

	public static string QueryComputerSystem(string type)
	{
		try
		{
			using ManagementObjectCollection managementObjectCollection = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem").Get();
			using ManagementObjectCollection.ManagementObjectEnumerator managementObjectEnumerator = managementObjectCollection.GetEnumerator();
			if (managementObjectEnumerator.MoveNext())
			{
				ManagementBaseObject current = managementObjectEnumerator.Current;
				using (current)
				{
					return current[type].ToString();
				}
			}
		}
		catch (Exception ex)
		{
			return "未能获取到当前计算机系统信息，可能是当前程序无管理员权限，如果是web应用程序，请将应用程序池的高级设置中的进程模型下的标识设置为：LocalSystem；如果是普通桌面应用程序，请提升管理员权限后再操作。异常信息：" + ex.Message;
		}
		return string.Empty;
	}

	public static string QueryEnvironment(string type)
	{
		return Environment.ExpandEnvironmentVariables(type);
	}

	public static List<DiskInfo> GetDiskInfo()
	{
		try
		{
			if (K7v9ZcPfXg.Count > 0)
			{
				return K7v9ZcPfXg;
			}
			using ManagementClass managementClass = new ManagementClass("Win32_DiskDrive");
			using ManagementObjectCollection managementObjectCollection = managementClass.GetInstances();
			foreach (ManagementBaseObject item in managementObjectCollection)
			{
				using (item)
				{
					K7v9ZcPfXg.Add(new DiskInfo
					{
						Total = float.Parse(item["Size"].ToString()),
						Model = item["Model"].ToString(),
						SerialNumber = item["SerialNumber"].ToString()
					});
				}
			}
			return K7v9ZcPfXg;
		}
		catch (Exception)
		{
			return new List<DiskInfo>();
		}
	}

	private static float P0s97N2mVh(PerformanceCounter P_0, string P_1, string P_2, string P_3)
	{
		P_0.CategoryName = P_1;
		P_0.CounterName = P_2;
		P_0.InstanceName = P_3;
		return P_0.NextValue();
	}

	[DllImport("User32")]
	public static extern int GetWindow(int hWnd, int wCmd);

	[DllImport("User32")]
	public static extern int GetWindowLongA(int hWnd, int wIndx);

	[DllImport("user32.dll")]
	public static extern bool GetWindowText(int hWnd, StringBuilder title, int maxBufSize);

	[DllImport("User32", CharSet = CharSet.Auto)]
	public static extern int GetWindowTextLength(IntPtr hWnd);
}
