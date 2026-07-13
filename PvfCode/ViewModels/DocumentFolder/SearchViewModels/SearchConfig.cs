using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.Mvvm;
using Newtonsoft.Json;
using PvfCode.Dot;
using PvfCode.Services.SearchModel;
using PvfCode.ViewModels.DocumentFolder.SearchViewModels.Enums;
using Utools;

namespace PvfCode.ViewModels.DocumentFolder.SearchViewModels;

[JsonObject(MemberSerialization.OptOut)]
public class SearchConfig : ViewModelBase
{
	public delegate void DelegateConfigChanged();

	private SourceType? _sourceType;

	private PvfCode.ViewModels.DocumentFolder.SearchViewModels.Enums.SearchType _searchType;

	private string _findKeyword;

	private string _replaceKeyword;

	public SourceType SourceType
	{
		get
		{
			if (!_sourceType.HasValue)
			{
				_sourceType = SourceType.当前文档;
			}
			return _sourceType.Value;
		}
		set
		{
			_sourceType = value;
			RaisePropertyChanged("SourceType");
			OnConfigChanged();
		}
	}

	public PvfCode.ViewModels.DocumentFolder.SearchViewModels.Enums.SearchType SearchType
	{
		get
		{
			return _searchType;
		}
		set
		{
			_searchType = value;
			RaisePropertyChanged("SearchType");
			OnConfigChanged();
		}
	}

	public string FindKeyword
	{
		get
		{
			if (_findKeyword == null)
			{
				_findKeyword = string.Empty;
			}
			if (!AppSetting.Instance.EditConfig.SearchPanelKeywordConvertTW)
			{
				return _findKeyword;
			}
			return ChineseHelper.ToTraditional(_findKeyword);
		}
		set
		{
			_findKeyword = value;
			RaisePropertyChanged("FindKeyword");
			OnConfigChanged();
		}
	}

	public bool KeywordConvertTW
	{
		get
		{
			return AppSetting.Instance.EditConfig.SearchPanelKeywordConvertTW;
		}
		set
		{
			AppSetting.Instance.EditConfig.SearchPanelKeywordConvertTW = value;
			RaisePropertyChanged("KeywordConvertTW");
			OnConfigChanged();
		}
	}

	public bool NameConvertCode
	{
		get
		{
			return AppSetting.Instance.EditConfig.SearchPanelNameConvertCode;
		}
		set
		{
			AppSetting.Instance.EditConfig.SearchPanelNameConvertCode = value;
			RaisePropertyChanged("NameConvertCode");
			OnConfigChanged();
		}
	}

	public string ReplaceKeyword
	{
		get
		{
			if (_replaceKeyword == null)
			{
				_replaceKeyword = string.Empty;
			}
			if (!AppSetting.Instance.EditConfig.SearchPanelKeywordConvertTW)
			{
				return _replaceKeyword;
			}
			return ChineseHelper.ToTraditional(_replaceKeyword);
		}
		set
		{
			_replaceKeyword = value;
			RaisePropertyChanged("ReplaceKeyword");
		}
	}

	public bool CaseSensitive
	{
		get
		{
			return GetProperty(() => CaseSensitive);
		}
		set
		{
			SetProperty(() => CaseSensitive, value, OnConfigChanged);
		}
	}

	public bool RegularExpression
	{
		get
		{
			return GetProperty(() => RegularExpression);
		}
		set
		{
			SetProperty(() => RegularExpression, value, OnConfigChanged);
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
			SetProperty(() => WholeWordMatch, value, OnConfigChanged);
		}
	}

	public event DelegateConfigChanged EventDelegateConfigChanged;

	public SearchConfig()
	{
	}

	public void OnConfigChanged()
	{
		EventDelegateConfigChanged?.Invoke();
	}

	public async Task<ResultData<int>> ItemNameConvertItemCode()
	{
		ResultData<int> re = new ResultData<int>();
		if (string.IsNullOrEmpty(FindKeyword))
		{
			re.Msg = AppSetting.Instance.GetIlogger()?.GetStr("EditorSearchItemCodeConvertNameErr");
			return re;
		}
		if (!AppCore.ViewModelBase.PVF.PvfIsOpen)
		{
			re.Msg = AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseLoadPvfPackFirst");
			return re;
		}
		ResultData<HashSet<string>> resultData = await new SearchService(new PvfCode.Services.SearchModel.SearchConfig
		{
			Keyword = FindKeyword,
			NormalUsing = SearchNormalUsing.None,
			Type = PvfCode.Services.SearchModel.SearchType.Strings,
			WholeWordMatch = true,
			SourceType = SearchSourceType.AllFiles,
			IsStartMatch = false
		}, AppCore.ViewModelBase.PVF).Search();
		if (resultData.IsError)
		{
			re.Msg = resultData.Msg;
			return re;
		}
		if (resultData.Data == null || !resultData.Data.Any())
		{
			re.Msg = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_NoFindCode"), FindKeyword);
		}
		else
		{
			int? num = null;
			foreach (string datum in resultData.Data)
			{
				PvfFile file = AppCore.ViewModelBase.PVF.GetFile(datum);
				if (file != null && file.IsScriptFile && file.ItemCode.HasValue)
				{
					num = file.ItemCode;
					break;
				}
			}
			if (!num.HasValue)
			{
				re.Msg = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_NoFindCode"), FindKeyword);
			}
			else
			{
				re.Data = num.Value;
			}
		}
		return re;
	}
}
