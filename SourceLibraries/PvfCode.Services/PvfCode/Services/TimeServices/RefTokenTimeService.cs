using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace PvfCode.Services.TimeServices;

public class RefTokenTimeService : BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			await Task.Delay(3600000);
			try
			{
				await ServiceCloud.Instance.RefreshToken();
			}
			catch (Exception ex)
			{
				AppSetting.Instance.GetIlogger()?.Error("TOKEN错误请务必复制发送给作者QQ812143836 ：" + ex.Message);
			}
		}
	}

	public RefTokenTimeService()
	{
	}
}
