using System.Runtime.CompilerServices;
using System.Windows.Media;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using Newtonsoft.Json;
using PvfCode.Models.Enums;

namespace PvfCode.Models.Options;

[JsonObject(MemberSerialization.OptOut)]
public class PvfFilePreviewColorOptions : ViewModelBase
{
	[CompilerGenerated]
	private ThemeType eQkE875ZYx;

	private Color? QvmEuX1jCx;

	private Color? KiDE5VOuSS;

	public ThemeType ThemeTypeChina
	{
		[CompilerGenerated]
		get
		{
			return eQkE875ZYx;
		}
		[CompilerGenerated]
		set
		{
			eQkE875ZYx = value;
		}
	}

	public Color PvfFilePreviewBackBrush
	{
		get
		{
			if (!QvmEuX1jCx.HasValue)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					QvmEuX1jCx = qIFEEiaJvU("#EA000000");
					break;
				case ThemeType.VS2019Dark:
					QvmEuX1jCx = qIFEEiaJvU("#E0052236");
					break;
				case ThemeType.VS2019Light:
					QvmEuX1jCx = qIFEEiaJvU("#EA000000");
					break;
				}
			}
			return QvmEuX1jCx.Value;
		}
		set
		{
			QvmEuX1jCx = value;
			RaisePropertyChanged("PvfFilePreviewBackBrush");
		}
	}

	public Color PvfFilePreviewBorderBrush
	{
		get
		{
			if (!KiDE5VOuSS.HasValue)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					KiDE5VOuSS = qIFEEiaJvU("#5d6b99");
					break;
				case ThemeType.VS2019Dark:
					KiDE5VOuSS = qIFEEiaJvU("#007acc");
					break;
				case ThemeType.VS2019Light:
					KiDE5VOuSS = qIFEEiaJvU("#007acc");
					break;
				}
			}
			return KiDE5VOuSS.Value;
		}
		set
		{
			KiDE5VOuSS = value;
			RaisePropertyChanged("PvfFilePreviewBorderBrush");
		}
	}

	public PvfFilePreviewColorOptions(ThemeType themeTypeChina)
	{
		ThemeTypeChina = themeTypeChina;
	}

	private Color qIFEEiaJvU(string P_0)
	{
		return (Color)ColorConverter.ConvertFromString(P_0);
	}

	private byte iwREZQboiH(string P_0)
	{
		if (!byte.TryParse(P_0, out var result))
		{
			return 0;
		}
		return result;
	}

	[Command]
	public void OnRest()
	{
		QvmEuX1jCx = null;
		RaisePropertyChanged("PvfFilePreviewBackBrush");
	}
}
