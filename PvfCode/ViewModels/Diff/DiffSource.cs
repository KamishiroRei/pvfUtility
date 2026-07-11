using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using PvfCode.Dot;
using PvfCode.Models.Pvf.Enums;
using PvfCode.ViewModels.VsCodeEditorViewModels.Enums;
using SevenZip;

namespace PvfCode.ViewModels.Diff;

public class DiffSource : ModelBase
{
	[CompilerGenerated]
	private PvfGroup n4RGtFC3it;

	private string UxaGbsNuFf;

	[CompilerGenerated]
	private string hywGI3fZ6H;

	[CompilerGenerated]
	private bool fqFGEoQ62g;

	public PvfGroup Pvf
	{
		[CompilerGenerated]
		get
		{
			return n4RGtFC3it;
		}
		[CompilerGenerated]
		set
		{
			n4RGtFC3it = value;
		}
	}

	public string FilePath
	{
		get
		{
			return UxaGbsNuFf;
		}
		set
		{
			UxaGbsNuFf = value;
			DoNotify("FilePath");
		}
	}

	public string FilePath7z
	{
		[CompilerGenerated]
		get
		{
			return hywGI3fZ6H;
		}
		[CompilerGenerated]
		set
		{
			hywGI3fZ6H = value;
		}
	}

	public bool Is7z
	{
		[CompilerGenerated]
		get
		{
			return fqFGEoQ62g;
		}
		[CompilerGenerated]
		set
		{
			fqFGEoQ62g = value;
		}
	}

	public PvfFileType FileType => AppSetting.Instance.PvfConfig.GetPvfFileType(Path.GetExtension(FilePath));

	public DiffSource(PvfGroup pvf, string filePath)
	{
		Pvf = pvf;
		FilePath = filePath;
		Is7z = false;
	}

	public DiffSource(string filePath)
	{
		FilePath = filePath;
		Is7z = false;
	}

	public DiffSource(string filePath, string filePath7z)
	{
		FilePath = filePath;
		Is7z = true;
		FilePath7z = filePath7z;
	}

	public ResultData<string> GetFileText(EncodingType encodingType)
	{
		ResultData<string> resultData = new ResultData<string>();
		if (Pvf != null)
		{
			if (Pvf.FileAny(FilePath))
			{
				resultData.Data = Pvf.GetFileText(FilePath, encodingType);
			}
			else
			{
				resultData.Msg = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_FileNotExist"), FilePath);
			}
		}
		else
		{
			Encoding encoding = Encoding.GetEncoding((int)encodingType);
			if (Is7z)
			{
				ResultData<byte[]> resultData2 = SevenZipHelper.Get7zipFileBytes(FilePath7z, FilePath);
				if (resultData2.IsError)
				{
					resultData.Msg = resultData2.Msg;
					return resultData;
				}
				byte[] data = resultData2.Data;
				if (data != null && data.Length != 0)
				{
					resultData.Data = encoding.GetString(data).TrimEnd(new char[1]);
				}
				else
				{
					resultData.Data = "";
				}
			}
			else if (File.Exists(FilePath))
			{
				resultData.Data = File.ReadAllText(FilePath, encoding);
			}
			else
			{
				resultData.Msg = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_FileNotExist"), FilePath);
			}
		}
		return resultData;
	}

	public LanguageType GetLanguageType()
	{
		if (FileType == PvfFileType.kor)
		{
			return LanguageType.Kor;
		}
		if (FileType == PvfFileType.nut)
		{
			return LanguageType.Squirrel;
		}
		return LanguageType.ScriptLanguage;
	}

	[SpecialName]
	private bool q5PGddWBYQ()
	{
		return Pvf == null;
	}

	public ResultData SaveFileText(string newText)
	{
		ResultData resultData = new ResultData();
		try
		{
			if (Is7z)
			{
				resultData.Msg = AppSetting.Instance.GetIlogger()?.GetStr("mess_7zNotSupport");
				return resultData;
			}
			if (q5PGddWBYQ())
			{
				File.WriteAllText(FilePath, newText, Encoding.UTF8);
			}
			else
			{
				Pvf.SaveFileText(FilePath, newText);
			}
		}
		catch (Exception ex)
		{
			resultData.Msg = ex.Message;
		}
		return resultData;
	}
}
