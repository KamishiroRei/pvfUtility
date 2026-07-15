using System.Windows.Media;
using PvfCode.Models.Pvf;
using PvfCode.Models.Pvf.Enums;

namespace PvfCode.Services.GMTool.Models;

public class ItemCodePostalData
{
	public readonly PvfFile File;

	private readonly PvfGroup pvf;

	public string ItemCodeStr
	{
		get
		{
			return $"<{File.ItemCode}>";
		}
	}

	public int ItemCode
	{
		get
		{
			PvfFile file = File;
			if (file != null && file.ItemCode.HasValue)
			{
				return File.ItemCode.Value;
			}
			return -1;
		}
	}

	public string ItemName
	{
		get
		{
			string text = pvf.GetItemName(File);
			if (string.IsNullOrEmpty(text))
			{
				text = "未设定[name]";
			}
			return text;
		}
	}

	public int Rarity
	{
		get
		{
			File.GetRarity((PvfPack)pvf, out int rarity);
			return rarity;
		}
	}

	public bool IsEqu => File.FileType == PvfFileType.equ;

	public EquTypeDefault EquType => File.GetEquType(pvf);

	public ImageSource Icon
	{
		get
		{
			if (ImagePack2Service.Instance.TreeGetIcon(pvf, File, out ImageSource imageSource))
			{
				return imageSource;
			}
			return null;
		}
	}

	public ItemCodePostalData(PvfFile file, PvfGroup pack)
	{
		File = file;
		pvf = pack;
	}
}
