using System.Runtime.CompilerServices;
using System.Windows.Media;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using PvfCode.LoggerBase;
using PvfCode.Models.Pvf.Enums;

namespace PvfCode.Services.PreviewPvfFileFolder.NpcShop;

public class NpcShopPreviewItem : ViewModelBase
{
	private readonly PvfGroup by7M1KMUtt;

	private PvfFile? BTWMP1YHZr
	{
		get
		{
			if (ItemCode == -1 || ItemCode == -2)
			{
				return null;
			}
			return by7M1KMUtt.ListFileTable.ItemCodeConvertPvfFile(by7M1KMUtt, ItemCode);
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
			PvfFile pvfFile = BTWMP1YHZr;
			if (pvfFile == null)
			{
				return null;
			}
			ImagePack2Service.Instance.TreeGetIcon(by7M1KMUtt, pvfFile, out ImageSource imageSource);
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
			PvfFile pvfFile = BTWMP1YHZr;
			if (pvfFile == null || pvfFile.FileType != PvfFileType.equ)
			{
				return false;
			}
			if (pvfFile.GetAttachType(by7M1KMUtt, out var attachType))
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
			PvfFile pvfFile = BTWMP1YHZr;
			if (pvfFile == null)
			{
				return null;
			}
			ImagePack2Service.Instance.TreeGetIcon(by7M1KMUtt, pvfFile, out ImageSource imageSource);
			return FilePreviewDataBase.Create(by7M1KMUtt, pvfFile, imageSource);
		}
	}

	public NpcShopPreviewItem(int itemCode, PvfGroup pvf)
	{
		ItemCode = itemCode;
		by7M1KMUtt = pvf;
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
			PvfFile pvfFile = BTWMP1YHZr;
			if (pvfFile == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 1);
				defaultInterpolatedStringHandler.AppendLiteral("代码对应文件不存在：");
				defaultInterpolatedStringHandler.AppendFormatted(ItemCode);
				ilogger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			else
			{
				ilogger.OpenPvfFileDocument(pvfFile.FileName);
			}
		}
	}
}
