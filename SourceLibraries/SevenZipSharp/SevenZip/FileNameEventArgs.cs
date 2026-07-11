using System;

namespace SevenZip;

public sealed class FileNameEventArgs : PercentDoneEventArgs, ICancellable
{
	private readonly string _fileName;

	public bool Cancel { get; set; }

	public bool Skip
	{
		get
		{
			return false;
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	public string FileName => _fileName;

	public FileNameEventArgs(string fileName, byte percentDone)
		: base(percentDone)
	{
		_fileName = fileName;
	}
}
