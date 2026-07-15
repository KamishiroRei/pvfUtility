using System;
using System.Collections.Generic;
using DevExpress.Mvvm;
using Newtonsoft.Json;
using PvfCode.Dot;
using PvfCode.Models.BatchOperation.Enums;
using PvfCode.Models.Pvf.Enums;

namespace PvfCode.Models.BatchOperation;

[JsonObject(MemberSerialization.OptOut)]
public class BatchOperationConfig : ViewModelBase, ICloneable
{
	private string findKeyword;

	private string replaceKeyword;

	private string findStartKeyword;

	private string findEndKeyword;

	private string traitReplaceKeyword;

	private string addContent;

	private int? deleteSectionLineNumber;

	private RemoveOrKeepFileType? removeOrKeepFileType;

	private List<string> fileTypes;

	public string FindKeyword
	{
		get
		{
			if (findKeyword == null)
			{
				findKeyword = string.Empty;
			}
			return findKeyword;
		}
		set
		{
			findKeyword = value;
			RaisePropertyChanged(nameof(FindKeyword));
		}
	}

	public string ReplaceKeyword
	{
		get
		{
			if (replaceKeyword == null)
			{
				replaceKeyword = string.Empty;
			}
			return replaceKeyword;
		}
		set
		{
			replaceKeyword = value;
			RaisePropertyChanged(nameof(ReplaceKeyword));
		}
	}

	public string FindStartKeyword
	{
		get
		{
			if (string.IsNullOrEmpty(findStartKeyword))
			{
				findStartKeyword = string.Empty;
			}
			return findStartKeyword;
		}
		set
		{
			findStartKeyword = value;
			RaisePropertyChanged(nameof(FindStartKeyword));
		}
	}

	public string FindEndKeyword
	{
		get
		{
			if (findEndKeyword == null)
			{
				findEndKeyword = string.Empty;
			}
			return findEndKeyword;
		}
		set
		{
			findEndKeyword = value;
			RaisePropertyChanged(nameof(FindEndKeyword));
		}
	}

	public string TraitReplaceKeyword
	{
		get
		{
			if (traitReplaceKeyword == null)
			{
				traitReplaceKeyword = string.Empty;
			}
			return traitReplaceKeyword;
		}
		set
		{
			traitReplaceKeyword = value;
			RaisePropertyChanged(nameof(TraitReplaceKeyword));
		}
	}

	public string AddContent
	{
		get
		{
			if (addContent != null)
			{
				return addContent;
			}
			return "";
		}
		set
		{
			addContent = value;
			RaisePropertyChanged(nameof(AddContent));
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
			if (!deleteSectionLineNumber.HasValue)
			{
				deleteSectionLineNumber = 1;
			}
			return deleteSectionLineNumber.Value;
		}
		set
		{
			deleteSectionLineNumber = value;
			RaisePropertyChanged(nameof(DeleteSectionLineNumber));
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

	public HashSet<string> SourceFiles { get; set; }

	public RemoveOrKeepFileType RemoveOrKeepFileType
	{
		get
		{
			if (!removeOrKeepFileType.HasValue)
			{
				removeOrKeepFileType = RemoveOrKeepFileType.保留;
			}
			return removeOrKeepFileType.Value;
		}
		set
		{
			removeOrKeepFileType = value;
			RaisePropertyChanged(nameof(RemoveOrKeepFileType));
		}
	}

	public List<string> FileTypes
	{
		get
		{
			return fileTypes;
		}
		set
		{
			fileTypes = value;
			RaisePropertyChanged(nameof(FileTypes));
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
}
