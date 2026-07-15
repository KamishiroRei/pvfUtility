using System.Windows.Media;
using DevExpress.Mvvm;
using PvfCode.Services.PreviewPvfFileFolder.NpcShop;
using PvfCode.Services.PreviewPvfFileFolder.Stackable;
using Utools;

namespace PvfCode.Services.PreviewPvfFileFolder;

public abstract class FilePreviewDataBase : ViewModelBase
{
	public PvfFile File { get; set; }

	public PvfGroup Pvf { get; set; }

	public ImageSource ImageSource { get; set; }

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
