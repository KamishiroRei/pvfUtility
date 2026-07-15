using System.Windows.Media;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using PvfCode.LoggerBase;
using PvfCode.Models.Pvf;

namespace PvfCode.Services.PreviewPvfFileFolder.Stackable.Models;

public class package_data : ViewModelBase
{
	private readonly PvfGroup pvf;

	public string ItemName
	{
		get
		{
			PvfFile file = GetFile();
			if (file == null)
			{
				return ItemCode.ToString();
			}
			return pvf.GetItemName(file);
		}
	}

	public int ItemCode { get; set; }

	public int ItemCount { get; set; }

	public int Rarity
	{
		get
		{
			PvfFile file = GetFile();
			if (file == null)
			{
				return 0;
			}
			file.GetRarity((PvfPack)pvf, out int rarity);
			return rarity;
		}
	}

	public FilePreviewDataBase? PreviewBase
	{
		get
		{
			PvfFile file = GetFile();
			if (file == null)
			{
				return null;
			}
			ImagePack2Service.Instance.TreeGetIcon(pvf, file, out ImageSource imageSource);
			return FilePreviewDataBase.Create(pvf, file, imageSource);
		}
	}

	public package_data(int itemCode, int itemCount, PvfGroup pvf)
	{
		ItemCode = itemCode;
		ItemCount = itemCount;
		this.pvf = pvf;
	}

	public PvfFile? GetFile()
	{
		return pvf.ListFileTable.ItemCodeConvertPvfFile(pvf, ItemCode);
	}

	[Command]
	public void OnOpenFile()
	{
		PvfFile file = GetFile();
		Ilogger ilogger = AppSetting.Instance.GetIlogger();
		if (file == null)
		{
			if (ilogger != null)
			{
				ilogger.Error($"找不到文件：{ItemCode}");
			}
		}
		else
		{
			ilogger?.OpenPvfFileDocument(file.FileName);
		}
	}
}
