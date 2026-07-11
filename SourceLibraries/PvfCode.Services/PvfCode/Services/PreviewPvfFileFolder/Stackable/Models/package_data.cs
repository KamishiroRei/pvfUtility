using System.Runtime.CompilerServices;
using System.Windows.Media;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using PvfCode.LoggerBase;
using PvfCode.Models.Pvf;

namespace PvfCode.Services.PreviewPvfFileFolder.Stackable.Models;

public class package_data : ViewModelBase
{
	[CompilerGenerated]
	private int cKhMI2Etf5;

	[CompilerGenerated]
	private int KRHMe8Uh1Y;

	private readonly PvfGroup CdsMCZViOQ;

	public string ItemName
	{
		get
		{
			PvfFile file = GetFile();
			if (file == null)
			{
				return ItemCode.ToString();
			}
			return CdsMCZViOQ.GetItemName(file);
		}
	}

	public int ItemCode
	{
		[CompilerGenerated]
		get
		{
			return cKhMI2Etf5;
		}
		[CompilerGenerated]
		set
		{
			cKhMI2Etf5 = value;
		}
	}

	public int ItemCount
	{
		[CompilerGenerated]
		get
		{
			return KRHMe8Uh1Y;
		}
		[CompilerGenerated]
		set
		{
			KRHMe8Uh1Y = value;
		}
	}

	public int Rarity
	{
		get
		{
			PvfFile file = GetFile();
			if (file == null)
			{
				return 0;
			}
			file.GetRarity((PvfPack)CdsMCZViOQ, out int rarity);
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
			ImagePack2Service.Instance.TreeGetIcon(CdsMCZViOQ, file, out ImageSource imageSource);
			return FilePreviewDataBase.Create(CdsMCZViOQ, file, imageSource);
		}
	}

	public package_data(int itemCode, int itemCount, PvfGroup pvf)
	{
		ItemCode = itemCode;
		ItemCount = itemCount;
		CdsMCZViOQ = pvf;
	}

	public PvfFile? GetFile()
	{
		return CdsMCZViOQ.ListFileTable.ItemCodeConvertPvfFile(CdsMCZViOQ, ItemCode);
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
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(6, 1);
				defaultInterpolatedStringHandler.AppendLiteral("找不到文件：");
				defaultInterpolatedStringHandler.AppendFormatted(ItemCode);
				ilogger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
			}
		}
		else
		{
			ilogger?.OpenPvfFileDocument(file.FileName);
		}
	}
}
