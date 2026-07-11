using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DevExpress.Mvvm;
using Newtonsoft.Json;
using PvfCode.Dot;
using PvfCode.Models.BatchOperation.Enums;
using PvfCode.Models.Pvf.Enums;

namespace PvfCode.Models.BatchOperation;

[JsonObject(MemberSerialization.OptOut)]
public class BatchOperationConfig : ViewModelBase, ICloneable
{
	private string pyVZQKqjMS;

	private string wRjZ69mGmQ;

	private string qyqZyluwLC;

	private string ttPZwuD573;

	private string WVaZoJgrKO;

	private string B4MZ25myHZ;

	private int? vKDZtwU9En;

	[CompilerGenerated]
	private HashSet<string> nPsZ9JKUf0;

	private RemoveOrKeepFileType? O8yZR0yBYj;

	private List<string> HNPZWDhCd8;

	public string FindKeyword
	{
		get
		{
			if (pyVZQKqjMS == null)
			{
				pyVZQKqjMS = string.Empty;
			}
			return pyVZQKqjMS;
		}
		set
		{
			pyVZQKqjMS = value;
			RaisePropertyChanged("FindKeyword");
		}
	}

	public string ReplaceKeyword
	{
		get
		{
			if (wRjZ69mGmQ == null)
			{
				wRjZ69mGmQ = string.Empty;
			}
			return wRjZ69mGmQ;
		}
		set
		{
			wRjZ69mGmQ = value;
			RaisePropertyChanged("ReplaceKeyword");
		}
	}

	public string FindStartKeyword
	{
		get
		{
			if (string.IsNullOrEmpty(qyqZyluwLC))
			{
				qyqZyluwLC = string.Empty;
			}
			return qyqZyluwLC;
		}
		set
		{
			qyqZyluwLC = value;
			RaisePropertyChanged("FindStartKeyword");
		}
	}

	public string FindEndKeyword
	{
		get
		{
			if (ttPZwuD573 == null)
			{
				ttPZwuD573 = string.Empty;
			}
			return ttPZwuD573;
		}
		set
		{
			ttPZwuD573 = value;
			RaisePropertyChanged("FindEndKeyword");
		}
	}

	public string TraitReplaceKeyword
	{
		get
		{
			if (WVaZoJgrKO == null)
			{
				WVaZoJgrKO = string.Empty;
			}
			return WVaZoJgrKO;
		}
		set
		{
			WVaZoJgrKO = value;
			RaisePropertyChanged("TraitReplaceKeyword");
		}
	}

	public string AddContent
	{
		get
		{
			if (B4MZ25myHZ != null)
			{
				return B4MZ25myHZ;
			}
			return "";
		}
		set
		{
			B4MZ25myHZ = value;
			RaisePropertyChanged("AddContent");
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
			SetProperty(() => CaseSensitive, value);
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
			SetProperty(() => RegularExpression, value);
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

	public string DeleteSectionKeyword
	{
		get
		{
			return GetProperty(() => DeleteSectionKeyword);
		}
		set
		{
			SetProperty<string>(() => DeleteSectionKeyword, value);
		}
	}

	public int DeleteSectionLineNumber
	{
		get
		{
			if (!vKDZtwU9En.HasValue)
			{
				vKDZtwU9En = 1;
			}
			return vKDZtwU9En.Value;
		}
		set
		{
			vKDZtwU9En = value;
			RaisePropertyChanged("DeleteSectionLineNumber");
		}
	}

	public bool KeepDeleteSection
	{
		get
		{
			return GetProperty(() => KeepDeleteSection);
		}
		set
		{
			SetProperty(() => KeepDeleteSection, value);
		}
	}

	public bool HasEndSection
	{
		get
		{
			return GetProperty(() => HasEndSection);
		}
		set
		{
			SetProperty(() => HasEndSection, value);
		}
	}

	public bool RegularExpressionVisibility
	{
		get
		{
			if (BatchOperationType == BatchOperationType.TraitFindReplce)
			{
				return false;
			}
			return true;
		}
	}

	public int TabControlSelectedIndex
	{
		get
		{
			return (int)BatchOperationType;
		}
		set
		{
			BatchOperationType = (BatchOperationType)value;
			RaisePropertyChanged("TabControlSelectedIndex");
			RaisePropertiesChanged("RegularExpressionVisibility");
		}
	}

	public BatchOperationType BatchOperationType
	{
		get
		{
			return GetProperty(() => BatchOperationType);
		}
		set
		{
			SetProperty(() => BatchOperationType, value);
		}
	}

	public HashSet<string> SourceFiles
	{
		[CompilerGenerated]
		get
		{
			return nPsZ9JKUf0;
		}
		[CompilerGenerated]
		set
		{
			nPsZ9JKUf0 = value;
		}
	}

	public RemoveOrKeepFileType RemoveOrKeepFileType
	{
		get
		{
			if (!O8yZR0yBYj.HasValue)
			{
				O8yZR0yBYj = RemoveOrKeepFileType.保留;
			}
			return O8yZR0yBYj.Value;
		}
		set
		{
			O8yZR0yBYj = value;
			RaisePropertyChanged("RemoveOrKeepFileType");
		}
	}

	public List<string> FileTypes
	{
		get
		{
			return HNPZWDhCd8;
		}
		set
		{
			HNPZWDhCd8 = value;
			RaisePropertyChanged("FileTypes");
		}
	}

	public ResultData HasSection()
	{
		ResultData resultData = new ResultData();
		if (string.IsNullOrEmpty(DeleteSectionKeyword) || DeleteSectionKeyword.Length < 3)
		{
			resultData.Msg = "标签不能为空！且长度不能小于3";
			return resultData;
		}
		if (DeleteSectionKeyword[0] != '[' || DeleteSectionKeyword[DeleteSectionKeyword.Length - 1] != ']')
		{
			resultData.Msg = "错误的标签名 起始和结束字符必须时 []";
			return resultData;
		}
		return resultData;
	}

	public string GetEndSectionName()
	{
		return "[/" + DeleteSectionKeyword.Substring(1, DeleteSectionKeyword.Length - 2);
	}

	public void RefKeyword()
	{
	}

	public object Clone()
	{
		return MemberwiseClone();
	}

	public BatchOperationConfig CloneData()
	{
		return (BatchOperationConfig)Clone();
	}

	public BatchOperationConfig()
	{
	}
}
