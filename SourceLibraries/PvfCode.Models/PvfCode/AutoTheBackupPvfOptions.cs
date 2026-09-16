using System;
using System.IO;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using PvfCode.Models.Pvf;
using Utools;

namespace PvfCode;

[JsonObject(MemberSerialization.OptOut)]
public class AutoTheBackupPvfOptions : ViewModelBase
{
	private bool? qI6Qk7oRR;

	private int? f8p6nbkyP;

	private int z7tyKyf6p;

	private bool YxVwxfFSd;

	private string gS7oKPPxA;

	private string v3F2regFo;

	public bool AutoTheBackupPvfIsOpen
	{
		get
		{
			if (!qI6Qk7oRR.HasValue)
			{
				qI6Qk7oRR = true;
			}
			return qI6Qk7oRR.Value;
		}
		set
		{
			qI6Qk7oRR = value;
			RaisePropertyChanged("AutoTheBackupPvfIsOpen");
		}
	}

	public int SleepTimeMinutes
	{
		get
		{
			if (!f8p6nbkyP.HasValue)
			{
				f8p6nbkyP = 15;
			}
			if (f8p6nbkyP <= 0)
			{
				f8p6nbkyP = 1;
			}
			return f8p6nbkyP.Value;
		}
		set
		{
			f8p6nbkyP = value;
			RaisePropertyChanged("SleepTimeMinutes");
		}
	}

	public int BackupPvfMaxCount
	{
		get
		{
			if (z7tyKyf6p <= 0)
			{
				z7tyKyf6p = 1;
			}
			return z7tyKyf6p;
		}
		set
		{
			z7tyKyf6p = value;
			RaisePropertyChanged("BackupPvfMaxCount");
		}
	}

	public bool BackupSuccessLog
	{
		get
		{
			return YxVwxfFSd;
		}
		set
		{
			YxVwxfFSd = value;
			RaisePropertyChanged("BackupSuccessLog");
		}
	}

	public string BackupPath
	{
		get
		{
			if (string.IsNullOrEmpty(gS7oKPPxA))
			{
				gS7oKPPxA = Path.Combine(AppSetting.AppBasePath, "BackupPvf");
			}
			if (!Directory.Exists(gS7oKPPxA))
			{
				FileHelper.CheckDir(gS7oKPPxA);
			}
			if (!Directory.Exists(gS7oKPPxA))
			{
				gS7oKPPxA = Path.Combine(AppSetting.AppBasePath, "BackupPvf");
				FileHelper.CheckDir(gS7oKPPxA);
			}
			return gS7oKPPxA;
		}
		set
		{
			gS7oKPPxA = value;
			RaisePropertyChanged("BackupPath");
		}
	}

	public BackUpPvfModel BackUpPvfModel
	{
		get
		{
			return GetProperty(() => BackUpPvfModel);
		}
		set
		{
			SetProperty(() => BackUpPvfModel, value);
			RaisePropertyChanged("ShowBackUpDirName");
		}
	}

	public string BackUpDirName
	{
		get
		{
			if (string.IsNullOrEmpty(v3F2regFo))
			{
				v3F2regFo = "pvfUtility_BackUpPvf";
			}
			return v3F2regFo;
		}
		set
		{
			v3F2regFo = value;
			RaisePropertyChanged("BackUpDirName");
		}
	}

	public bool ShowBackUpDirName => BackUpPvfModel == BackUpPvfModel.在当前打开的PVF所在目录下进行备份;

	public string CreateFilePath(PvfPack pack)
	{
		return Path.Combine(GetBackUpPath(pack), "script_" + DateTime.Now.ToLogTime() + ".pvf");
	}

	public string GetBackUpPath(PvfPack pack)
	{
		string text = ((AppSetting.Instance.PvfConfig.AutoTheBackupPvfConfig.BackUpPvfModel != BackUpPvfModel.在当前打开的PVF所在目录下进行备份) ? AppSetting.Instance.PvfConfig.AutoTheBackupPvfConfig.BackupPath : Path.Combine(Path.GetDirectoryName(pack.PvfPackFilePath), AppSetting.Instance.PvfConfig.AutoTheBackupPvfConfig.BackUpDirName));
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		return text;
	}

	[Command]
	public void OpenBackupPath()
	{
		try
		{
			FileHelper.OpenFolderAndSelectFile(BackupPath);
		}
		catch (Exception ex)
		{
			AppSetting.Instance.GetIlogger()?.ShowMsg(ex.Message, isError: true);
		}
	}

	[Command]
	public void OnSelectBackupPath()
	{
		CommonOpenFileDialog commonOpenFileDialog = new CommonOpenFileDialog
		{
			IsFolderPicker = true,
			Title = AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseSelectPvfPath")
		};
		if (commonOpenFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
		{
			BackupPath = commonOpenFileDialog.FileName;
		}
	}

	public AutoTheBackupPvfOptions()
	{
	}
}
