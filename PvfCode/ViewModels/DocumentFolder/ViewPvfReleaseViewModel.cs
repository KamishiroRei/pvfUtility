using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using AnyClone;
using DevExpress.Mvvm.DataAnnotations;
using Microsoft.WindowsAPICodePack.Dialogs;
using PvfCode.Dot;
using PvfCode.Models.Pvf.Enums;
using PvfCode.Services.PvfRelease;
using PvfCode.ViewModels.DocumentFolder.Enums;
using Utools;

namespace PvfCode.ViewModels.DocumentFolder;

public class ViewPvfReleaseViewModel : DocumentBase
{
	public string WaitIndicatorContent
	{
		get
		{
			return GetProperty(() => WaitIndicatorContent);
		}
		set
		{
			SetProperty<string>(() => WaitIndicatorContent, value);
		}
	}

	public PvfReleaseType TargetType
	{
		get
		{
			return GetProperty(() => TargetType);
		}
		set
		{
			SetProperty(() => TargetType, value);
			RaisePropertyChanged("TargetFolder");
		}
	}

	public string TargetFolder
	{
		get
		{
			return TargetType switch
			{
				PvfReleaseType.客户端 => AppSetting.Instance.PvfConfig.ReleaseLog.ClientLog.TargetPath, 
				PvfReleaseType.服务端 => AppSetting.Instance.PvfConfig.ReleaseLog.ServerLog.TargetPath, 
				_ => null, 
			};
		}
		set
		{
			switch (TargetType)
			{
			case PvfReleaseType.客户端:
				AppSetting.Instance.PvfConfig.ReleaseLog.ClientLog.TargetPath = value;
				break;
			case PvfReleaseType.服务端:
				AppSetting.Instance.PvfConfig.ReleaseLog.ServerLog.TargetPath = value;
				break;
			}
			RaisePropertyChanged("TargetFolder");
		}
	}

	private string GetTargetFilePath()
	{
		return Path.Combine(TargetFolder, "Script.pvf");
	}

	public ViewPvfReleaseViewModel()
		: base(AppSetting.Instance.GetIlogger()?.GetStr("DocumentName_Publish"))
	{
		base.DocumentType = PvfFileDocumentType.发布;
		TargetType = PvfReleaseType.客户端;
	}

	[Command]
	public void OnOpenTargetFolder()
	{
		if (!string.IsNullOrEmpty(TargetFolder))
		{
			if (Directory.Exists(TargetFolder))
			{
				FileHelper.OpenFolderAndSelectFile(Path.Combine(TargetFolder, "Script.pvf"));
			}
			else
			{
				AppCore.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_DirNotExist"), TargetFolder));
			}
		}
	}

	[Command]
	public async void OnSelectTargetPath()
	{
		CommonOpenFileDialog commonOpenFileDialog = new CommonOpenFileDialog
		{
			IsFolderPicker = true,
			Title = AppSetting.Instance.GetIlogger().GetStr("Title_SelectFolder")
		};
		if (commonOpenFileDialog.ShowDialog(Application.Current.MainWindow) == CommonFileDialogResult.Ok)
		{
			TargetFolder = commonOpenFileDialog.FileName;
			await AppSetting.Instance.SaveSetting();
		}
	}

	[Command]
	public async void OnStart()
	{
		if (string.IsNullOrEmpty(TargetFolder))
		{
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseSelectPublishDir"));
			return;
		}
		if (!Directory.Exists(TargetFolder))
		{
			FileHelper.CheckDir(TargetFolder);
		}
		if (GetTargetFilePath() == AppCore.ViewModelBase.PVF.PvfPackFilePath)
		{
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PublishDirCannotSameAsPvfDir"));
			return;
		}
		base.IsLoading = true;
		await Task.Run((Func<Task?>)PublishAsync);
		base.IsLoading = false;
	}

	private async Task PublishAsync()
	{
		WaitIndicatorContent = AppSetting.Instance.GetIlogger().GetStr("mess_ClonePvfPack");
		PvfGroup pvf = AppCore.ViewModelBase.PVF.Clone();
		WaitIndicatorContent = AppSetting.Instance.GetIlogger().GetStr("mess_Publishing");
		PvfReleaseBase pvfReleaseBase = ((TargetType != PvfReleaseType.客户端) ? ((PvfReleaseBase)new ServicePvfReleaseToServer(pvf)) : ((PvfReleaseBase)new ServicePvfReleaseToClient(pvf)));
		if (pvfReleaseBase != null)
		{
			ResultData resultData = await pvfReleaseBase.Start(AppSetting.Instance.PvfConfig.ReleaseLog.ClientReleaseOptions);
			if (resultData.IsError)
			{
				AppSetting.Instance.PvfConfig.ReleaseLog.LastReleaseType = TargetType;
				AppSetting.Instance.PvfConfig.ReleaseLog.LastLog.Time = DateTime.Now;
				AppSetting.Instance.PvfConfig.ReleaseLog.LastLog.TargetPath = TargetFolder;
				AppSetting.Instance.PvfConfig.ReleaseLog.LastLog.IsSuccess = false;
				AppSetting.Instance.PvfConfig.ReleaseLog.LastLog.Error = resultData.Msg;
				AppCore.Logger.Error(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_PublishError"), resultData.Msg));
				AppCore.ShowMsg(resultData.Msg, isError: true);
			}
			else
			{
				ResultData resultData2 = await pvf.SavePvfPack(GetTargetFilePath(), isFastMode: false, AppCore.ViewModelBase.MainProgress);
				AppSetting.Instance.PvfConfig.ReleaseLog.LastReleaseType = TargetType;
				AppSetting.Instance.PvfConfig.ReleaseLog.LastLog.Time = DateTime.Now;
				AppSetting.Instance.PvfConfig.ReleaseLog.LastLog.TargetPath = TargetFolder;
				if (resultData2.IsError)
				{
					AppSetting.Instance.PvfConfig.ReleaseLog.LastLog.IsSuccess = false;
					AppSetting.Instance.PvfConfig.ReleaseLog.LastLog.Error = resultData2.Msg;
				}
				else
				{
					AppSetting.Instance.PvfConfig.ReleaseLog.LastLog.IsSuccess = true;
					AppCore.Logger.Success(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_PublishSuccess"), GetTargetFilePath()));
				}
			}
			await AppSetting.Instance.SaveSetting();
		}
		await Task.Run(delegate
		{
			WindowsEx.ClearMemorySilent(Process.GetCurrentProcess());
		});
		GC.Collect();
		GC.WaitForPendingFinalizers();
		GC.Collect();
	}

	[Command]
	public void OnSelectall(bool val)
	{
		AppSetting.Instance.PvfConfig.ReleaseLog.ClientReleaseOptions.Select(val);
	}

	public override void Dispose()
	{
	}
}
