using System;
using DevExpress.Mvvm;
using GMTool.Dot.QueryModel.Enums;
using Utools;

namespace GMTool.Dot.QueryModel;

public class FindUserDto : ViewModelBase, ICloneable
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
			return _Keyword.GbkToLatin1New();
		}
		set
		{
			_Keyword = value;
			RaisePropertyChanged("Keyword");
		}
	}

	public FindUserType FindUserType
	{
		get
		{
			return GetProperty(() => FindUserType);
		}
		set
		{
			SetProperty(() => FindUserType, value);
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

	public bool FilterDummies
	{
		get
		{
			return GetProperty(() => FilterDummies);
		}
		set
		{
			SetProperty(() => FilterDummies, value);
		}
	}

	public object Clone()
	{
		return MemberwiseClone();
	}

	public FindUserDto CloneData()
	{
		return (FindUserDto)Clone();
	}
}
