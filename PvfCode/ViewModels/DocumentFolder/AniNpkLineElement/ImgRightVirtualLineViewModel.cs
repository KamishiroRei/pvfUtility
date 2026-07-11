using System.Collections.Generic;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using PvfCode.Dot;

namespace PvfCode.ViewModels.DocumentFolder.AniNpkLineElement;

internal class ImgRightVirtualLineViewModel : ViewModelBase
{
	private readonly KeyValuePair<string, int> imageInfo;

	public bool IsLoading
	{
		get => GetProperty(() => IsLoading);
		set => SetProperty(() => IsLoading, value);
	}

	public ImgRightVirtualLineViewModel(KeyValuePair<string, int> imageInfo)
	{
		this.imageInfo = imageInfo;
		IsLoading = true;
	}

	[Command]
	public void OnPreview()
	{
	}

	[Command]
	public void OnGoToFile()
	{
		string imagePath = imageInfo.Key.Replace("%04d", "0001").Replace("%02d%02d", "0001");
		ResultData result = ImagePack2Service.Instance.ImgGoToNpkFile(imagePath);
		if (result.IsError)
		{
			AppCore.ShowMsg(result.Msg);
		}
	}
}
