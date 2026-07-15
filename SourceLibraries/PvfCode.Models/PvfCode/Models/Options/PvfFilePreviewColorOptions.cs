using System.Windows.Media;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using Newtonsoft.Json;
using PvfCode.Models.Enums;

namespace PvfCode.Models.Options;

[JsonObject(MemberSerialization.OptOut)]
public class PvfFilePreviewColorOptions : ViewModelBase
{
	private Color? previewBackgroundColor;

	private Color? previewBorderColor;

	public ThemeType ThemeTypeChina { get; set; }

	public Color PvfFilePreviewBackBrush
	{
		get
		{
			if (!previewBackgroundColor.HasValue)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					previewBackgroundColor = ParseColor("#EA000000");
					break;
				case ThemeType.VS2019Dark:
					previewBackgroundColor = ParseColor("#E0052236");
					break;
				case ThemeType.VS2019Light:
					previewBackgroundColor = ParseColor("#EA000000");
					break;
				}
			}
			return previewBackgroundColor.Value;
		}
		set
		{
			previewBackgroundColor = value;
			RaisePropertyChanged(nameof(PvfFilePreviewBackBrush));
		}
	}

	public Color PvfFilePreviewBorderBrush
	{
		get
		{
			if (!previewBorderColor.HasValue)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					previewBorderColor = ParseColor("#5d6b99");
					break;
				case ThemeType.VS2019Dark:
					previewBorderColor = ParseColor("#007acc");
					break;
				case ThemeType.VS2019Light:
					previewBorderColor = ParseColor("#007acc");
					break;
				}
			}
			return previewBorderColor.Value;
		}
		set
		{
			previewBorderColor = value;
			RaisePropertyChanged(nameof(PvfFilePreviewBorderBrush));
		}
	}

	public PvfFilePreviewColorOptions(ThemeType themeTypeChina)
	{
		ThemeTypeChina = themeTypeChina;
	}

	private static Color ParseColor(string value)
	{
		return (Color)ColorConverter.ConvertFromString(value);
	}

	[Command]
	public void OnRest()
	{
		previewBackgroundColor = null;
		RaisePropertyChanged(nameof(PvfFilePreviewBackBrush));
	}
}
