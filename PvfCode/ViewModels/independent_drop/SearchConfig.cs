using System;
using System.Collections.Generic;
using DevExpress.Mvvm;
using PvfCode.ViewModels.independent_drop.Enums;

namespace PvfCode.ViewModels.independent_drop;

public class SearchConfig : ViewModelBase
{
	public string SearchKeyword
	{
		get
		{
			return GetProperty(() => SearchKeyword);
		}
		set
		{
			SetProperty<string>(() => SearchKeyword, value);
		}
	}

	public SearchType SearchType
	{
		get
		{
			return GetProperty(() => SearchType);
		}
		set
		{
			SetProperty(() => SearchType, value);
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

	public HashSet<int> KeywordCodes { get; set; }

	public void KeywordConvertNumberList()
	{
		KeywordCodes = new HashSet<int>();
		if (string.IsNullOrEmpty(SearchKeyword))
		{
			return;
		}
		string[] array = SearchKeyword.Split(new char[6] { ' ', ',', ';', '，', '；', '\t' }, StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < array.Length; i++)
		{
			if (int.TryParse(array[i], out var result))
			{
				KeywordCodes.Add(result);
			}
		}
	}

	public SearchConfig()
	{
	}
}
