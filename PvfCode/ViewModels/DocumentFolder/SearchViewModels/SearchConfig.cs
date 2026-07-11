using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
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

	private SourceType? QMQSkXD3b2;

	private PvfCode.ViewModels.DocumentFolder.SearchViewModels.Enums.SearchType edvS0NP9Tw;

	private string EcaS7BuXOA;

	private string xy0SXmgVXs;

	[CompilerGenerated]
	private DelegateConfigChanged WaLSpPej24;

	public SourceType SourceType
	{
		get
		{
			if (!QMQSkXD3b2.HasValue)
			{
				QMQSkXD3b2 = SourceType.当前文档;
			}
			return QMQSkXD3b2.Value;
		}
		set
		{
			QMQSkXD3b2 = value;
			RaisePropertyChanged("SourceType");
			OnConfigChanged();
		}
	}

	public PvfCode.ViewModels.DocumentFolder.SearchViewModels.Enums.SearchType SearchType
	{
		get
		{
			return edvS0NP9Tw;
		}
		set
		{
			edvS0NP9Tw = value;
			RaisePropertyChanged("SearchType");
			OnConfigChanged();
		}
	}

	public string FindKeyword
	{
		get
		{
			if (EcaS7BuXOA == null)
			{
				EcaS7BuXOA = string.Empty;
			}
			if (!AppSetting.Instance.EditConfig.SearchPanelKeywordConvertTW)
			{
				return EcaS7BuXOA;
			}
			return ChineseHelper.ToTraditional(EcaS7BuXOA);
		}
		set
		{
			EcaS7BuXOA = value;
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
			if (xy0SXmgVXs == null)
			{
				xy0SXmgVXs = string.Empty;
			}
			if (!AppSetting.Instance.EditConfig.SearchPanelKeywordConvertTW)
			{
				return xy0SXmgVXs;
			}
			return ChineseHelper.ToTraditional(xy0SXmgVXs);
		}
		set
		{
			xy0SXmgVXs = value;
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

	public event DelegateConfigChanged EventDelegateConfigChanged
	{
		[CompilerGenerated]
		add
		{
			DelegateConfigChanged delegateConfigChanged = WaLSpPej24;
			DelegateConfigChanged delegateConfigChanged2;
			do
			{
				delegateConfigChanged2 = delegateConfigChanged;
				DelegateConfigChanged value2 = (DelegateConfigChanged)Delegate.Combine(delegateConfigChanged2, value);
				delegateConfigChanged = Interlocked.CompareExchange(ref WaLSpPej24, value2, delegateConfigChanged2);
			}
			while ((object)delegateConfigChanged != delegateConfigChanged2);
		}
		[CompilerGenerated]
		remove
		{
			DelegateConfigChanged delegateConfigChanged = WaLSpPej24;
			DelegateConfigChanged delegateConfigChanged2;
			do
			{
				delegateConfigChanged2 = delegateConfigChanged;
				DelegateConfigChanged value2 = (DelegateConfigChanged)Delegate.Remove(delegateConfigChanged2, value);
				delegateConfigChanged = Interlocked.CompareExchange(ref WaLSpPej24, value2, delegateConfigChanged2);
			}
			while ((object)delegateConfigChanged != delegateConfigChanged2);
		}
	}

	public SearchConfig()
	{
	}

	public void OnConfigChanged()
	{
		WaLSpPej24?.Invoke();
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
