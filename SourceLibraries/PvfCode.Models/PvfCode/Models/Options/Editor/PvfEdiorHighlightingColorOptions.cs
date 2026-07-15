using System.Windows.Media;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using Newtonsoft.Json;
using PvfCode.Models.Enums;

namespace PvfCode.Models.Options.Editor;

[JsonObject(MemberSerialization.OptOut)]
public class PvfEdiorHighlightingColorOptions : ViewModelBase
{
	private SolidColorBrush? header;

	private SolidColorBrush? section;

	private SolidColorBrush? stringBrush;

	private SolidColorBrush? filePath;

	private SolidColorBrush? digits;

	private SolidColorBrush? comment;

	private SolidColorBrush korStringMarkSymbol;

	private SolidColorBrush korName;

	private SolidColorBrush korColon;

	private SolidColorBrush? curlybraces;

	private SolidColorBrush? punctuation;

	private SolidColorBrush? methodCall;

	private SolidColorBrush? genObject;

	private SolidColorBrush? keywords;

	private SolidColorBrush? keywords2;

	private SolidColorBrush? lstItemNameForeBrush;

	private SolidColorBrush? korStrIndex;

	private SolidColorBrush? korStrValue;

	private SolidColorBrush? korStrAngleBrackets;

	public ThemeType ThemeTypeChina { get; set; }

	public SolidColorBrush Header
	{
		get
		{
			if (header == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					header = AppSetting.Instance.ToColor("Gray");
					break;
				case ThemeType.VS2019Dark:
					header = AppSetting.Instance.ToColor("Gray");
					break;
				case ThemeType.VS2019Light:
					header = AppSetting.Instance.ToColor("Gray");
					break;
				}
			}
			return header;
		}
		set
		{
			header = value;
			RaisePropertyChanged("Header");
		}
	}

	public SolidColorBrush Section
	{
		get
		{
			if (section == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					section = AppSetting.Instance.ToColor("#f92672");
					break;
				case ThemeType.VS2019Dark:
					section = AppSetting.Instance.ToColor("#f92672");
					break;
				case ThemeType.VS2019Light:
					section = AppSetting.Instance.ToColor("#f92672");
					break;
				}
			}
			return section;
		}
		set
		{
			section = value;
			RaisePropertyChanged("Section");
		}
	}

	public SolidColorBrush String
	{
		get
		{
			if (stringBrush == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					stringBrush = AppSetting.Instance.ToColor("#a31515");
					break;
				case ThemeType.VS2019Dark:
					stringBrush = AppSetting.Instance.ToColor("#af7a66");
					break;
				case ThemeType.VS2019Light:
					stringBrush = AppSetting.Instance.ToColor("#a31515");
					break;
				}
			}
			return stringBrush;
		}
		set
		{
			stringBrush = value;
			RaisePropertyChanged("String");
		}
	}

	public SolidColorBrush FilePath
	{
		get
		{
			if (filePath == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					filePath = AppSetting.Instance.ToColor("#a31515");
					break;
				case ThemeType.VS2019Dark:
					filePath = AppSetting.Instance.ToColor("#af7a66");
					break;
				case ThemeType.VS2019Light:
					filePath = AppSetting.Instance.ToColor("#a31515");
					break;
				}
			}
			return filePath;
		}
		set
		{
			filePath = value;
			RaisePropertyChanged("FilePath");
		}
	}

	public SolidColorBrush Digits
	{
		get
		{
			if (digits == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					digits = AppSetting.Instance.ToColor("#5b2da8");
					break;
				case ThemeType.VS2019Dark:
					digits = AppSetting.Instance.ToColor("#b5cea8");
					break;
				case ThemeType.VS2019Light:
					digits = AppSetting.Instance.ToColor("#5b2da8");
					break;
				}
			}
			return digits;
		}
		set
		{
			digits = value;
			RaisePropertyChanged("Digits");
		}
	}

	public SolidColorBrush Comment
	{
		get
		{
			if (comment == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					comment = AppSetting.Instance.ToColor("Green");
					break;
				case ThemeType.VS2019Dark:
					comment = AppSetting.Instance.ToColor("#3f9b4a");
					break;
				case ThemeType.VS2019Light:
					comment = AppSetting.Instance.ToColor("Green");
					break;
				}
			}
			return comment;
		}
		set
		{
			comment = value;
			RaisePropertyChanged("Comment");
		}
	}

	public SolidColorBrush KorStringMarkSymbol
	{
		get
		{
			if (korStringMarkSymbol == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					korStringMarkSymbol = AppSetting.Instance.ToColor("#8064A2");
					break;
				case ThemeType.VS2019Dark:
					korStringMarkSymbol = AppSetting.Instance.ToColor("#85A543");
					break;
				case ThemeType.VS2019Light:
					korStringMarkSymbol = AppSetting.Instance.ToColor("#8064A2");
					break;
				}
			}
			return korStringMarkSymbol;
		}
		set
		{
			korStringMarkSymbol = value;
			RaisePropertyChanged("KorStringMarkSymbol");
		}
	}

	public SolidColorBrush KorName
	{
		get
		{
			if (korName == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					korName = AppSetting.Instance.ToColor("#0A588C");
					break;
				case ThemeType.VS2019Dark:
					korName = AppSetting.Instance.ToColor("#C0504D");
					break;
				case ThemeType.VS2019Light:
					korName = AppSetting.Instance.ToColor("#0A588C");
					break;
				}
			}
			return korName;
		}
		set
		{
			korName = value;
			RaisePropertyChanged("KorName");
		}
	}

	public SolidColorBrush KorColon
	{
		get
		{
			if (korColon == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					korColon = AppSetting.Instance.ToColor("#FF9B98");
					break;
				case ThemeType.VS2019Dark:
					korColon = AppSetting.Instance.ToColor("#4BACC6");
					break;
				case ThemeType.VS2019Light:
					korColon = AppSetting.Instance.ToColor("#FF9B98");
					break;
				}
			}
			return korColon;
		}
		set
		{
			korColon = value;
			RaisePropertyChanged("KorColon");
		}
	}

	public SolidColorBrush Curlybraces
	{
		get
		{
			if (curlybraces == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					curlybraces = AppSetting.Instance.ToColor("#007acc");
					break;
				case ThemeType.VS2019Dark:
					curlybraces = AppSetting.Instance.ToColor("#ffc813");
					break;
				case ThemeType.VS2019Light:
					curlybraces = AppSetting.Instance.ToColor("#007acc");
					break;
				}
			}
			return curlybraces;
		}
		set
		{
			curlybraces = value;
			RaisePropertyChanged("Curlybraces");
		}
	}

	public SolidColorBrush Punctuation
	{
		get
		{
			if (punctuation == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					punctuation = AppSetting.Instance.ToColor("#000000");
					break;
				case ThemeType.VS2019Dark:
					punctuation = AppSetting.Instance.ToColor("#b4b4b4");
					break;
				case ThemeType.VS2019Light:
					punctuation = AppSetting.Instance.ToColor("#000000");
					break;
				}
			}
			return punctuation;
		}
		set
		{
			punctuation = value;
			RaisePropertyChanged("Punctuation");
		}
	}

	public SolidColorBrush MethodCall
	{
		get
		{
			if (methodCall == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					methodCall = AppSetting.Instance.ToColor("#74531f");
					break;
				case ThemeType.VS2019Dark:
					methodCall = AppSetting.Instance.ToColor("#FFdcdcaa");
					break;
				case ThemeType.VS2019Light:
					methodCall = AppSetting.Instance.ToColor("#74531f");
					break;
				}
			}
			return methodCall;
		}
		set
		{
			methodCall = value;
			RaisePropertyChanged("MethodCall");
		}
	}

	public SolidColorBrush GGenObject
	{
		get
		{
			if (genObject == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					genObject = AppSetting.Instance.ToColor("Teal");
					break;
				case ThemeType.VS2019Dark:
					genObject = AppSetting.Instance.ToColor("#FFd8a0df");
					break;
				case ThemeType.VS2019Light:
					genObject = AppSetting.Instance.ToColor("Teal");
					break;
				}
			}
			return genObject;
		}
		set
		{
			genObject = value;
			RaisePropertyChanged("GGenObject");
		}
	}

	public SolidColorBrush Keywords
	{
		get
		{
			if (keywords == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					keywords = AppSetting.Instance.ToColor("#8f08c4");
					break;
				case ThemeType.VS2019Dark:
					keywords = AppSetting.Instance.ToColor("#D8A0CC");
					break;
				case ThemeType.VS2019Light:
					keywords = AppSetting.Instance.ToColor("#8f08c4");
					break;
				}
			}
			return keywords;
		}
		set
		{
			keywords = value;
			RaisePropertyChanged("Keywords");
		}
	}

	public SolidColorBrush Keywords2
	{
		get
		{
			if (keywords2 == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					keywords2 = AppSetting.Instance.ToColor("#0000ff");
					break;
				case ThemeType.VS2019Dark:
					keywords2 = AppSetting.Instance.ToColor("#569CD6");
					break;
				case ThemeType.VS2019Light:
					keywords2 = AppSetting.Instance.ToColor("#0000ff");
					break;
				}
			}
			return keywords2;
		}
		set
		{
			keywords2 = value;
			RaisePropertyChanged("Keywords2");
		}
	}

	public SolidColorBrush LstItemNameForeBrush
	{
		get
		{
			if (lstItemNameForeBrush == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					lstItemNameForeBrush = AppSetting.Instance.ToColor("Gray");
					break;
				case ThemeType.VS2019Dark:
					lstItemNameForeBrush = AppSetting.Instance.ToColor("Gray");
					break;
				case ThemeType.VS2019Light:
					lstItemNameForeBrush = AppSetting.Instance.ToColor("Gray");
					break;
				}
			}
			return lstItemNameForeBrush;
		}
		set
		{
			lstItemNameForeBrush = value;
			RaisePropertyChanged("LstItemNameForeBrush");
		}
	}

	public SolidColorBrush KorStrIndex
	{
		get
		{
			if (korStrIndex == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					korStrIndex = AppSetting.Instance.ToColor("#f92672");
					break;
				case ThemeType.VS2019Dark:
					korStrIndex = AppSetting.Instance.ToColor("#f92672");
					break;
				case ThemeType.VS2019Light:
					korStrIndex = AppSetting.Instance.ToColor("#f92672");
					break;
				}
			}
			return korStrIndex;
		}
		set
		{
			korStrIndex = value;
			RaisePropertyChanged("KorStrIndex");
		}
	}

	public SolidColorBrush KorStrValue
	{
		get
		{
			if (korStrValue == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					korStrValue = AppSetting.Instance.ToColor("#a31515");
					break;
				case ThemeType.VS2019Dark:
					korStrValue = AppSetting.Instance.ToColor("#af7a66");
					break;
				case ThemeType.VS2019Light:
					korStrValue = AppSetting.Instance.ToColor("#a31515");
					break;
				}
			}
			return korStrValue;
		}
		set
		{
			korStrValue = value;
			RaisePropertyChanged("KorStrValue");
		}
	}

	public SolidColorBrush KorStrAngleBrackets
	{
		get
		{
			if (korStrAngleBrackets == null)
			{
				switch (ThemeTypeChina)
				{
				case ThemeType.VS2019Blue:
					korStrAngleBrackets = AppSetting.Instance.ToColor("#5b2da8");
					break;
				case ThemeType.VS2019Dark:
					korStrAngleBrackets = AppSetting.Instance.ToColor("#b5cea8");
					break;
				case ThemeType.VS2019Light:
					korStrAngleBrackets = AppSetting.Instance.ToColor("#5b2da8");
					break;
				}
			}
			return korStrAngleBrackets;
		}
		set
		{
			korStrAngleBrackets = value;
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
