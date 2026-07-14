using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PvfCode.Images;

public class CharacJobAvatar
{
	private BitmapSource swordman;

	private BitmapSource swordmanGray;

	private BitmapSource fighter;

	private BitmapSource fighterGray;

	private BitmapSource gunner;

	private BitmapSource gunnerGray;

	private BitmapSource mage;

	private BitmapSource mageGray;

	private BitmapSource priest;

	private BitmapSource priestGray;

	private BitmapSource atGunner;

	private BitmapSource atGunnerGray;

	private BitmapSource thief;

	private BitmapSource thiefGray;

	private BitmapSource atFighter;

	private BitmapSource atFighterGray;

	private BitmapSource atMage;

	private BitmapSource atMageGray;

	private BitmapSource demonicSwordman;

	private BitmapSource demonicSwordmanGray;

	private BitmapSource creatorMage;

	private BitmapSource creatorMageGray;

	private BitmapSource atSwordman;

	private BitmapSource atSwordmanGray;

	private BitmapSource knight;

	private BitmapSource knightGray;

	private BitmapSource demonicLancer;

	private BitmapSource demonicLancerGray;

	private BitmapSource atPriest;

	private BitmapSource atPriestGray;

	private BitmapSource gunBlader;

	private BitmapSource gunBladerGray;

	public BitmapSource Swordman
	{
		get
		{
			if (swordman == null)
			{
				swordman = LoadAvatar(0);
				((Freezable)swordman).Freeze();
			}
			return swordman;
		}
	}

	public BitmapSource SwordmanGray
	{
		get
		{
			if (swordmanGray == null)
			{
				swordmanGray = ConvertToGrayscale(Swordman);
				((Freezable)swordmanGray).Freeze();
			}
			return swordmanGray;
		}
	}

	public BitmapSource Fighter
	{
		get
		{
			if (fighter == null)
			{
				fighter = LoadAvatar(1);
				((Freezable)fighter).Freeze();
			}
			return fighter;
		}
	}

	public BitmapSource FighterGray
	{
		get
		{
			if (fighterGray == null)
			{
				fighterGray = ConvertToGrayscale(Fighter);
				((Freezable)fighterGray).Freeze();
			}
			return fighterGray;
		}
	}

	public BitmapSource Gunner
	{
		get
		{
			if (gunner == null)
			{
				gunner = LoadAvatar(2);
				((Freezable)gunner).Freeze();
			}
			return gunner;
		}
	}

	public BitmapSource GunnerGray
	{
		get
		{
			if (gunnerGray == null)
			{
				gunnerGray = ConvertToGrayscale(Gunner);
				((Freezable)gunnerGray).Freeze();
			}
			return gunnerGray;
		}
	}

	public BitmapSource Mage
	{
		get
		{
			if (mage == null)
			{
				mage = LoadAvatar(3);
				((Freezable)mage).Freeze();
			}
			return mage;
		}
	}

	public BitmapSource MageGray
	{
		get
		{
			if (mageGray == null)
			{
				mageGray = ConvertToGrayscale(Mage);
				((Freezable)mageGray).Freeze();
			}
			return mageGray;
		}
	}

	public BitmapSource Priest
	{
		get
		{
			if (priest == null)
			{
				priest = LoadAvatar(4);
				((Freezable)priest).Freeze();
			}
			return priest;
		}
	}

	public BitmapSource PriestGray
	{
		get
		{
			if (priestGray == null)
			{
				priestGray = ConvertToGrayscale(Priest);
				((Freezable)priestGray).Freeze();
			}
			return priestGray;
		}
	}

	public BitmapSource AtGunner
	{
		get
		{
			if (atGunner == null)
			{
				atGunner = LoadAvatar(5);
				((Freezable)atGunner).Freeze();
			}
			return atGunner;
		}
	}

	public BitmapSource AtGunnerGray
	{
		get
		{
			if (atGunnerGray == null)
			{
				atGunnerGray = ConvertToGrayscale(AtGunner);
				((Freezable)atGunnerGray).Freeze();
			}
			return atGunnerGray;
		}
	}

	public BitmapSource Thief
	{
		get
		{
			if (thief == null)
			{
				thief = LoadAvatar(6);
				((Freezable)thief).Freeze();
			}
			return thief;
		}
	}

	public BitmapSource ThiefGray
	{
		get
		{
			if (thiefGray == null)
			{
				thiefGray = ConvertToGrayscale(Thief);
				((Freezable)thiefGray).Freeze();
			}
			return thiefGray;
		}
	}

	public BitmapSource AtFighter
	{
		get
		{
			if (atFighter == null)
			{
				atFighter = LoadAvatar(7);
				((Freezable)atFighter).Freeze();
			}
			return atFighter;
		}
	}

	public BitmapSource AtFighterGray
	{
		get
		{
			if (atFighterGray == null)
			{
				atFighterGray = ConvertToGrayscale(AtFighter);
				((Freezable)atFighterGray).Freeze();
			}
			return atFighterGray;
		}
	}

	public BitmapSource AtMage
	{
		get
		{
			if (atMage == null)
			{
				atMage = LoadAvatar(8);
				((Freezable)atMage).Freeze();
			}
			return atMage;
		}
	}

	public BitmapSource AtMageGray
	{
		get
		{
			if (atMageGray == null)
			{
				atMageGray = ConvertToGrayscale(AtMage);
				((Freezable)atMageGray).Freeze();
			}
			return atMageGray;
		}
	}

	public BitmapSource DemonicSwordman
	{
		get
		{
			if (demonicSwordman == null)
			{
				demonicSwordman = LoadAvatar(9);
				((Freezable)demonicSwordman).Freeze();
			}
			return demonicSwordman;
		}
	}

	public BitmapSource DemonicSwordmanGray
	{
		get
		{
			if (demonicSwordmanGray == null)
			{
				demonicSwordmanGray = ConvertToGrayscale(DemonicSwordman);
				((Freezable)demonicSwordmanGray).Freeze();
			}
			return demonicSwordmanGray;
		}
	}

	public BitmapSource CreatorMage
	{
		get
		{
			if (creatorMage == null)
			{
				creatorMage = LoadAvatar(10);
				((Freezable)creatorMage).Freeze();
			}
			return creatorMage;
		}
	}

	public BitmapSource CreatorMageGray
	{
		get
		{
			if (creatorMageGray == null)
			{
				creatorMageGray = ConvertToGrayscale(CreatorMage);
				((Freezable)creatorMageGray).Freeze();
			}
			return creatorMageGray;
		}
	}

	public BitmapSource AtSwordman
	{
		get
		{
			if (atSwordman == null)
			{
				atSwordman = LoadAvatar(11);
				((Freezable)atSwordman).Freeze();
			}
			return atSwordman;
		}
	}

	public BitmapSource AtSwordmanGray
	{
		get
		{
			if (atSwordmanGray == null)
			{
				atSwordmanGray = ConvertToGrayscale(AtSwordman);
				((Freezable)atSwordmanGray).Freeze();
			}
			return atSwordmanGray;
		}
	}

	public BitmapSource Knight
	{
		get
		{
			if (knight == null)
			{
				knight = LoadAvatar(12);
				((Freezable)knight).Freeze();
			}
			return knight;
		}
	}

	public BitmapSource KnightGray
	{
		get
		{
			if (knightGray == null)
			{
				knightGray = ConvertToGrayscale(Knight);
				((Freezable)knightGray).Freeze();
			}
			return knightGray;
		}
	}

	public BitmapSource DemonicLancer
	{
		get
		{
			if (demonicLancer == null)
			{
				demonicLancer = LoadAvatar(13);
				((Freezable)demonicLancer).Freeze();
			}
			return demonicLancer;
		}
	}

	public BitmapSource DemonicLancerGray
	{
		get
		{
			if (demonicLancerGray == null)
			{
				demonicLancerGray = ConvertToGrayscale(DemonicLancer);
				((Freezable)demonicLancerGray).Freeze();
			}
			return demonicLancerGray;
		}
	}

	public BitmapSource AtPriest
	{
		get
		{
			if (atPriest == null)
			{
				atPriest = LoadAvatar(14);
				((Freezable)atPriest).Freeze();
			}
			return atPriest;
		}
	}

	public BitmapSource AtPriestGray
	{
		get
		{
			if (atPriestGray == null)
			{
				atPriestGray = ConvertToGrayscale(AtPriest);
				((Freezable)atPriestGray).Freeze();
			}
			return atPriestGray;
		}
	}

	public BitmapSource GunBlader
	{
		get
		{
			if (gunBlader == null)
			{
				gunBlader = LoadAvatar(15);
				((Freezable)gunBlader).Freeze();
			}
			return gunBlader;
		}
	}

	public BitmapSource GunBladerGray
	{
		get
		{
			if (gunBladerGray == null)
			{
				gunBladerGray = ConvertToGrayscale(GunBlader);
				((Freezable)gunBladerGray).Freeze();
			}
			return gunBladerGray;
		}
	}

	private BitmapSource LoadAvatar(int jobIndex)
	{
		return new BitmapImage(new Uri($"pack://application:,,,/pvfUtility;component/images/pngs/dnfcharacdefaulticon/{jobIndex}.png", UriKind.RelativeOrAbsolute));
	}

	private BitmapSource ConvertToGrayscale(BitmapSource source)
	{
		FormatConvertedBitmap formatConvertedBitmap = new FormatConvertedBitmap();
		formatConvertedBitmap.BeginInit();
		formatConvertedBitmap.Source = source;
		formatConvertedBitmap.DestinationFormat = PixelFormats.Gray32Float;
		formatConvertedBitmap.EndInit();
		return formatConvertedBitmap;
	}

	public CharacJobAvatar()
	{
	}
}
