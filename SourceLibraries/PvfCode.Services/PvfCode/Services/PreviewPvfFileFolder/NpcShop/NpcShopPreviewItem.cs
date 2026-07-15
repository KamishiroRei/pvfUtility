using System.Windows.Media;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using PvfCode.LoggerBase;
using PvfCode.Models.Pvf.Enums;

namespace PvfCode.Services.PreviewPvfFileFolder.NpcShop;

public class NpcShopPreviewItem : ViewModelBase
{
	private readonly PvfGroup pvf;

	private PvfFile? File
	{
		get
		{
			if (ItemCode == -1 || ItemCode == -2)
			{
				return null;
			}
			return pvf.ListFileTable.ItemCodeConvertPvfFile(pvf, ItemCode);
		}
	}

	public int ItemCode
	{
		get
		{
			return GetProperty(() => ItemCode);
		}
		set
		{
			SetProperty(() => ItemCode, value);
		}
	}

	public ImageSource ImageSource
	{
		get
		{
			PvfFile pvfFile = File;
			if (pvfFile == null)
			{
				return null;
			}
			ImagePack2Service.Instance.TreeGetIcon(pvf, pvfFile, out ImageSource imageSource);
			if (imageSource != null)
			{
				return imageSource;
			}
			return AppSetting.Instance.GetRes().ReadNull;
		}
	}

	public bool EquIsSealing
	{
		get
		{
			PvfFile pvfFile = File;
			if (pvfFile == null || pvfFile.FileType != PvfFileType.equ)
			{
				return false;
			}
			if (pvfFile.GetAttachType(pvf, out var attachType))
			{
				return attachType == AttachType.sealing;
			}
			return false;
		}
	}

	public bool IsNull
	{
		get
		{
			if (ItemCode != -1)
			{
				return ItemCode == -2;
			}
			return true;
		}
	}

	public FilePreviewDataBase? PreviewBase
	{
		get
		{
			if (IsNull)
			{
				return null;
			}
			PvfFile pvfFile = File;
			if (pvfFile == null)
			{
				return null;
			}
			ImagePack2Service.Instance.TreeGetIcon(pvf, pvfFile, out ImageSource imageSource);
			return FilePreviewDataBase.Create(pvf, pvfFile, imageSource);
		}
	}

	public NpcShopPreviewItem(int itemCode, PvfGroup pvf)
	{
		ItemCode = itemCode;
		this.pvf = pvf;
	}

	public NpcShopPreviewItem()
	{
		ItemCode = -1;
	}

	[Command]
	public void OnOpenFile()
	{
		if (ItemCode != -1)
		{
			Ilogger ilogger = AppSetting.Instance.GetIlogger();
			PvfFile pvfFile = File;
			if (pvfFile == null)
			{
				ilogger.Error($"代码对应文件不存在：{ItemCode}");
			}
			else
			{
				ilogger.OpenPvfFileDocument(pvfFile.FileName);
			}
		}
	}
}
