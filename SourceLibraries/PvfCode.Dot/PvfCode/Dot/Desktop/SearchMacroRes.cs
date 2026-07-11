using PvfCode.Dot.Desktop.Enums;

namespace PvfCode.Dot.Desktop;

public class SearchMacroRes : ModelBase
{
	private string _Keyword;

	private MacroType _MacroType;

	public string Keyword
	{
		get
		{
			return _Keyword;
		}
		set
		{
			_Keyword = value;
			DoNotify("Keyword");
		}
	}

	public MacroType MacroType
	{
		get
		{
			return _MacroType;
		}
		set
		{
			_MacroType = value;
			DoNotify("MacroType");
		}
	}
}
