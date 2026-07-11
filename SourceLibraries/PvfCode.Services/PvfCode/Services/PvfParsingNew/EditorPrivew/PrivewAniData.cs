using PvfCode.NPK.Utils.Models;

namespace PvfCode.Services.PvfParsingNew.EditorPrivew;

public class PrivewAniData
{
	private string icon;

	public string Icon
	{
		get
		{
			if (icon != null)
			{
				return icon;
			}
			return string.Empty;
		}
		set
		{
			icon = value;
		}
	}

	public int IconIndex { get; set; }

	public int Delay { get; set; }

	public ImgFile Sprite { get; set; }

	public PrivewAniData()
	{
	}
}
