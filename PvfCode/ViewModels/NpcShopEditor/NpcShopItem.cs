using System.Runtime.CompilerServices;
using System.Windows.Media;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using PvfCode.LoggerBase;
using PvfCode.Models.Pvf.Enums;
using PvfCode.Services.PreviewPvfFileFolder;

namespace PvfCode.ViewModels.NpcShopEditor;

public class NpcShopItem : ViewModelBase
{
	public PvfGroup Pvf => AppCore.ViewModelBase.PVF;

	public virtual PvfFile? File
	{
		get
		{
			if (ItemCode == -1 || ItemCode == -2)
			{
				return null;
			}
			return Pvf.ListFileTable.ItemCodeConvertPvfFile(Pvf, ItemCode);
		}
	}

	public virtual int ItemCode
	{
		get
		{
			return GetProperty(() => ItemCode);
		}
		set
		{
			SetProperty(() => ItemCode, value);
			RaisePropertyChanged("ItemName");
			RaisePropertyChanged("ImageSource");
			RaisePropertyChanged("EquIsSealing");
		}
	}

	public string? ItemName
	{
		get
		{
			PvfFile file = File;
			if (file == null)
			{
				return null;
			}
			return Pvf.GetItemName(file);
		}
	}

	public virtual ImageSource? ImageSource
	{
		get
		{
			if (ItemCode == -1)
			{
				return null;
			}
			PvfFile file = File;
			if (file == null)
			{
				return Res.Instance.ReadNull;
			}
			ImagePack2Service.Instance.TreeGetIcon(Pvf, file, out ImageSource imageSource);
			if (imageSource != null)
			{
				return imageSource;
			}
			return Res.Instance.ReadNull;
		}
	}

	public bool EquIsSealing
	{
		get
		{
			PvfFile file = File;
			if (file == null || file.FileType != PvfFileType.equ)
			{
				return false;
			}
			if (file.GetAttachType(Pvf, out var attachType))
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
			PvfFile file = File;
			if (file == null)
			{
				return null;
			}
			ImagePack2Service.Instance.TreeGetIcon(Pvf, file, out ImageSource imageSource);
			return FilePreviewDataBase.Create(Pvf, file, imageSource);
		}
	}

	public NpcShopItem(int itemCode)
	{
		ItemCode = itemCode;
	}

	public NpcShopItem()
	{
		ItemCode = -1;
	}

	[Command]
	public void OnOpenFile()
	{
		if (ItemCode != -1)
		{
			Ilogger ilogger = AppSetting.Instance.GetIlogger();
			PvfFile file = File;
			if (file == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 1);
				defaultInterpolatedStringHandler.AppendLiteral("代码对应文件不存在：");
				defaultInterpolatedStringHandler.AppendFormatted(ItemCode);
				ilogger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			else
			{
				ilogger.OpenPvfFileDocument(file.FileName);
			}
		}
	}
}
