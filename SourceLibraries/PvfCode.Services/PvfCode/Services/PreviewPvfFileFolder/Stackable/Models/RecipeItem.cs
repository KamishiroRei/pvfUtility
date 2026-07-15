using System.Windows.Media;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using PvfCode.LoggerBase;

namespace PvfCode.Services.PreviewPvfFileFolder.Stackable.Models;

public class RecipeItem : ViewModelBase
{
	private readonly PvfGroup pvf;

	public int ItemCode { get; set; }

	public int NeedCount { get; set; }

	public string? ItemName
	{
		get
		{
			PvfFile file = GetFile();
			if (file == null)
			{
				return null;
			}
			string itemName = pvf.GetItemName(file);
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
			return $"0/{NeedCount}";
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

	public RecipeItem(int itemCode, int needCount, PvfGroup pvf)
	{
		ItemCode = itemCode;
		NeedCount = needCount;
		this.pvf = pvf;
	}

	public PvfFile? GetFile()
	{
		string text = pvf.ListFileTable.ItemCodeConvertFilePath(ItemCode);
		if (text == null)
		{
			return null;
		}
		if (pvf.FileList.TryGetValue(text, out PvfFile value))
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
				ilogger.Error($"找不到对应的文件 代码：{ItemCode}");
			}
		}
		else
		{
			ilogger.OpenPvfFileDocument(file.FileName);
		}
	}
}
