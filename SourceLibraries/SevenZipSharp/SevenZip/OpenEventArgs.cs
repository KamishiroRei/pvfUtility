using System;

namespace SevenZip;

public sealed class OpenEventArgs : EventArgs
{
	private readonly ulong _totalSize;

	[CLSCompliant(false)]
	public ulong TotalSize => _totalSize;

	[CLSCompliant(false)]
	public OpenEventArgs(ulong totalSize)
	{
		_totalSize = totalSize;
	}
}
