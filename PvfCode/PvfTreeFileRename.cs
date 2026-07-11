using System.IO;

namespace PvfCode;

public class PvfTreeFileRename : PvfTreeFile
{
	private string m5QjaaBxKW;

	public string NewFileName
	{
		get
		{
			return m5QjaaBxKW;
		}
		set
		{
			m5QjaaBxKW = value;
			DoNotify("NewFileName");
			DoNotify("ChangedViewText");
		}
	}

	public string ChangedViewText
	{
		get
		{
			if (!Changed)
			{
				return null;
			}
			return "UP";
		}
	}

	public bool Changed => base.FileName != NewFileName;

	public PvfTreeFileRename(PvfGroup pvf, string fullPath, string fileName, bool isFile, short level)
		: base(pvf, fullPath, fileName, isFile, level)
	{
		NewFileName = fileName;
	}

	public bool CheckChanged(string newFileName)
	{
		return base.FileName != newFileName;
	}

	public string? GetNewFullPath(string? newFileName = null)
	{
		if (newFileName == null)
		{
			newFileName = NewFileName;
		}
		if (string.IsNullOrEmpty(newFileName))
		{
			return null;
		}
		string directoryName = Path.GetDirectoryName(base.FullPath);
		string text = ((directoryName != null) ? Path.Combine(directoryName, newFileName).Replace("\\", "/") : newFileName);
		return text.ToLower();
	}
}
