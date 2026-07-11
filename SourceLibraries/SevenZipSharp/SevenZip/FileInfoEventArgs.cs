namespace SevenZip;

public sealed class FileInfoEventArgs : PercentDoneEventArgs, ICancellable
{
	private readonly ArchiveFileInfo _fileInfo;

	public bool Cancel { get; set; }

	public bool Skip { get; set; }

	public ArchiveFileInfo FileInfo => _fileInfo;

	public FileInfoEventArgs(ArchiveFileInfo fileInfo, byte percentDone)
		: base(percentDone)
	{
		_fileInfo = fileInfo;
	}
}
