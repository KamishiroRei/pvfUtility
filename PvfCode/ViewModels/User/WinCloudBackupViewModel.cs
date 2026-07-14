using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using PvfCode.Dot;
using PvfCode.Dot.Desktop;
using Utools;

namespace PvfCode.ViewModels.User;

public class WinCloudBackupViewModel : ViewModelBase
{
	public bool BackUpSuccess
	{
		get
		{
			return GetProperty(() => BackUpSuccess);
		}
		set
		{
			SetProperty(() => BackUpSuccess, value);
		}
	}

	public bool IsLoading
	{
		get
		{
			return GetProperty(() => IsLoading);
		}
		set
		{
			SetProperty(() => IsLoading, value);
		}
	}

	public AccountCloudBackupDto BackUpData
	{
		get
		{
			return GetProperty(() => BackUpData);
		}
		set
		{
			SetProperty<AccountCloudBackupDto>(() => BackUpData, value);
		}
	}

	public WinCloudBackupViewModel()
	{
	}

	[Command]
	public async void Loaded()
	{
		IsLoading = true;
		await Task.Run(async delegate
		{
			ResultData<AccountCloudBackupDto> resultData = await ServiceCloud.Instance.DownloadAccountCloudBackup();
			if (resultData.IsError)
			{
				AppCore.ShowMsg(resultData.Msg, isError: true);
			}
			if (resultData.Data != null)
			{
				BackUpData = resultData.Data;
			}
		});
		if (BackUpData == null)
		{
			BackUpData = new AccountCloudBackupDto();
		}
		if (BackUpData.AppSetting == null)
		{
			BackUpData.AppSetting = new UserCloudBackUpData();
		}
		if (BackUpData.BookMark == null)
		{
			BackUpData.BookMark = new UserCloudBackUpData();
		}
		if (BackUpData.SectionComment == null)
		{
			BackUpData.SectionComment = new UserCloudBackUpData();
		}
		if (BackUpData.ItemCodeHoverConfig == null)
		{
			BackUpData.ItemCodeHoverConfig = new UserCloudBackUpData();
		}
		if (BackUpData.TreeListComment == null)
		{
			BackUpData.TreeListComment = new UserCloudBackUpData();
		}
		IsLoading = false;
	}

	[Command]
	public async void OnBackupStart()
	{
		IsLoading = true;
		if (BackUpData.BookMark.AllowBackUp)
		{
			BackUpData.BookMark.Data = AppSetting.Instance.BookMarkGroup.ToJson();
		}
		if (BackUpData.TreeListComment.AllowBackUp)
		{
			BackUpData.TreeListComment.Data = AppSetting.Instance.PvfConfig.TreelistCommentDic.ToJson();
		}
		if (BackUpData.SectionComment.AllowBackUp)
		{
			ResultData<List<PvfCommentDto>> resultData = await ServicePvfTabComment.Instance.GetList();
			if (resultData.IsError)
			{
				BackUpData.SectionComment.AllowBackUp = false;
				AppCore.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_BackupTabCommentFail"), resultData.Msg));
			}
			else
			{
				BackUpData.SectionComment.Data = resultData.Data.ToJson();
			}
		}
		if (BackUpData.ItemCodeHoverConfig.AllowBackUp)
		{
			if (File.Exists(AppSetting.Instance.EditConfig.ItemCodeConvertItemNameConfiger.SavePath))
			{
				BackUpData.ItemCodeHoverConfig.Data = File.ReadAllText(AppSetting.Instance.EditConfig.ItemCodeConvertItemNameConfiger.SavePath);
			}
			else
			{
				AppCore.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_NoFoundCodeTipFile"), AppSetting.Instance.EditConfig.ItemCodeConvertItemNameConfiger.SavePath));
			}
		}
		if (BackUpData.AppSetting.AllowBackUp)
		{
			BackUpData.AppSetting.Data = AppSetting.Instance.ToJson();
		}
		await Task.Run(async delegate
		{
			ResultData resultData2 = await ServiceCloud.Instance.UploadAccountCloudBackup(BackUpData);
			if (resultData2.IsError)
			{
				AppCore.ShowMsg(resultData2.Msg, isError: true);
			}
			else
			{
				if (BackUpData.BookMark.AllowBackUp)
				{
					BackUpData.BookMark.BackUpTime = DateTime.Now;
				}
				if (BackUpData.TreeListComment.AllowBackUp)
				{
					BackUpData.TreeListComment.BackUpTime = DateTime.Now;
				}
				if (BackUpData.SectionComment.AllowBackUp)
				{
					BackUpData.SectionComment.BackUpTime = DateTime.Now;
				}
				if (BackUpData.ItemCodeHoverConfig.AllowBackUp)
				{
					BackUpData.ItemCodeHoverConfig.BackUpTime = DateTime.Now;
				}
				if (BackUpData.AppSetting.AllowBackUp)
				{
					BackUpData.AppSetting.BackUpTime = DateTime.Now;
				}
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_UploadSuccess"));
			}
		});
		BackUpSuccess = true;
		IsLoading = false;
	}

	[Command]
	public async void OnDownloadBackupStart()
	{
		IsLoading = true;
		await Task.Run(async delegate
		{
			ResultData<AccountCloudBackupDto> resultData = await ServiceCloud.Instance.DownloadAccountCloudBackup();
			if (resultData.IsError)
			{
				AppCore.ShowMsg(resultData.Msg, isError: true);
			}
			else
			{
				AccountCloudBackupDto data = resultData.Data;
				if (data.AppSetting != null && !string.IsNullOrEmpty(data.AppSetting.Data))
				{
					AppSetting.Instance = data.AppSetting.Data.JsonToObject<AppSetting>();
				}
				if (data.BookMark != null)
				{
					AppSetting.Instance.BookMarkGroup = data.BookMark.Data.JsonToObject<BookMarkGroupDto>();
				}
				if (data.TreeListComment != null)
				{
					AppSetting.Instance.PvfConfig.TreelistCommentDic = data.TreeListComment.Data.JsonToObject<Dictionary<string, TreelistCommentRes>>();
				}
				if (data.SectionComment != null)
				{
					List<PvfCommentDto> list = data.SectionComment.Data.JsonToObject<List<PvfCommentDto>>();
					if (list != null && list.Any())
					{
						await ServicePvfTabComment.Instance.AddRagned(list);
					}
				}
				if (data.ItemCodeHoverConfig != null)
				{
					File.WriteAllText(AppSetting.Instance.EditConfig.ItemCodeConvertItemNameConfiger.SavePath, data.ItemCodeHoverConfig.Data);
					AppSetting.Instance.EditConfig.ItemCodeConvertItemNameConfiger.XmlToModel(showErrDialog: true);
				}
				await AppSetting.Instance.SaveSetting();
			}
		});
		IsLoading = false;
	}
}
