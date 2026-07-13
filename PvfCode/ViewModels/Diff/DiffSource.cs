using System;
using System.IO;
using System.Text;
using PvfCode.Dot;
using PvfCode.Models.Pvf.Enums;
using PvfCode.ViewModels.VsCodeEditorViewModels.Enums;
using SevenZip;

namespace PvfCode.ViewModels.Diff;

public class DiffSource : ModelBase
{
	private string filePath;

	public PvfGroup Pvf { get; set; }

	public string FilePath
	{
		get
		{
			return filePath;
		}
		set
		{
			filePath = value;
			DoNotify("FilePath");
		}
	}

	public string FilePath7z { get; set; }

	public bool Is7z { get; set; }

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
			if (Pvf == null)
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
