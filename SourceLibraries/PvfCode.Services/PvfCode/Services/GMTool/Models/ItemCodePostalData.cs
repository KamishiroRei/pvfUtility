using System.Runtime.CompilerServices;
using System.Windows.Media;
using PvfCode.Models.Pvf;
using PvfCode.Models.Pvf.Enums;

namespace PvfCode.Services.GMTool.Models;

public class ItemCodePostalData
{
	public readonly PvfFile File;

	private readonly PvfGroup blNMpJtYMU;

	public string ItemCodeStr
	{
		get
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(2, 1);
			defaultInterpolatedStringHandler.AppendLiteral("<");
			defaultInterpolatedStringHandler.AppendFormatted(File.ItemCode);
			defaultInterpolatedStringHandler.AppendLiteral(">");
			return defaultInterpolatedStringHandler.ToStringAndClear();
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
			string text = blNMpJtYMU.GetItemName(File);
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
			File.GetRarity((PvfPack)blNMpJtYMU, out int rarity);
			return rarity;
		}
	}

	public bool IsEqu => File.FileType == PvfFileType.equ;

	public EquTypeDefault EquType => File.GetEquType(blNMpJtYMU);

	public ImageSource Icon
	{
		get
		{
			if (ImagePack2Service.Instance.TreeGetIcon(blNMpJtYMU, File, out ImageSource imageSource))
			{
				return imageSource;
			}
			return null;
		}
	}

	public ItemCodePostalData(PvfFile file, PvfGroup pack)
	{
		File = file;
		blNMpJtYMU = pack;
	}
}
