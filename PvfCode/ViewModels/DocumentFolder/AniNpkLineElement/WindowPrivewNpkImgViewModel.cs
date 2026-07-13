using System.Collections.Generic;
using System.IO;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using PvfCode.NPK.Utils;
using PvfCode.NPK.Utils.Models;

namespace PvfCode.ViewModels.DocumentFolder.AniNpkLineElement;

public class WindowPrivewNpkImgViewModel : ViewModelBase
{
	public bool Loading
	{
		get
		{
			return GetProperty(() => Loading);
		}
		set
		{
			SetProperty(() => Loading, value);
		}
	}

	public string ImgPath { get; set; }

	public string NpkPath { get; set; }

	public int Index { get; set; }

	public ImgFile Sprite
	{
		get
		{
			return GetProperty(() => Sprite);
		}
		set
		{
			SetProperty<ImgFile>(() => Sprite, value);
		}
	}

	public WindowPrivewNpkImgViewModel(string imgPath, string npkPath, int index)
	{
		ImgPath = imgPath;
		NpkPath = npkPath;
		Index = index;
	}

	[Command]
	public void Loaded()
	{
		Loading = true;
		List<ImagePack> list = NpkCoder.ReadNpk(NpkPath);
		if (list == null || list.Count == 0)
		{
			Loading = false;
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_NpkNotContainImg"), isError: true);
			return;
		}
		if (!File.Exists(NpkPath))
		{
			AppCore.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_NpkNotExist"), NpkPath), isError: true);
		}
		ImagePack imagePack = list.Find((ImagePack pack) => pack.Name == Path.GetFileName(ImgPath.ToLower()));
		if (imagePack == null)
		{
			Loading = false;
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_NpkNotContainImg"), isError: true);
		}
		else if (Index < imagePack.Count)
		{
			Sprite = imagePack.ImgList[Index];
			Loading = false;
		}
		else
		{
			AppCore.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_NpkNotContainImgIndex"), Index), isError: true);
			Loading = false;
		}
	}
}
