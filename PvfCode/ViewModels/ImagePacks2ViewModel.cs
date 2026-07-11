using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using Microsoft.WindowsAPICodePack.Dialogs;
using PvfCode.Dot;

namespace PvfCode.ViewModels;

public class ImagePacks2ViewModel : ViewModelBase
{
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

	[Command]
	public async void OnLoadImagePacks2()
	{
		CommonOpenFileDialog commonOpenFileDialog = new CommonOpenFileDialog
		{
			Title = AppSetting.Instance.GetIlogger().GetStr("mess_SelectDNFClientModelDir"),
			IsFolderPicker = true,
			Multiselect = false,
			AllowPropertyEditing = true,
			EnsurePathExists = true,
			EnsureValidNames = true
		};
		if (commonOpenFileDialog.ShowDialog(Application.Current.MainWindow) == CommonFileDialogResult.Ok)
		{
			AppSetting.Instance.ImagePacks2Options.ImagePacks2Path = commonOpenFileDialog.FileName;
			if (DiskDetectionUtils.DetectDrive(AppSetting.Instance.ImagePacks2Options.ImagePacks2Path).HardwareType != HardwareType.Ssd)
			{
				AppCore.Logger.Warning(AppCore.Logger.GetStrNoReplace("Mess_NotSSD"));
			}
			await Task.Run((Func<Task?>)LoadImagePacks2);
			await AppSetting.Instance.SaveSetting();
		}
	}

	public async Task LoadImagePacks2()
	{
		AppCore.Logger.Warning(AppSetting.Instance.GetIlogger()?.GetStr("mess_ImagePack2ModelPatchLoading"));
		if (string.IsNullOrEmpty(AppSetting.Instance.ImagePacks2Options.ImagePacks2Path))
		{
			AppCore.Logger.Error(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseSelectImagePacks2ModelDir"));
			return;
		}
		if (!Directory.Exists(AppSetting.Instance.ImagePacks2Options.ImagePacks2Path))
		{
			AppCore.Logger.Error(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_DirNotExist"), AppSetting.Instance.ImagePacks2Options.ImagePacks2Path));
			return;
		}
		IsLoading = true;
		ResultData resultData = await ImagePack2Service.Instance.LoadImagePack2Async(AppSetting.Instance.ImagePacks2Options.ImagePacks2Path);
		if (resultData.IsError)
		{
			AppCore.Logger.Error(resultData.Msg);
		}
		if (AppCore.ViewModelBase.PVF != null && AppCore.ViewModelBase.PVF.PvfIsOpen && ImagePack2Service.Instance.Count > 0)
		{
			ImagePack2Service.Instance.LoadTreeIcons(AppCore.ViewModelBase.PVF);
		}
		IsLoading = false;
	}

	[Command]
	public async void OnRefresh()
	{
		await Task.Run((Func<Task?>)LoadImagePacks2);
		await AppSetting.Instance.SaveSetting();
	}

	[Command]
	public void OnClear()
	{
		AppSetting.Instance.ImagePacks2Options.ImagePacks2Path = "";
		ImagePack2Service.Instance.Clear();
	}

	public ImagePacks2ViewModel()
	{
	}
}
