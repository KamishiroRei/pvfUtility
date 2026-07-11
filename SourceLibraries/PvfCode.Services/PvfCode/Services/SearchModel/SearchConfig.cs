using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

	private string A6TegXO3BM;

	private string fSEezbm2sq;

	private SearchType COwCuXYjDM;

	private SearchSourceType WHbCI695Ue;

	private SearchNormalUsing h3kCeBg2rh;

	private bool jMSCCuybY4;

	[CompilerGenerated]
	private HashSet<string> aIUClgIdOb;

	private ScriptContentSearchMode KOSCYTWjiY;

	[CompilerGenerated]
	private bool jUJC8sLhgf;

	private bool jybCjYgt8C;

	private bool ASnCMVv5a9;

	private bool EglCOfPjb5;

	[CompilerGenerated]
	private RemoveOrKeepFileType Vu3C0SDt6x;

	private List<string> sKwC3phcIU;

	private HashSet<string> K9nCPerIGX;

	private string V2bC1WAjQ2;

	private string VfiCydwAlY;

	private string KqfCHrh8ic;

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
			if (A6TegXO3BM == null)
			{
				A6TegXO3BM = string.Empty;
			}
			return A6TegXO3BM;
		}
		set
		{
			A6TegXO3BM = value;
			DoNotify("SearchFolder");
		}
	}

	public string Keyword
	{
		get
		{
			if (fSEezbm2sq == null)
			{
				fSEezbm2sq = string.Empty;
			}
			return fSEezbm2sq;
		}
		set
		{
			fSEezbm2sq = value;
			DoNotify("Keyword");
		}
	}

	public SearchType Type
	{
		get
		{
			return COwCuXYjDM;
		}
		set
		{
			COwCuXYjDM = value;
			DoNotify("Type");
			DoNotify("ImmediatePopup");
		}
	}

	public SearchSourceType SourceType
	{
		get
		{
			return WHbCI695Ue;
		}
		set
		{
			WHbCI695Ue = value;
			DoNotify("SourceType");
		}
	}

	public SearchNormalUsing NormalUsing
	{
		get
		{
			return h3kCeBg2rh;
		}
		set
		{
			h3kCeBg2rh = value;
			DoNotify("NormalUsing");
		}
	}

	public bool IsStartMatch
	{
		get
		{
			return jMSCCuybY4;
		}
		set
		{
			jMSCCuybY4 = value;
			DoNotify("IsStartMatch");
			if (value)
			{
				EglCOfPjb5 = false;
				DoNotify("WholeWordMatch");
			}
		}
	}

	public HashSet<string> SearchResult
	{
		[CompilerGenerated]
		get
		{
			return aIUClgIdOb;
		}
		[CompilerGenerated]
		set
		{
			aIUClgIdOb = value;
		}
	}

	[JsonIgnore]
	public bool ScriptContentSearchModeComBoBoxVisibility => !Trait;

	[JsonIgnore]
	public int ScriptContentSearchModeSelectedIndex => (int)ScriptContentSearchMode;

	public ScriptContentSearchMode ScriptContentSearchMode
	{
		get
		{
			return KOSCYTWjiY;
		}
		set
		{
			KOSCYTWjiY = value;
			DoNotify("ScriptContentSearchMode");
		}
	}

	public bool IsUseLikeSearchPath
	{
		[CompilerGenerated]
		get
		{
			return jUJC8sLhgf;
		}
		[CompilerGenerated]
		set
		{
			jUJC8sLhgf = value;
		}
	}

	public bool Trait
	{
		get
		{
			return jybCjYgt8C;
		}
		set
		{
			jybCjYgt8C = value;
			DoNotify("Trait");
			DoNotify("ScriptContentSearchModeComBoBoxVisibility");
		}
	}

	public bool UseRegularExpression
	{
		get
		{
			return ASnCMVv5a9;
		}
		set
		{
			ASnCMVv5a9 = value;
			DoNotify("UseRegularExpression");
		}
	}

	public bool WholeWordMatch
	{
		get
		{
			return EglCOfPjb5;
		}
		set
		{
			EglCOfPjb5 = value;
			DoNotify("WholeWordMatch");
			if (value)
			{
				jMSCCuybY4 = false;
				DoNotify("IsStartMatch");
			}
		}
	}

	public RemoveOrKeepFileType RemoveOrKeep
	{
		[CompilerGenerated]
		get
		{
			return Vu3C0SDt6x;
		}
		[CompilerGenerated]
		set
		{
			Vu3C0SDt6x = value;
		}
	}

	public List<string> FileTypesString
	{
		get
		{
			return sKwC3phcIU;
		}
		set
		{
			sKwC3phcIU = value;
			DoNotify("FileTypesString");
		}
	}

	[JsonIgnore]
	public HashSet<string> FileTypes
	{
		get
		{
			if (K9nCPerIGX == null)
			{
				K9nCPerIGX = new HashSet<string>();
				foreach (EnumberEntity item in EnumberHelper.EnumToList<PvfFileType>())
				{
					K9nCPerIGX.Add("." + item.EnumName);
				}
			}
			return K9nCPerIGX;
		}
	}

	public string ScriptContent
	{
		get
		{
			if (V2bC1WAjQ2 != null)
			{
				return V2bC1WAjQ2;
			}
			return string.Empty;
		}
		set
		{
			V2bC1WAjQ2 = value;
			DoNotify("ScriptContent");
		}
	}

	public string ScriptContentStart
	{
		get
		{
			return VfiCydwAlY;
		}
		set
		{
			VfiCydwAlY = value;
			DoNotify("ScriptContentStart");
		}
	}

	public string ScriptContentStop
	{
		get
		{
			return KqfCHrh8ic;
		}
		set
		{
			KqfCHrh8ic = value;
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
		if (!string.IsNullOrEmpty(fSEezbm2sq))
		{
			return fSEezbm2sq;
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
