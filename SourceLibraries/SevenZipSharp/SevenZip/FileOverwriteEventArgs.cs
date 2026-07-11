using System;

namespace SevenZip;

public sealed class FileOverwriteEventArgs : EventArgs
{
	public bool Cancel { get; set; }

	public string FileName { get; set; }

	public FileOverwriteEventArgs(string fileName)
	{
		FileName = fileName;
	}
}
