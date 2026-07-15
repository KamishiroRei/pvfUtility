using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using PvfCode.Models.Pvf.Enums;
using PvfCode.Services.SearchModel.Enums;
using Utools;

namespace PvfCode.Services.SearchModel;

[JsonObject(MemberSerialization.OptOut)]
public class SearchConfig : ModelBase, ICloneable
{
	[JsonIgnore]
	public Regex Regex;

	private string searchFolder;

	private string keyword;

	private SearchType type;

	private SearchSourceType sourceType;

	private SearchNormalUsing normalUsing;

	private bool isStartMatch;

	private ScriptContentSearchMode scriptContentSearchMode;

	private bool trait;

	private bool useRegularExpression;

	private bool wholeWordMatch;

	private List<string> fileTypesString;

	private HashSet<string> fileTypes;

	private string scriptContent;

	private string scriptContentStart;

	private string scriptContentStop;

	[JsonIgnore]
	public bool ImmediatePopup => Type switch
	{
		SearchType.Num => false, 
		SearchType.Strings => AppSetting.Instance.PublicSearchServiceOptions.OpenStringAndSectionCompletion, 
		SearchType.FileName => AppSetting.Instance.PublicSearchServiceOptions.OpenFilePathCompletion, 
		SearchType.ScriptContent => false, 
		SearchType.Name => AppSetting.Instance.PublicSearchServiceOptions.OpenNameCompletion, 
		_ => false, 
	};

	public string SearchFolder
	{
		get
		{
			if (searchFolder == null)
			{
				searchFolder = string.Empty;
			}
			return searchFolder;
		}
		set
		{
			searchFolder = value;
			DoNotify("SearchFolder");
		}
	}

	public string Keyword
	{
		get
		{
			if (keyword == null)
			{
				keyword = string.Empty;
			}
			return keyword;
		}
		set
		{
			keyword = value;
			DoNotify("Keyword");
		}
	}

	public SearchType Type
	{
		get
		{
			return type;
		}
		set
		{
			type = value;
			DoNotify("Type");
			DoNotify("ImmediatePopup");
		}
	}

	public SearchSourceType SourceType
	{
		get
		{
			return sourceType;
		}
		set
		{
			sourceType = value;
			DoNotify("SourceType");
		}
	}

	public SearchNormalUsing NormalUsing
	{
		get
		{
			return normalUsing;
		}
		set
		{
			normalUsing = value;
			DoNotify("NormalUsing");
		}
	}

	public bool IsStartMatch
	{
		get
		{
			return isStartMatch;
		}
		set
		{
			isStartMatch = value;
			DoNotify("IsStartMatch");
			if (value)
			{
				wholeWordMatch = false;
				DoNotify("WholeWordMatch");
			}
		}
	}

	public HashSet<string> SearchResult { get; set; }

	[JsonIgnore]
	public bool ScriptContentSearchModeComBoBoxVisibility => !Trait;

	[JsonIgnore]
	public int ScriptContentSearchModeSelectedIndex => (int)ScriptContentSearchMode;

	public ScriptContentSearchMode ScriptContentSearchMode
	{
		get
		{
			return scriptContentSearchMode;
		}
		set
		{
			scriptContentSearchMode = value;
			DoNotify("ScriptContentSearchMode");
		}
	}

	public bool IsUseLikeSearchPath { get; set; }

	public bool Trait
	{
		get
		{
			return trait;
		}
		set
		{
			trait = value;
			DoNotify("Trait");
			DoNotify("ScriptContentSearchModeComBoBoxVisibility");
		}
	}

	public bool UseRegularExpression
	{
		get
		{
			return useRegularExpression;
		}
		set
		{
			useRegularExpression = value;
			DoNotify("UseRegularExpression");
		}
	}

	public bool WholeWordMatch
	{
		get
		{
			return wholeWordMatch;
		}
		set
		{
			wholeWordMatch = value;
			DoNotify("WholeWordMatch");
			if (value)
			{
				isStartMatch = false;
				DoNotify("IsStartMatch");
			}
		}
	}

	public RemoveOrKeepFileType RemoveOrKeep { get; set; }

	public List<string> FileTypesString
	{
		get
		{
			return fileTypesString;
		}
		set
		{
			fileTypesString = value;
			DoNotify("FileTypesString");
		}
	}

	[JsonIgnore]
	public HashSet<string> FileTypes
	{
		get
		{
			if (fileTypes == null)
			{
				fileTypes = new HashSet<string>();
				foreach (EnumberEntity item in EnumberHelper.EnumToList<PvfFileType>())
				{
					fileTypes.Add("." + item.EnumName);
				}
			}
			return fileTypes;
		}
	}

	public string ScriptContent
	{
		get
		{
			if (scriptContent != null)
			{
				return scriptContent;
			}
			return string.Empty;
		}
		set
		{
			scriptContent = value;
			DoNotify("ScriptContent");
		}
	}

	public string ScriptContentStart
	{
		get
		{
			return scriptContentStart;
		}
		set
		{
			scriptContentStart = value;
			DoNotify("ScriptContentStart");
		}
	}

	public string ScriptContentStop
	{
		get
		{
			return scriptContentStop;
		}
		set
		{
			scriptContentStop = value;
			DoNotify("ScriptContentStop");
		}
	}

	public SearchConfig()
	{
		ReSet();
	}

	public void ReSet()
	{
		SearchFolder = string.Empty;
		Keyword = string.Empty;
		Type = SearchType.Strings;
		SourceType = SearchSourceType.AllFiles;
		NormalUsing = SearchNormalUsing.None;
		RemoveOrKeep = RemoveOrKeepFileType.保留;
		FileTypesString = null;
		ScriptContent = string.Empty;
		ScriptContentStart = string.Empty;
		ScriptContentStop = string.Empty;
		Trait = false;
	}

	public string GetKey()
	{
		if (Type == SearchType.ScriptContent)
		{
			if (Trait)
			{
				if (!string.IsNullOrEmpty(ScriptContentStart))
				{
					return ScriptContentStart;
				}
				return "未命名";
			}
			if (!string.IsNullOrEmpty(ScriptContent))
			{
				return ScriptContent;
			}
			return "未命名";
		}
		if (!string.IsNullOrEmpty(keyword))
		{
			return keyword;
		}
		return "未命名";
	}

	public object Clone()
	{
		return MemberwiseClone();
	}

	public SearchConfig CloneData()
	{
		return (SearchConfig)Clone();
	}
}
