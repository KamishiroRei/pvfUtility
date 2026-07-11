using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace PvfCode.Services.TimeServices;

public class CleanMemoryTimeService : BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			await Task.Delay(600000);
			try
			{
				await Task.Run((Action)GC.Collect);
			}
			catch (Exception)
			{
			}
		}
	}

	[DllImport("kernel32.dll")]
	public static extern int SetProcessWorkingSetSize(IntPtr process, int minSize, int maxSize);

	public static void ClearMemory()
	{
		GC.Collect();
		GC.WaitForPendingFinalizers();
		_ = Environment.OSVersion.Platform;
		SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1);
		AppSetting.Instance.GetIlogger().Error("clear");
	}

	public CleanMemoryTimeService()
	{
	}
}
