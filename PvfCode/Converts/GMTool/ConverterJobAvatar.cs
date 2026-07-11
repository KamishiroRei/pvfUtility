using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using GMTool.Dot;

namespace PvfCode.Converts.GMTool;

public class ConverterJobAvatar : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}
		ImageSource result = null;
		if (value is CharacInfoDto characInfoDto)
		{
			switch (characInfoDto.job)
			{
			case 0:
				result = (characInfoDto.CharacOnLine ? Res.Instance.CharacJobAvatar.Swordman : Res.Instance.CharacJobAvatar.SwordmanGray);
				break;
			case 1:
				result = (characInfoDto.CharacOnLine ? Res.Instance.CharacJobAvatar.Fighter : Res.Instance.CharacJobAvatar.FighterGray);
				break;
			case 2:
				result = (characInfoDto.CharacOnLine ? Res.Instance.CharacJobAvatar.Gunner : Res.Instance.CharacJobAvatar.GunnerGray);
				break;
			case 3:
				result = (characInfoDto.CharacOnLine ? Res.Instance.CharacJobAvatar.Mage : Res.Instance.CharacJobAvatar.MageGray);
				break;
			case 4:
				result = (characInfoDto.CharacOnLine ? Res.Instance.CharacJobAvatar.Priest : Res.Instance.CharacJobAvatar.PriestGray);
				break;
			case 5:
				result = (characInfoDto.CharacOnLine ? Res.Instance.CharacJobAvatar.AtGunner : Res.Instance.CharacJobAvatar.AtGunnerGray);
				break;
			case 6:
				result = (characInfoDto.CharacOnLine ? Res.Instance.CharacJobAvatar.Thief : Res.Instance.CharacJobAvatar.ThiefGray);
				break;
			case 7:
				result = (characInfoDto.CharacOnLine ? Res.Instance.CharacJobAvatar.AtFighter : Res.Instance.CharacJobAvatar.AtFighterGray);
				break;
			case 8:
				result = (characInfoDto.CharacOnLine ? Res.Instance.CharacJobAvatar.AtMage : Res.Instance.CharacJobAvatar.AtMageGray);
				break;
			case 9:
				result = (characInfoDto.CharacOnLine ? Res.Instance.CharacJobAvatar.DemonicSwordman : Res.Instance.CharacJobAvatar.DemonicSwordmanGray);
				break;
			case 10:
				result = (characInfoDto.CharacOnLine ? Res.Instance.CharacJobAvatar.CreatorMage : Res.Instance.CharacJobAvatar.CreatorMageGray);
				break;
			case 11:
				result = (characInfoDto.CharacOnLine ? Res.Instance.CharacJobAvatar.AtSwordman : Res.Instance.CharacJobAvatar.AtSwordmanGray);
				break;
			case 12:
				result = (characInfoDto.CharacOnLine ? Res.Instance.CharacJobAvatar.Knight : Res.Instance.CharacJobAvatar.KnightGray);
				break;
			case 13:
				result = (characInfoDto.CharacOnLine ? Res.Instance.CharacJobAvatar.DemonicLancer : Res.Instance.CharacJobAvatar.DemonicLancerGray);
				break;
			case 14:
				result = (characInfoDto.CharacOnLine ? Res.Instance.CharacJobAvatar.AtPriest : Res.Instance.CharacJobAvatar.AtPriestGray);
				break;
			case 15:
				result = (characInfoDto.CharacOnLine ? Res.Instance.CharacJobAvatar.GunBlader : Res.Instance.CharacJobAvatar.GunBladerGray);
				break;
			}
		}
		return result;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}

	public ConverterJobAvatar()
	{
	}
}
