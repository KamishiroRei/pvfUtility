using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using PvfCode.Models.Pvf.Enums;

namespace PvfCode.Services.ExtractFilesModel;

[JsonObject(MemberSerialization.OptOut)]
public class ExtractConfig : ModelBase, ICloneable
{
	public string _TargetPath;

	public string _TargetPath7z;

	private bool extractSuccessOpenFolder;

	private FileOperation? operation;

	private bool decompileScript;

	private bool decompileBinaryAni;

	private bool convertConvertSimplifiedChinese;

	private bool extractTo7zip;

	private bool extractToAutoImportFileGroup;

	private RemoveOrKeepFileType? removeOrKeepFileType;

	private List<string> fileTypes;

	private bool extractCorrespondenceFileLst;

	private bool useCompatibleDecompiler;

	public string TargetPath
	{
		get
		{
			if (ExtractTo7zip)
			{
				return _TargetPath7z;
			}
			return GetDiskTargetPath;
		}
		set
		{
			if (ExtractTo7zip)
			{
				if (value != null && Path.GetExtension(value).ToLower() != ".7z")
				{
					value += ".7z";
				}
				_TargetPath7z = value;
			}
			else
			{
				_TargetPath = value;
			}
			DoNotify("TargetPath");
			DoNotify("GetDiskTargetPath");
		}
	}

	[JsonIgnore]
	public string GetDiskTargetPath
	{
		get
		{
			if (string.IsNullOrEmpty(_TargetPath) && AppSetting.Instance.PathConfig.PvfOpenLog != null && AppSetting.Instance.PathConfig.PvfOpenLog.Count > 0)
			{
				return Path.Combine(Path.GetDirectoryName(AppSetting.Instance.PathConfig.PvfOpenLog.ToList()[0].Key), "script");
			}
			return _TargetPath;
		}
	}

	[JsonIgnore]
	public HashSet<string> SourceFiles { get; set; }

	public bool ExtractSuccessOpenFolder
	{
		get
		{
			return extractSuccessOpenFolder;
		}
		set
		{
			extractSuccessOpenFolder = value;
			DoNotify("ExtractSuccessOpenFolder");
		}
	}

	public FileOperation Operation
	{
		get
		{
			if (!operation.HasValue)
			{
				operation = FileOperation.Cover;
			}
			return operation.Value;
		}
		set
		{
			operation = value;
			DoNotify("Operation");
		}
	}

	public bool DecompileScript
	{
		get
		{
			return decompileScript;
		}
		set
		{
			decompileScript = value;
			DoNotify("DecompileScript");
		}
	}

	public bool DecompileBinaryAni
	{
		get
		{
			return decompileBinaryAni;
		}
		set
		{
			decompileBinaryAni = value;
			DoNotify("DecompileBinaryAni");
		}
	}

	public bool ConvertConvertSimplifiedChinese
	{
		get
		{
			return convertConvertSimplifiedChinese;
		}
		set
		{
			convertConvertSimplifiedChinese = value;
			DoNotify("ConvertConvertSimplifiedChinese");
		}
	}

	public bool ExtractTo7zip
	{
		get
		{
			return extractTo7zip;
		}
		set
		{
			extractTo7zip = value;
			DoNotify("ExtractTo7zip");
			DoNotify("TargetPath");
		}
	}

	public bool ExtractToAutoImportFileGroup
	{
		get
		{
			return extractToAutoImportFileGroup;
		}
		set
		{
			extractToAutoImportFileGroup = value;
			DoNotify("ExtractToAutoImportFileGroup");
			if (value)
			{
				ExtractTo7zip = true;
			}
		}
	}

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
			DoNotify("RemoveOrKeepFileType");
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
			DoNotify("FileTypes");
		}
	}

	public bool ExtractCorrespondenceFileLst
	{
		get
		{
			return extractCorrespondenceFileLst;
		}
		set
		{
			extractCorrespondenceFileLst = value;
			DoNotify("ExtractCorrespondenceFileLst");
		}
	}

	public bool UseCompatibleDecompiler
	{
		get
		{
			return useCompatibleDecompiler;
		}
		set
		{
			useCompatibleDecompiler = value;
			DoNotify("UseCompatibleDecompiler");
		}
	}

	public ExtractConfig()
	{
		ResSet();
	}

	public void ResSet()
	{
		UseCompatibleDecompiler = false;
		TargetPath = string.Empty;
		SourceFiles = new HashSet<string>();
		ExtractSuccessOpenFolder = false;
		Operation = FileOperation.Cover;
		DecompileScript = true;
		DecompileBinaryAni = true;
		ConvertConvertSimplifiedChinese = false;
		ExtractTo7zip = false;
		ExtractToAutoImportFileGroup = false;
		RemoveOrKeepFileType = RemoveOrKeepFileType.排除;
		FileTypes = null;
		ExtractCorrespondenceFileLst = false;
	}

	public object Clone()
	{
		return MemberwiseClone();
	}

	public ExtractConfig CloneData()
	{
		return (ExtractConfig)Clone();
	}
}
