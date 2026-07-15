using System.Windows.Media;

namespace PvfCode.Services.PreviewPvfFileFolder.Stackable;

public class EnchantCardInfoLevelIconInfo
{
	public ImageSource ImageSource { get; set; }

	public int Width { get; set; }

	public int Height { get; set; }

	public EnchantCardInfoLevelIconInfo(ImageSource imageSource, int width, int height)
	{
		ImageSource = imageSource;
		Width = width;
		Height = height;
	}
}
