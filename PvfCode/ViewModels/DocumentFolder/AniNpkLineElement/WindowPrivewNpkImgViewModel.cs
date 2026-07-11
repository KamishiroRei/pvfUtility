using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using PvfCode.NPK.Utils;
using PvfCode.NPK.Utils.Models;

namespace PvfCode.ViewModels.DocumentFolder.AniNpkLineElement;

public class WindowPrivewNpkImgViewModel : ViewModelBase
{
	[CompilerGenerated]
	private string ixdGLu15js;

	[CompilerGenerated]
	private string NlNGnUaMhc;

	[CompilerGenerated]
	private int EqyGqWh74q;

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

	public string ImgPath
	{
		[CompilerGenerated]
		get
		{
			return ixdGLu15js;
		}
		[CompilerGenerated]
		set
		{
			ixdGLu15js = value;
		}
	}

	public string NpkPath
	{
		[CompilerGenerated]
		get
		{
			return NlNGnUaMhc;
		}
		[CompilerGenerated]
		set
		{
			NlNGnUaMhc = value;
		}
	}

	public int Index
	{
		[CompilerGenerated]
		get
		{
			return EqyGqWh74q;
		}
		[CompilerGenerated]
		set
		{
			EqyGqWh74q = value;
		}
	}

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
		ImagePack imagePack = list.Find((ImagePack P_0) => P_0.Name == Path.GetFileName(ImgPath.ToLower()));
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

	[CompilerGenerated]
	private bool eaWGsn7WAU(ImagePack P_0)
	{
		return P_0.Name == Path.GetFileName(ImgPath.ToLower());
	}
}
