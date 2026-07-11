using DevExpress.Mvvm;

namespace GMTool.Dot;

public class ItemCodeFindRes : ViewModelBase
{
	private string _Keyword;

	public string Keyword
	{
		get
		{
			if (_Keyword == null)
			{
				_Keyword = string.Empty;
			}
			return _Keyword;
		}
		set
		{
			_Keyword = value;
		}
	}

	public bool WholeWordMatch
	{
		get
		{
			return GetProperty(() => WholeWordMatch);
		}
		set
		{
			SetProperty(() => WholeWordMatch, value);
		}
	}

	public bool IsStartMatch
	{
		get
		{
			return GetProperty(() => IsStartMatch);
		}
		set
		{
			SetProperty(() => IsStartMatch, value);
		}
	}
}
