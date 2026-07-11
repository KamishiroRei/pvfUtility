#define TRACE
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Microsoft.Win32;

namespace Utools;

public static class WindowsEx
{
	private static object UEc9SjaZgO;

	[DllImport("psapi.dll", EntryPoint = "EmptyWorkingSet")]
	private static extern int ssI9oPrybU(IntPtr P_0);

	public static double ClearMemory()
	{
		ClearMemorySilent();
		Thread.Sleep(1000);
		return SystemInfo.GetRamInfo().MemoryUsage;
	}

	public static void ClearMemorySilent()
	{
		GC.Collect();
		GC.WaitForPendingFinalizers();
		Process[] processes = Process.GetProcesses();
		foreach (Process process in processes)
		{
			using (process)
			{
				if (!process.ProcessName.Equals("System") || !process.ProcessName.Equals("Idle"))
				{
					try
					{
						ssI9oPrybU(process.Handle);
					}
					catch
					{
					}
				}
			}
		}
		GC.Collect();
	}

	public static void ClearMemorySilent(string processName)
	{
		GC.Collect();
		GC.WaitForPendingFinalizers();
		Process[] processes = Process.GetProcesses();
		foreach (Process process in processes)
		{
			using (process)
			{
				if (process.ProcessName.Equals(processName))
				{
					try
					{
						ssI9oPrybU(process.Handle);
					}
					catch
					{
					}
				}
			}
		}
		GC.Collect();
	}

	public static void ClearMemorySilent(Process process)
	{
		lock (UEc9SjaZgO)
		{
			Process[] processes = Process.GetProcesses();
			foreach (Process process2 in processes)
			{
				using (process2)
				{
					if ((process2.ProcessName.Equals("System") && process2.ProcessName.Equals("Idle")) || !process2.ProcessName.ToLower().Contains("pvfutility"))
					{
						continue;
					}
					try
					{
						if (process2.Id == process.Id)
						{
							ssI9oPrybU(process2.Handle);
						}
					}
					catch
					{
					}
				}
			}
		}
	}

	public static string RunApp(string filename, string arguments, bool recordLog)
	{
		try
		{
			if (recordLog)
			{
				Trace.WriteLine(filename + " " + arguments);
			}
			using Process process = new Process
			{
				StartInfo = 
				{
					FileName = filename,
					CreateNoWindow = true,
					Arguments = arguments,
					RedirectStandardOutput = true,
					UseShellExecute = false
				}
			};
			process.Start();
			using StreamReader streamReader = new StreamReader(process.StandardOutput.BaseStream, Encoding.Default);
			Thread.Sleep(100);
			if (!process.HasExited)
			{
				process.Kill();
			}
			string text = streamReader.ReadToEnd();
			if (recordLog)
			{
				Trace.WriteLine(text);
			}
			return text;
		}
		catch (Exception ex)
		{
			Trace.WriteLine(ex);
			return ex.Message;
		}
	}

	public static string GetOsVersion()
	{
		try
		{
			return Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion")?.GetValue("ProductName").ToString();
		}
		catch
		{
			return Environment.OSVersion.VersionString;
		}
	}

	public static float GetCpuUsageForProcess()
	{
		using Process process = Process.GetCurrentProcess();
		string processName = process.ProcessName;
		using PerformanceCounter performanceCounter = new PerformanceCounter("Process", "% Processor Time", processName);
		performanceCounter.NextValue();
		return performanceCounter.NextValue();
	}

	public static float GetCpuUsageForProcess(string processName)
	{
		using PerformanceCounter performanceCounter = new PerformanceCounter("Process", "% Processor Time", processName);
		performanceCounter.NextValue();
		return performanceCounter.NextValue();
	}

	public static bool CheckIsInsertMicrosoftEdgeRuntime()
	{
		bool is64BitProcess = Environment.Is64BitProcess;
		string name = "SOFTWARE\\WOW6432Node\\Microsoft\\EdgeUpdate\\Clients\\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}";
		string name2 = "Software\\Microsoft\\EdgeUpdate\\Clients\\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}";
		if (!is64BitProcess)
		{
			name = "SOFTWARE\\Microsoft\\EdgeUpdate\\Clients\\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}";
			name2 = "Software\\Microsoft\\EdgeUpdate\\Clients\\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}";
		}
		bool flag = false;
		bool flag2 = false;
		using (RegistryKey registryKey = Registry.LocalMachine?.OpenSubKey(name))
		{
			flag = registryKey != null;
		}
		using (RegistryKey registryKey2 = Registry.CurrentUser?.OpenSubKey(name2))
		{
			flag2 = registryKey2 != null;
		}
		return flag || flag2;
	}

	public static List<string> GetSerialNumber()
	{
		List<string> list = new List<string>();
		try
		{
			foreach (ManagementObject item2 in new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMedia").Get())
			{
				string item = item2["SerialNumber"].ToString().Trim();
				list.Add(item);
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
		return list;
	}

	public static string GetSerialNumberByCmd()
	{
		string text = "wmic diskdrive get SerialNumber";
		Process process = new Process();
		process.StartInfo.FileName = "cmd.exe";
		process.StartInfo.UseShellExecute = false;
		process.StartInfo.RedirectStandardInput = true;
		process.StartInfo.RedirectStandardOutput = true;
		process.StartInfo.RedirectStandardError = true;
		process.StartInfo.CreateNoWindow = true;
		process.Start();
		Thread.Sleep(1000);
		process.StandardInput.WriteLine(text + "&exit");
		process.StandardInput.AutoFlush = true;
		string text2 = process.StandardOutput.ReadToEnd();
		process.WaitForExit();
		process.Close();
		List<string> list = new List<string>();
		string[] array = text2.Split(new char[2] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < array.Length; i++)
		{
			list.Add(array[i].Trim());
		}
		if (list.Count > 4)
		{
			return list[4];
		}
		return "";
	}

	static WindowsEx()
	{
		UEc9SjaZgO = new object();
	}
}
