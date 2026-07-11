using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using PvfCode.Models.Pvf.Enums;

namespace PvfCode.Services.ExtractFilesModel;

[JsonObject(MemberSerialization.OptOut)]
public class ExtractConfig : ModelBase, ICloneable
{
	public string _TargetPath;

	public string _TargetPath7z;

	[CompilerGenerated]
	private HashSet<string> GXEkKkeaR3;

	private bool LCHkYJOVND;

	private FileOperation? DaGkJtFdvE;

	private bool KW6kdgSFrv;

	private bool Cr4k1Xx1CS;

	private bool mIekGjIqEL;

	private bool NwqkOBU4NZ;

	private bool pUHkrNCCks;

	private RemoveOrKeepFileType? VG1ken2x20;

	private List<string> fa9kjgKJxc;

	private bool sWjkxSuqon;

	private bool vQRkmYOGob;

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
	public HashSet<string> SourceFiles
	{
		[CompilerGenerated]
		get
		{
			return GXEkKkeaR3;
		}
		[CompilerGenerated]
		set
		{
			GXEkKkeaR3 = value;
		}
	}

	public bool ExtractSuccessOpenFolder
	{
		get
		{
			return LCHkYJOVND;
		}
		set
		{
			LCHkYJOVND = value;
			DoNotify("ExtractSuccessOpenFolder");
		}
	}

	public FileOperation Operation
	{
		get
		{
			if (!DaGkJtFdvE.HasValue)
			{
				DaGkJtFdvE = FileOperation.Cover;
			}
			return DaGkJtFdvE.Value;
		}
		set
		{
			DaGkJtFdvE = value;
			DoNotify("Operation");
		}
	}

	public bool DecompileScript
	{
		get
		{
			return KW6kdgSFrv;
		}
		set
		{
			KW6kdgSFrv = value;
			DoNotify("DecompileScript");
		}
	}

	public bool DecompileBinaryAni
	{
		get
		{
			return Cr4k1Xx1CS;
		}
		set
		{
			Cr4k1Xx1CS = value;
			DoNotify("DecompileBinaryAni");
		}
	}

	public bool ConvertConvertSimplifiedChinese
	{
		get
		{
			return mIekGjIqEL;
		}
		set
		{
			mIekGjIqEL = value;
			DoNotify("ConvertConvertSimplifiedChinese");
		}
	}

	public bool ExtractTo7zip
	{
		get
		{
			return NwqkOBU4NZ;
		}
		set
		{
			NwqkOBU4NZ = value;
			DoNotify("ExtractTo7zip");
			DoNotify("TargetPath");
		}
	}

	public bool ExtractToAutoImportFileGroup
	{
		get
		{
			return pUHkrNCCks;
		}
		set
		{
			pUHkrNCCks = value;
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
			if (!VG1ken2x20.HasValue)
			{
				VG1ken2x20 = RemoveOrKeepFileType.保留;
			}
			return VG1ken2x20.Value;
		}
		set
		{
			VG1ken2x20 = value;
			DoNotify("RemoveOrKeepFileType");
		}
	}

	public List<string> FileTypes
	{
		get
		{
			return fa9kjgKJxc;
		}
		set
		{
			fa9kjgKJxc = value;
			DoNotify("FileTypes");
		}
	}

	public bool ExtractCorrespondenceFileLst
	{
		get
		{
			return sWjkxSuqon;
		}
		set
		{
			sWjkxSuqon = value;
			DoNotify("ExtractCorrespondenceFileLst");
		}
	}

	public bool UseCompatibleDecompiler
	{
		get
		{
			return vQRkmYOGob;
		}
		set
		{
			vQRkmYOGob = value;
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
