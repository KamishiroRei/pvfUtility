using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Mvvm;
using Newtonsoft.Json;
using PvfCode.Models.Enums;

namespace PvfCode;

[JsonObject(MemberSerialization.OptOut)]
public class TreeColorConfig : BindableBase
{
	[CompilerGenerated]
	private ThemeType uDVnCK1G1k;

	private SolidColorBrush wdWnvvQNiH;

	private SolidColorBrush JmynbrdbTl;

	private SolidColorBrush wJJnVv9NkF;

	private SolidColorBrush bvLnPNSinP;

	private SolidColorBrush PwJnFa0f0u;

	private SolidColorBrush? EGinXBiVcI;

	private SolidColorBrush? RFynNaTCcV;

	private SolidColorBrush? mHnniCelaw;

	private SolidColorBrush? JXcnMMHMhH;

	private SolidColorBrush? sUBnavuj1T;

	private SolidColorBrush? C4XnIIBx8D;

	private SolidColorBrush? ei2nUg858t;

	private SolidColorBrush? cGFnlYTjft;

	private ICommand mUenfE5FVi;

	public ThemeType ThemeTypeChina
	{
		[CompilerGenerated]
		get
		{
			return uDVnCK1G1k;
		}
		[CompilerGenerated]
		set
		{
			uDVnCK1G1k = value;
		}
	}

	public SolidColorBrush NewFileFlagBrush
	{
		get
		{
			if (wdWnvvQNiH == null)
			{
				wdWnvvQNiH = ko2n4sYnoE("Red");
			}
			return wdWnvvQNiH;
		}
		set
		{
			wdWnvvQNiH = value;
			RaisePropertyChanged("NewFileFlagBrush");
		}
	}

	public SolidColorBrush FileNameBrush
	{
		get
		{
			if (JmynbrdbTl == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					JmynbrdbTl = ko2n4sYnoE("black");
					break;
				case ThemeType.VS2019Dark:
					JmynbrdbTl = ko2n4sYnoE("White");
					break;
				case ThemeType.VS2019Light:
					JmynbrdbTl = ko2n4sYnoE("black");
					break;
				}
			}
			return JmynbrdbTl;
		}
		set
		{
			JmynbrdbTl = value;
			RaisePropertyChanged("FileNameBrush");
		}
	}

	public SolidColorBrush ItemNameBrush
	{
		get
		{
			if (wJJnVv9NkF == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					wJJnVv9NkF = ko2n4sYnoE("#a31515");
					break;
				case ThemeType.VS2019Dark:
					wJJnVv9NkF = ko2n4sYnoE("#af7a66");
					break;
				case ThemeType.VS2019Light:
					wJJnVv9NkF = ko2n4sYnoE("#a31515");
					break;
				}
			}
			return wJJnVv9NkF;
		}
		set
		{
			wJJnVv9NkF = value;
			RaisePropertyChanged("ItemNameBrush");
		}
	}

	public SolidColorBrush ItemCodeBrush
	{
		get
		{
			if (bvLnPNSinP == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					bvLnPNSinP = ko2n4sYnoE("#5b2da8");
					break;
				case ThemeType.VS2019Dark:
					bvLnPNSinP = ko2n4sYnoE("#b5cea8");
					break;
				case ThemeType.VS2019Light:
					bvLnPNSinP = ko2n4sYnoE("#5b2da8");
					break;
				}
			}
			return bvLnPNSinP;
		}
		set
		{
			bvLnPNSinP = value;
			RaisePropertyChanged("ItemCodeBrush");
		}
	}

	public SolidColorBrush CommentBrush
	{
		get
		{
			if (PwJnFa0f0u == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					PwJnFa0f0u = ko2n4sYnoE("gray");
					break;
				case ThemeType.VS2019Dark:
					PwJnFa0f0u = ko2n4sYnoE("gray");
					break;
				case ThemeType.VS2019Light:
					PwJnFa0f0u = ko2n4sYnoE("gray");
					break;
				}
			}
			return PwJnFa0f0u;
		}
		set
		{
			PwJnFa0f0u = value;
			RaisePropertyChanged("CommentBrush");
		}
	}

	public SolidColorBrush SelectedBackBrush
	{
		get
		{
			if (EGinXBiVcI == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					EGinXBiVcI = ko2n4sYnoE("#d8daeb");
					break;
				case ThemeType.VS2019Dark:
					EGinXBiVcI = ko2n4sYnoE("#404040");
					break;
				case ThemeType.VS2019Light:
					EGinXBiVcI = ko2n4sYnoE("#c9def5");
					break;
				}
			}
			return EGinXBiVcI;
		}
		set
		{
			EGinXBiVcI = value;
			RaisePropertyChanged("SelectedBackBrush");
		}
	}

	public SolidColorBrush FocusedBackBrush
	{
		get
		{
			if (RFynNaTCcV == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					RFynNaTCcV = ko2n4sYnoE("#d8daeb");
					break;
				case ThemeType.VS2019Dark:
					RFynNaTCcV = ko2n4sYnoE("#404040");
					break;
				case ThemeType.VS2019Light:
					RFynNaTCcV = ko2n4sYnoE("#c9def5");
					break;
				}
			}
			return RFynNaTCcV;
		}
		set
		{
			RFynNaTCcV = value;
			RaisePropertyChanged("FocusedBackBrush");
		}
	}

	public SolidColorBrush RarityColor0
	{
		get
		{
			if (mHnniCelaw == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					mHnniCelaw = FileNameBrush;
					break;
				case ThemeType.VS2019Dark:
					mHnniCelaw = FileNameBrush;
					break;
				case ThemeType.VS2019Light:
					mHnniCelaw = FileNameBrush;
					break;
				}
			}
			return mHnniCelaw;
		}
		set
		{
			mHnniCelaw = value;
			RaisePropertyChanged("RarityColor0");
		}
	}

	public SolidColorBrush RarityColor1
	{
		get
		{
			if (JXcnMMHMhH == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					JXcnMMHMhH = ko2n4sYnoE("#68D5ED");
					break;
				case ThemeType.VS2019Dark:
					JXcnMMHMhH = ko2n4sYnoE("#68D5ED");
					break;
				case ThemeType.VS2019Light:
					JXcnMMHMhH = ko2n4sYnoE("#68D5ED");
					break;
				}
			}
			return JXcnMMHMhH;
		}
		set
		{
			JXcnMMHMhH = value;
			RaisePropertyChanged("RarityColor1");
		}
	}

	public SolidColorBrush RarityColor2
	{
		get
		{
			if (sUBnavuj1T == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					sUBnavuj1T = ko2n4sYnoE("#B36BFF");
					break;
				case ThemeType.VS2019Dark:
					sUBnavuj1T = ko2n4sYnoE("#B36BFF");
					break;
				case ThemeType.VS2019Light:
					sUBnavuj1T = ko2n4sYnoE("#B36BFF");
					break;
				}
			}
			return sUBnavuj1T;
		}
		set
		{
			sUBnavuj1T = value;
			RaisePropertyChanged("RarityColor2");
		}
	}

	public SolidColorBrush RarityColor3
	{
		get
		{
			if (C4XnIIBx8D == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					C4XnIIBx8D = ko2n4sYnoE("#DC007E");
					break;
				case ThemeType.VS2019Dark:
					C4XnIIBx8D = ko2n4sYnoE("#DC007E");
					break;
				case ThemeType.VS2019Light:
					C4XnIIBx8D = ko2n4sYnoE("#DC007E");
					break;
				}
			}
			return C4XnIIBx8D;
		}
		set
		{
			C4XnIIBx8D = value;
			RaisePropertyChanged("RarityColor3");
		}
	}

	public SolidColorBrush RarityColor4
	{
		get
		{
			if (ei2nUg858t == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					ei2nUg858t = ko2n4sYnoE("#FFB100");
					break;
				case ThemeType.VS2019Dark:
					ei2nUg858t = ko2n4sYnoE("#FFB100");
					break;
				case ThemeType.VS2019Light:
					ei2nUg858t = ko2n4sYnoE("#FFB100");
					break;
				}
			}
			return ei2nUg858t;
		}
		set
		{
			ei2nUg858t = value;
			RaisePropertyChanged("RarityColor4");
		}
	}

	public SolidColorBrush RarityColor5
	{
		get
		{
			if (cGFnlYTjft == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					cGFnlYTjft = ko2n4sYnoE("#FF6666");
					break;
				case ThemeType.VS2019Dark:
					cGFnlYTjft = ko2n4sYnoE("#FF6666");
					break;
				case ThemeType.VS2019Light:
					cGFnlYTjft = ko2n4sYnoE("#FF6666");
					break;
				}
			}
			return cGFnlYTjft;
		}
		set
		{
			cGFnlYTjft = value;
			RaisePropertyChanged("RarityColor5");
		}
	}

	[JsonIgnore]
	public ICommand ResetCommand
	{
		get
		{
			if (mUenfE5FVi == null)
			{
				mUenfE5FVi = new DelegateCommand(Reset);
			}
			return mUenfE5FVi;
		}
	}

	public TreeColorConfig(ThemeType themeTypeChina)
	{
		ThemeTypeChina = themeTypeChina;
	}

	private SolidColorBrush ko2n4sYnoE(string P_0)
	{
		return new SolidColorBrush((Color)ColorConverter.ConvertFromString(P_0));
	}

	public void Reset()
	{
		NewFileFlagBrush = null;
		FileNameBrush = null;
		ItemNameBrush = null;
		ItemCodeBrush = null;
		CommentBrush = null;
		FocusedBackBrush = null;
		SelectedBackBrush = null;
		RarityColor0 = null;
		RarityColor1 = null;
		RarityColor2 = null;
		RarityColor3 = null;
		RarityColor4 = null;
		RarityColor5 = null;
	}
}
