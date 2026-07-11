using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PvfCode.Dot;
using PvfCode.ViewModels.DocumentFolder.SearchViewModels.Enums;

namespace PvfCode.ViewModels.DocumentFolder.SearchViewModels.Searchs;

public static class SearchStrategyFactory
{
	public static async Task<ResultData<ISearchStrategy>> Create(SearchConfig config)
	{
		ResultData<ISearchStrategy> re = new ResultData<ISearchStrategy>();
		if (config.NameConvertCode)
		{
			ResultData<int> resultData = await config.ItemNameConvertItemCode();
			if (resultData.IsError)
			{
				re.Msg = resultData.Msg;
			}
			else
			{
				AppCore.Logger.Success(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_NameConvertCodeResult"), config.FindKeyword, resultData.Data));
				re.Data = Create(resultData.Data.ToString(), !config.CaseSensitive, config.WholeWordMatch, config.RegularExpression ? SearchMode.RegEx : SearchMode.Normal);
			}
		}
		else
		{
			re.Data = Create(config.FindKeyword, !config.CaseSensitive, config.WholeWordMatch, config.RegularExpression ? SearchMode.RegEx : SearchMode.Normal);
		}
		return re;
	}

	public static ISearchStrategy Create(string searchPattern, bool ignoreCase, bool matchWholeWords, SearchMode mode)
	{
		if (searchPattern == null)
		{
			throw new ArgumentNullException("searchPattern");
		}
		RegexOptions regexOptions = RegexOptions.Multiline | RegexOptions.Compiled;
		if (ignoreCase)
		{
			regexOptions |= RegexOptions.IgnoreCase;
		}
		switch (mode)
		{
		case SearchMode.Normal:
			searchPattern = Regex.Escape(searchPattern);
			break;
		case SearchMode.Wildcard:
			searchPattern = JLNAkZjBUl(searchPattern);
			break;
		}
		try
		{
			return new RegexSearchStrategy(new Regex(searchPattern, regexOptions), matchWholeWords);
		}
		catch (ArgumentException ex)
		{
			throw new Exception(ex.Message);
		}
	}

	private static string JLNAkZjBUl(string P_0)
	{
		if (string.IsNullOrEmpty(P_0))
		{
			return "";
		}
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < P_0.Length; i++)
		{
			char c = P_0[i];
			switch (c)
			{
			case '?':
				stringBuilder.Append(".");
				break;
			case '*':
				stringBuilder.Append(".*");
				break;
			default:
				stringBuilder.Append(Regex.Escape(c.ToString()));
				break;
			}
		}
		return stringBuilder.ToString();
	}
}
