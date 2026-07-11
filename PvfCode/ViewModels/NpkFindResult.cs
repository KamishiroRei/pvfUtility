using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using PvfCode.NPK.Utils.Models;
using Utools;

namespace PvfCode.ViewModels;

public class NpkFindResult : ViewModelBase
{
	[CompilerGenerated]
	private string XHNFEYM05N;

	[CompilerGenerated]
	private UtImgFile PFaFOKqXRl;

	public string ImagePath
	{
		[CompilerGenerated]
		get
		{
			return XHNFEYM05N;
		}
		[CompilerGenerated]
		set
		{
			XHNFEYM05N = value;
		}
	}

	public string ImageFileName
	{
		get
		{
			if (!string.IsNullOrEmpty(ImagePath))
			{
				return Path.GetFileName(ImagePath);
			}
			return ImagePath;
		}
	}

	public UtImgFile ImgFile
	{
		[CompilerGenerated]
		get
		{
			return PFaFOKqXRl;
		}
		[CompilerGenerated]
		set
		{
			PFaFOKqXRl = value;
		}
	}

	public string NpkFilePath => ImagePack2Service.Instance.GetNpkFilePath(ImgFile);

	public string NpkFileName
	{
		get
		{
			if (!string.IsNullOrEmpty(NpkFilePath))
			{
				return Path.GetFileName(NpkFilePath);
			}
			return NpkFilePath;
		}
	}

	public NpkFindResult(UtImgFile utImgFile, string imagePath)
	{
		ImgFile = utImgFile;
		ImagePath = imagePath;
	}

	[Command]
	public void OnGoToNpkFile()
	{
		if (((int)Keyboard.Modifiers & 2) == 2)
		{
			if (string.IsNullOrEmpty(NpkFilePath) || !File.Exists(NpkFilePath))
			{
				AppCore.ShowMsg("NPK文件不存在");
			}
			else
			{
				FileHelper.OpenFolderAndSelectFile(NpkFilePath);
			}
		}
	}
}
