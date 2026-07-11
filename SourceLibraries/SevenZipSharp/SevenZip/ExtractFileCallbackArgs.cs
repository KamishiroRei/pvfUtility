using System;
using System.IO;

namespace SevenZip;

public class ExtractFileCallbackArgs : EventArgs
{
	private readonly ArchiveFileInfo _archiveFileInfo;

	private Stream _extractToStream;

	public ArchiveFileInfo ArchiveFileInfo => _archiveFileInfo;

	public ExtractFileCallbackReason Reason { get; internal set; }

	public Exception Exception { get; set; }

	public bool CancelExtraction { get; set; }

	public string ExtractToFile { get; set; }

	public Stream ExtractToStream
	{
		get
		{
			return _extractToStream;
		}
		set
		{
			if (_extractToStream != null && !_extractToStream.CanWrite)
			{
				throw new ExtractionFailedException("The specified stream is not writable!");
			}
			_extractToStream = value;
		}
	}

	public object ObjectData { get; set; }

	public ExtractFileCallbackArgs(ArchiveFileInfo archiveFileInfo)
	{
		Reason = ExtractFileCallbackReason.Start;
		_archiveFileInfo = archiveFileInfo;
	}
}
