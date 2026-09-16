using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using PvfCode.Dot;

namespace PvfCode.Services;

internal class PvfAutoBackupService : BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			await Task.Delay(AppSetting.Instance.PvfConfig.AutoTheBackupPvfConfig.SleepTimeMinutes * 60000);
			if (!AppSetting.Instance.PvfConfig.AutoTheBackupPvfConfig.AutoTheBackupPvfIsOpen)
			{
				continue;
			}

			try
			{
				PvfGroup pvf = AppCore.ViewModelBase.PVF;
				if (!pvf.PvfIsOpen)
				{
					continue;
				}
				// 脏检查：无未保存变更时跳过整包重建，避免周期性 CPU/内存尖峰
				if (!pvf.HasUnsavedChanges)
				{
					continue;
				}

				string backupPath = AppSetting.Instance.PvfConfig.AutoTheBackupPvfConfig.CreateFilePath(pvf);
				ResultData result = await Task.Run(() => pvf.SavePvfPack(backupPath, isFastMode: true, null));
				if (result.IsError)
				{
					AppCore.Logger.Error(
						AppSetting.Instance.GetIlogger()?.GetStr("AutoTheBackupPvfTimeService_BackupPvfError") + result.Msg);
					continue;
				}

				if (AppSetting.Instance.PvfConfig.AutoTheBackupPvfConfig.BackupSuccessLog)
				{
					AppCore.Logger.Success(
						AppSetting.Instance.GetIlogger()?.GetStr("AutoTheBackupPvfTimeService_BackupSuccess") + backupPath);
				}

				string backupDirectory = AppSetting.Instance.PvfConfig.AutoTheBackupPvfConfig.GetBackUpPath(pvf);
				string[] backupFiles = Directory.GetFiles(backupDirectory, "*.pvf");
				if (backupFiles == null || backupFiles.Length <= 1)
				{
					continue;
				}

				List<FileInfo> fileInfos = backupFiles.Select(path => new FileInfo(path)).ToList();
				IEnumerable<FileInfo> filesToKeep = fileInfos
					.OrderByDescending(file => file.CreationTime)
					.Take(AppSetting.Instance.PvfConfig.AutoTheBackupPvfConfig.BackupPvfMaxCount);

				foreach (string filePath in backupFiles)
				{
					if (filesToKeep.Any(file => file.FullName == filePath))
					{
						continue;
					}

					try
					{
						File.Delete(filePath);
					}
					catch (Exception)
					{
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(
					AppSetting.Instance.GetIlogger()?.GetStr("AutoTheBackupPvfTimeService_BackupPvfError") + e.Message);
			}
		}
	}
}
