using System.Runtime.CompilerServices;
using System.Windows.Media;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using Newtonsoft.Json;
using PvfCode.Models.Enums;

namespace PvfCode.Models.Options.Editor;

[JsonObject(MemberSerialization.OptOut)]
public class PvfEdiorHighlightingColorOptions : ViewModelBase
{
	[CompilerGenerated]
	private ThemeType gxyEbmKpcB;

	private SolidColorBrush? ytTEVI6rii;

	private SolidColorBrush? KhJEPcriap;

	private SolidColorBrush? vKXEFGTbsb;

	private SolidColorBrush? w3TEXrjPEQ;

	private SolidColorBrush? oR9ENxmyry;

	private SolidColorBrush? UoREikAhiw;

	private SolidColorBrush oleEMXQv8E;

	private SolidColorBrush oYrEaIrH6f;

	private SolidColorBrush vQ4EI4c9kR;

	private SolidColorBrush? ymBEUc9D1j;

	private SolidColorBrush? j2hElckS33;

	private SolidColorBrush? XSLEfJCn0U;

	private SolidColorBrush? hSKEhbH9i9;

	private SolidColorBrush? EiMETSh0dc;

	private SolidColorBrush? V2sE0XOhSC;

	private SolidColorBrush? BvpEsup9yB;

	private SolidColorBrush? YvuEQ38X2c;

	private SolidColorBrush? b2VE6nfEuM;

	private SolidColorBrush? YmQEyfaYmu;

	public ThemeType ThemeTypeChina
	{
		[CompilerGenerated]
		get
		{
			return gxyEbmKpcB;
		}
		[CompilerGenerated]
		set
		{
			gxyEbmKpcB = value;
		}
	}

	public SolidColorBrush Header
	{
		get
		{
			if (ytTEVI6rii == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					ytTEVI6rii = AppSetting.Instance.ToColor("Gray");
					break;
				case ThemeType.VS2019Dark:
					ytTEVI6rii = AppSetting.Instance.ToColor("Gray");
					break;
				case ThemeType.VS2019Light:
					ytTEVI6rii = AppSetting.Instance.ToColor("Gray");
					break;
				}
			}
			return ytTEVI6rii;
		}
		set
		{
			ytTEVI6rii = value;
			RaisePropertyChanged("Header");
		}
	}

	public SolidColorBrush Section
	{
		get
		{
			if (KhJEPcriap == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					KhJEPcriap = AppSetting.Instance.ToColor("#f92672");
					break;
				case ThemeType.VS2019Dark:
					KhJEPcriap = AppSetting.Instance.ToColor("#f92672");
					break;
				case ThemeType.VS2019Light:
					KhJEPcriap = AppSetting.Instance.ToColor("#f92672");
					break;
				}
			}
			return KhJEPcriap;
		}
		set
		{
			KhJEPcriap = value;
			RaisePropertyChanged("Section");
		}
	}

	public SolidColorBrush String
	{
		get
		{
			if (vKXEFGTbsb == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					vKXEFGTbsb = AppSetting.Instance.ToColor("#a31515");
					break;
				case ThemeType.VS2019Dark:
					vKXEFGTbsb = AppSetting.Instance.ToColor("#af7a66");
					break;
				case ThemeType.VS2019Light:
					vKXEFGTbsb = AppSetting.Instance.ToColor("#a31515");
					break;
				}
			}
			return vKXEFGTbsb;
		}
		set
		{
			vKXEFGTbsb = value;
			RaisePropertyChanged("String");
		}
	}

	public SolidColorBrush FilePath
	{
		get
		{
			if (w3TEXrjPEQ == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					w3TEXrjPEQ = AppSetting.Instance.ToColor("#a31515");
					break;
				case ThemeType.VS2019Dark:
					w3TEXrjPEQ = AppSetting.Instance.ToColor("#af7a66");
					break;
				case ThemeType.VS2019Light:
					w3TEXrjPEQ = AppSetting.Instance.ToColor("#a31515");
					break;
				}
			}
			return w3TEXrjPEQ;
		}
		set
		{
			w3TEXrjPEQ = value;
			RaisePropertyChanged("FilePath");
		}
	}

	public SolidColorBrush Digits
	{
		get
		{
			if (oR9ENxmyry == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					oR9ENxmyry = AppSetting.Instance.ToColor("#5b2da8");
					break;
				case ThemeType.VS2019Dark:
					oR9ENxmyry = AppSetting.Instance.ToColor("#b5cea8");
					break;
				case ThemeType.VS2019Light:
					oR9ENxmyry = AppSetting.Instance.ToColor("#5b2da8");
					break;
				}
			}
			return oR9ENxmyry;
		}
		set
		{
			oR9ENxmyry = value;
			RaisePropertyChanged("Digits");
		}
	}

	public SolidColorBrush Comment
	{
		get
		{
			if (UoREikAhiw == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					UoREikAhiw = AppSetting.Instance.ToColor("Green");
					break;
				case ThemeType.VS2019Dark:
					UoREikAhiw = AppSetting.Instance.ToColor("#3f9b4a");
					break;
				case ThemeType.VS2019Light:
					UoREikAhiw = AppSetting.Instance.ToColor("Green");
					break;
				}
			}
			return UoREikAhiw;
		}
		set
		{
			UoREikAhiw = value;
			RaisePropertyChanged("Comment");
		}
	}

	public SolidColorBrush KorStringMarkSymbol
	{
		get
		{
			if (oleEMXQv8E == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					oleEMXQv8E = AppSetting.Instance.ToColor("#8064A2");
					break;
				case ThemeType.VS2019Dark:
					oleEMXQv8E = AppSetting.Instance.ToColor("#85A543");
					break;
				case ThemeType.VS2019Light:
					oleEMXQv8E = AppSetting.Instance.ToColor("#8064A2");
					break;
				}
			}
			return oleEMXQv8E;
		}
		set
		{
			oleEMXQv8E = value;
			RaisePropertyChanged("KorStringMarkSymbol");
		}
	}

	public SolidColorBrush KorName
	{
		get
		{
			if (oYrEaIrH6f == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					oYrEaIrH6f = AppSetting.Instance.ToColor("#0A588C");
					break;
				case ThemeType.VS2019Dark:
					oYrEaIrH6f = AppSetting.Instance.ToColor("#C0504D");
					break;
				case ThemeType.VS2019Light:
					oYrEaIrH6f = AppSetting.Instance.ToColor("#0A588C");
					break;
				}
			}
			return oYrEaIrH6f;
		}
		set
		{
			oYrEaIrH6f = value;
			RaisePropertyChanged("KorName");
		}
	}

	public SolidColorBrush KorColon
	{
		get
		{
			if (vQ4EI4c9kR == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					vQ4EI4c9kR = AppSetting.Instance.ToColor("#FF9B98");
					break;
				case ThemeType.VS2019Dark:
					vQ4EI4c9kR = AppSetting.Instance.ToColor("#4BACC6");
					break;
				case ThemeType.VS2019Light:
					vQ4EI4c9kR = AppSetting.Instance.ToColor("#FF9B98");
					break;
				}
			}
			return vQ4EI4c9kR;
		}
		set
		{
			vQ4EI4c9kR = value;
			RaisePropertyChanged("KorColon");
		}
	}

	public SolidColorBrush Curlybraces
	{
		get
		{
			if (ymBEUc9D1j == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					ymBEUc9D1j = AppSetting.Instance.ToColor("#007acc");
					break;
				case ThemeType.VS2019Dark:
					ymBEUc9D1j = AppSetting.Instance.ToColor("#ffc813");
					break;
				case ThemeType.VS2019Light:
					ymBEUc9D1j = AppSetting.Instance.ToColor("#007acc");
					break;
				}
			}
			return ymBEUc9D1j;
		}
		set
		{
			ymBEUc9D1j = value;
			RaisePropertyChanged("Curlybraces");
		}
	}

	public SolidColorBrush Punctuation
	{
		get
		{
			if (j2hElckS33 == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					j2hElckS33 = AppSetting.Instance.ToColor("#000000");
					break;
				case ThemeType.VS2019Dark:
					j2hElckS33 = AppSetting.Instance.ToColor("#b4b4b4");
					break;
				case ThemeType.VS2019Light:
					j2hElckS33 = AppSetting.Instance.ToColor("#000000");
					break;
				}
			}
			return j2hElckS33;
		}
		set
		{
			j2hElckS33 = value;
			RaisePropertyChanged("Punctuation");
		}
	}

	public SolidColorBrush MethodCall
	{
		get
		{
			if (XSLEfJCn0U == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					XSLEfJCn0U = AppSetting.Instance.ToColor("#74531f");
					break;
				case ThemeType.VS2019Dark:
					XSLEfJCn0U = AppSetting.Instance.ToColor("#FFdcdcaa");
					break;
				case ThemeType.VS2019Light:
					XSLEfJCn0U = AppSetting.Instance.ToColor("#74531f");
					break;
				}
			}
			return XSLEfJCn0U;
		}
		set
		{
			XSLEfJCn0U = value;
			RaisePropertyChanged("MethodCall");
		}
	}

	public SolidColorBrush GGenObject
	{
		get
		{
			if (hSKEhbH9i9 == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					hSKEhbH9i9 = AppSetting.Instance.ToColor("Teal");
					break;
				case ThemeType.VS2019Dark:
					hSKEhbH9i9 = AppSetting.Instance.ToColor("#FFd8a0df");
					break;
				case ThemeType.VS2019Light:
					hSKEhbH9i9 = AppSetting.Instance.ToColor("Teal");
					break;
				}
			}
			return XSLEfJCn0U;
		}
		set
		{
			hSKEhbH9i9 = value;
			RaisePropertyChanged("GGenObject");
		}
	}

	public SolidColorBrush Keywords
	{
		get
		{
			if (EiMETSh0dc == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					EiMETSh0dc = AppSetting.Instance.ToColor("#8f08c4");
					break;
				case ThemeType.VS2019Dark:
					EiMETSh0dc = AppSetting.Instance.ToColor("#D8A0CC");
					break;
				case ThemeType.VS2019Light:
					EiMETSh0dc = AppSetting.Instance.ToColor("#8f08c4");
					break;
				}
			}
			return EiMETSh0dc;
		}
		set
		{
			EiMETSh0dc = value;
			RaisePropertyChanged("Keywords");
		}
	}

	public SolidColorBrush Keywords2
	{
		get
		{
			if (V2sE0XOhSC == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					V2sE0XOhSC = AppSetting.Instance.ToColor("#0000ff");
					break;
				case ThemeType.VS2019Dark:
					V2sE0XOhSC = AppSetting.Instance.ToColor("#569CD6");
					break;
				case ThemeType.VS2019Light:
					V2sE0XOhSC = AppSetting.Instance.ToColor("#0000ff");
					break;
				}
			}
			return V2sE0XOhSC;
		}
		set
		{
			V2sE0XOhSC = value;
			RaisePropertyChanged("Keywords2");
		}
	}

	public SolidColorBrush LstItemNameForeBrush
	{
		get
		{
			if (BvpEsup9yB == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					BvpEsup9yB = AppSetting.Instance.ToColor("Gray");
					break;
				case ThemeType.VS2019Dark:
					BvpEsup9yB = AppSetting.Instance.ToColor("Gray");
					break;
				case ThemeType.VS2019Light:
					BvpEsup9yB = AppSetting.Instance.ToColor("Gray");
					break;
				}
			}
			return BvpEsup9yB;
		}
		set
		{
			BvpEsup9yB = value;
			RaisePropertyChanged("LstItemNameForeBrush");
		}
	}

	public SolidColorBrush KorStrIndex
	{
		get
		{
			if (YvuEQ38X2c == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					KorStrIndex = AppSetting.Instance.ToColor("#f92672");
					break;
				case ThemeType.VS2019Dark:
					KorStrIndex = AppSetting.Instance.ToColor("#f92672");
					break;
				case ThemeType.VS2019Light:
					KorStrIndex = AppSetting.Instance.ToColor("#f92672");
					break;
				}
			}
			return YvuEQ38X2c;
		}
		set
		{
			YvuEQ38X2c = value;
			RaisePropertyChanged("KorStrIndex");
		}
	}

	public SolidColorBrush KorStrValue
	{
		get
		{
			if (b2VE6nfEuM == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					b2VE6nfEuM = AppSetting.Instance.ToColor("#a31515");
					break;
				case ThemeType.VS2019Dark:
					b2VE6nfEuM = AppSetting.Instance.ToColor("#af7a66");
					break;
				case ThemeType.VS2019Light:
					b2VE6nfEuM = AppSetting.Instance.ToColor("#a31515");
					break;
				}
			}
			return b2VE6nfEuM;
		}
		set
		{
			b2VE6nfEuM = value;
			RaisePropertyChanged("KorStrValue");
		}
	}

	public SolidColorBrush KorStrAngleBrackets
	{
		get
		{
			if (YmQEyfaYmu == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					YmQEyfaYmu = AppSetting.Instance.ToColor("#5b2da8");
					break;
				case ThemeType.VS2019Dark:
					YmQEyfaYmu = AppSetting.Instance.ToColor("#b5cea8");
					break;
				case ThemeType.VS2019Light:
					YmQEyfaYmu = AppSetting.Instance.ToColor("#5b2da8");
					break;
				}
			}
			return YmQEyfaYmu;
		}
		set
		{
			YmQEyfaYmu = value;
			RaisePropertyChanged("KorStrAngleBrackets");
		}
	}

	public PvfEdiorHighlightingColorOptions(ThemeType themeTypeChina)
	{
		ThemeTypeChina = themeTypeChina;
	}

	[Command]
	public void ResSet()
	{
		Header = null;
		Section = null;
		MethodCall = null;
		Comment = null;
		Curlybraces = null;
		Digits = null;
		FilePath = null;
		GGenObject = null;
		Keywords = null;
		Keywords2 = null;
		MethodCall = null;
		base.Parameter = null;
		Punctuation = null;
		String = null;
		LstItemNameForeBrush = null;
		KorStringMarkSymbol = null;
		KorName = null;
		KorColon = null;
		KorStrIndex = null;
		KorStrValue = null;
		KorStrAngleBrackets = null;
	}
}
