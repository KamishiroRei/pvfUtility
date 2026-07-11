using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace PvfCode.Services.TimeServices;

public class GameProcessTimeService : BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			await Task.Delay(500);
			try
			{
				bool flag = false;
				Process[] processes = Process.GetProcesses();
				for (int i = 0; i < processes.Length; i++)
				{
					if (processes[i].ProcessName.ToLower() == "dnf")
					{
						AppSetting.Instance.GameOptions.GameUserA.GameIsStop = false;
						flag = true;
					}
				}
				if (!flag)
				{
					AppSetting.Instance.GameOptions.GameUserA.GameIsStop = true;
				}
			}
			catch (Exception)
			{
			}
		}
	}

	public GameProcessTimeService()
	{
	}
}
