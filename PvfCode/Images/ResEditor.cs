using System.Windows;
using System.Windows.Media;

namespace PvfCode.Images;

public class ResEditor
{
	private ImageSource Lidajdf3Ud;

	private ImageSource qRkaTpDpPL;

	private ImageSource CZ7aCw9MVL;

	private ImageSource Cc6aHUMKrL;

	private ImageSource r4mahFiYAl;

	public ImageSource SaveAll_16x
	{
		get
		{
			if (Lidajdf3Ud == null)
			{
				Lidajdf3Ud = Res.Instance.GetSvgImage("pack://application:,,,/pvfUtility;component/images/svgs/edit/saveall_16x.svg");
			}
			return Lidajdf3Ud;
		}
	}

	public ImageSource SaveAll_16x_2
	{
		get
		{
			if (qRkaTpDpPL == null)
			{
				qRkaTpDpPL = Res.Instance.GetSvgImage("pack://application:,,,/pvfUtility;component/images/svgs/edit/saveall_16x_2.svg");
			}
			return qRkaTpDpPL;
		}
	}

	public ImageSource SaveFileDialogControl_16x
	{
		get
		{
			if (CZ7aCw9MVL == null)
			{
				CZ7aCw9MVL = Res.Instance.GetSvgImage("pack://application:,,,/pvfUtility;component/images/svgs/edit/savefiledialogcontrol_16x.svg");
			}
			return CZ7aCw9MVL;
		}
	}

	public ImageSource SaveFileDialogControl_16x_2
	{
		get
		{
			if (Cc6aHUMKrL == null)
			{
				Cc6aHUMKrL = Res.Instance.GetSvgImage("pack://application:,,,/pvfUtility;component/images/svgs/edit/savefiledialogcontrol_16x_2.svg");
			}
			return Cc6aHUMKrL;
		}
	}

	public ImageSource HighImportance
	{
		get
		{
			if (r4mahFiYAl == null)
			{
				r4mahFiYAl = Res.Instance.GetSvgImage("pack://application:,,,/pvfUtility;component/images/svgs/edit/highimportance.svg");
				((Freezable)r4mahFiYAl).Freeze();
			}
			return r4mahFiYAl;
		}
	}

	public ResEditor()
	{
	}
}
