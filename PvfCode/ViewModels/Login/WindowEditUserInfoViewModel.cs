using System;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using Microsoft.WindowsAPICodePack.Dialogs;
using PvfCode.Dot;
using PvfCode.Dot.Desktop;
using Utools;

namespace PvfCode.ViewModels.Login;

public class WindowEditUserInfoViewModel : ViewModelBase
{
	private readonly Action Close;

	[CompilerGenerated]
	private EditUserInfoRes c8BmchmnFj;

	public EditUserInfoRes UserInfo
	{
		[CompilerGenerated]
		get
		{
			return c8BmchmnFj;
		}
		[CompilerGenerated]
		set
		{
			c8BmchmnFj = value;
		}
	}

	public bool AllowEditPhone
	{
		get
		{
			return GetProperty(() => AllowEditPhone);
		}
		set
		{
			SetProperty(() => AllowEditPhone, value);
		}
	}

	public ImageSource AvatarImageSource
	{
		get
		{
			return GetProperty(() => AvatarImageSource);
		}
		set
		{
			SetProperty<ImageSource>(() => AvatarImageSource, value);
		}
	}

	public WindowEditUserInfoViewModel(Action close)
	{
		UserInfo = new EditUserInfoRes();
		Close = close;
		if (ServiceCloud.Instance.User != null)
		{
			UserInfo.NickName = ServiceCloud.Instance.User.NickName;
			UserInfo.Phone = ServiceCloud.Instance.User.Phone;
			AllowEditPhone = UserInfo.Phone == 0;
		}
		string text = ServiceCloud.Instance.User?.Avatar;
		if (text != null)
		{
			if (text.ToLower().Contains("http://") || text.ToLower().Contains("https://"))
			{
				AvatarImageSource = pWpmUFZBpI(text);
				return;
			}
			byte[] bytes = ImageHelper.Base64ImageToBytes(text);
			AvatarImageSource = ImageSourceUtils.ConvertByteArrayToBitmapImage(bytes);
		}
	}

	[Command]
	public async void Save()
	{
		byte[] avatarBytes = ImageSourceUtils.ConvertBitmapSourceToByteArray(AvatarImageSource);
		UserInfo.AvatarBytes = avatarBytes;
		ResultData resultData = await ServiceCloud.Instance.SaveUserInfo(UserInfo);
		if (resultData.IsError)
		{
			AppCore.ShowMsg(resultData.Msg, isError: true);
			return;
		}
		ServiceCloud.Instance.User.NickName = UserInfo.NickName;
		ServiceCloud.Instance.User.Avatar = ImageHelper.BytesToBase64Image(UserInfo.AvatarBytes);
		ServiceCloud.Instance.User.Phone = UserInfo.Phone;
		Close();
	}

	[Command]
	public void ChangedAvatarImageSource()
	{
		CommonOpenFileDialog commonOpenFileDialog = new CommonOpenFileDialog
		{
			Title = AppSetting.Instance.GetIlogger().GetStr("SelectAvatarWindowTitle"),
			IsFolderPicker = false,
			Multiselect = false,
			AllowPropertyEditing = true,
			EnsurePathExists = true,
			EnsureValidNames = true
		};
		commonOpenFileDialog.Filters.Add(new CommonFileDialogFilter(AppSetting.Instance.GetIlogger()?.GetStr("Luanguage_ImageName"), ".jpg;*.png;*.jpeg;*.bmp;*.gif|" + AppSetting.Instance.GetIlogger()?.GetStr("Luanguage_AllFiles") + "|*.*"));
		if (commonOpenFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
		{
			AvatarImageSource = new BitmapImage(new Uri(commonOpenFileDialog.FileName));
		}
	}

	private ImageSource pWpmUFZBpI(string? url)
	{
		if (string.IsNullOrEmpty(url))
		{
			return null;
		}
		try
		{
			BitmapImage bitmapImage = new BitmapImage();
			bitmapImage.BeginInit();
			bitmapImage.UriSource = new Uri(url, UriKind.Absolute);
			bitmapImage.DecodePixelWidth = 300;
			bitmapImage.EndInit();
			return bitmapImage;
		}
		catch (Exception)
		{
			return null;
		}
	}
}
