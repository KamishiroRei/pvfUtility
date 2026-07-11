using System.Runtime.CompilerServices;
using System.Windows.Media;
using DevExpress.Mvvm;
using PvfCode.Services.PreviewPvfFileFolder.NpcShop;
using PvfCode.Services.PreviewPvfFileFolder.Stackable;
using Utools;

namespace PvfCode.Services.PreviewPvfFileFolder;

public abstract class FilePreviewDataBase : ViewModelBase
{
	[CompilerGenerated]
	private PvfFile zlQjCMlwC3;

	[CompilerGenerated]
	private PvfGroup VsyjlUMGZI;

	[CompilerGenerated]
	private ImageSource Dc2jYaj1xQ;

	public PvfFile File
	{
		[CompilerGenerated]
		get
		{
			return zlQjCMlwC3;
		}
		[CompilerGenerated]
		set
		{
			zlQjCMlwC3 = value;
		}
	}

	public PvfGroup Pvf
	{
		[CompilerGenerated]
		get
		{
			return VsyjlUMGZI;
		}
		[CompilerGenerated]
		set
		{
			VsyjlUMGZI = value;
		}
	}

	public ImageSource ImageSource
	{
		[CompilerGenerated]
		get
		{
			return Dc2jYaj1xQ;
		}
		[CompilerGenerated]
		set
		{
			Dc2jYaj1xQ = value;
		}
	}

	public FilePreviewDataBase(PvfGroup pvf, PvfFile file, ImageSource? imageSource = null)
	{
		Pvf = pvf;
		File = file;
		if (imageSource == null)
		{
			ImageSource = AppSetting.Instance.GetRes()?.ReadNull;
		}
		else
		{
			ImageSource = imageSource;
		}
	}

	public string ValueX(string? obj, double f)
	{
		if (string.IsNullOrEmpty(obj))
		{
			return null;
		}
		if (obj.Contains("."))
		{
			return (float.Parse(obj) * (float)f).ToString("f2");
		}
		string text = ((double)int.Parse(obj) * f).ToString();
		if (text.Contains("."))
		{
			return text.ToFloat2();
		}
		return text;
	}

	public static FilePreviewDataBase? Create(PvfGroup pvf, PvfFile file, ImageSource? imagesource)
	{
		switch (file.FileType)
		{
		case PvfFileType.equ:
			if (file.FilePathHeader == "stackable")
			{
				return new StackablePreviewBase(pvf, file, imagesource);
			}
			return new FilePreviewData_Equ(pvf, file, imagesource);
		case PvfFileType.stk:
			return new StackablePreviewBase(pvf, file, imagesource);
		case PvfFileType.shp:
		{
			if (file.FilePathHeader == "itemshop" && NpcShopPreviewViewModel.Create(file, pvf, out NpcShopPreviewViewModel npcShopPreviewViewModel))
			{
				return npcShopPreviewViewModel;
			}
			break;
		}
		}
		return null;
	}
}
