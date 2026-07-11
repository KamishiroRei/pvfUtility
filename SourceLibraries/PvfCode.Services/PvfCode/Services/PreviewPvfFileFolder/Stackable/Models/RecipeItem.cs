using System.Runtime.CompilerServices;
using System.Windows.Media;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using PvfCode.LoggerBase;

namespace PvfCode.Services.PreviewPvfFileFolder.Stackable.Models;

public class RecipeItem : ViewModelBase
{
	private readonly PvfGroup wZVjZOkCjs;

	[CompilerGenerated]
	private int K6djhPSsIJ;

	[CompilerGenerated]
	private int iSbjdD98aY;

	public int ItemCode
	{
		[CompilerGenerated]
		get
		{
			return K6djhPSsIJ;
		}
		[CompilerGenerated]
		set
		{
			K6djhPSsIJ = value;
		}
	}

	public int NeedCount
	{
		[CompilerGenerated]
		get
		{
			return iSbjdD98aY;
		}
		[CompilerGenerated]
		set
		{
			iSbjdD98aY = value;
		}
	}

	public string? ItemName
	{
		get
		{
			PvfFile file = GetFile();
			if (file == null)
			{
				return null;
			}
			string itemName = wZVjZOkCjs.GetItemName(file);
			if (string.IsNullOrEmpty(itemName))
			{
				return ItemCode.ToString();
			}
			return itemName.Replace("\\n", "");
		}
	}

	public string NeedCountString
	{
		get
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(2, 1);
			defaultInterpolatedStringHandler.AppendLiteral("0/");
			defaultInterpolatedStringHandler.AppendFormatted(NeedCount);
			return defaultInterpolatedStringHandler.ToStringAndClear();
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
			ImagePack2Service.Instance.TreeGetIcon(wZVjZOkCjs, file, out ImageSource imageSource);
			return FilePreviewDataBase.Create(wZVjZOkCjs, file, imageSource);
		}
	}

	public RecipeItem(int itemCode, int needCount, PvfGroup pvf)
	{
		ItemCode = itemCode;
		NeedCount = needCount;
		wZVjZOkCjs = pvf;
	}

	public PvfFile? GetFile()
	{
		string text = wZVjZOkCjs.ListFileTable.ItemCodeConvertFilePath(ItemCode);
		if (text == null)
		{
			return null;
		}
		if (wZVjZOkCjs.FileList.TryGetValue(text, out PvfFile value))
		{
			return value;
		}
		return null;
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
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(12, 1);
				defaultInterpolatedStringHandler.AppendLiteral("找不到对应的文件 代码：");
				defaultInterpolatedStringHandler.AppendFormatted(ItemCode);
				ilogger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
			}
		}
		else
		{
			ilogger.OpenPvfFileDocument(file.FileName);
		}
	}
}
