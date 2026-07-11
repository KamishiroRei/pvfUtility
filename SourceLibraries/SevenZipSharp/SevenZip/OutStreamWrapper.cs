using System;
using System.IO;
using System.Runtime.InteropServices;

namespace SevenZip;

internal sealed class OutStreamWrapper : StreamWrapper, ISequentialOutStream, IOutStream
{
	public event EventHandler<IntEventArgs> BytesWritten;

	public OutStreamWrapper(Stream baseStream, string fileName, DateTime time, bool disposeStream)
		: base(baseStream, fileName, time, disposeStream)
	{
	}

	public OutStreamWrapper(Stream baseStream, bool disposeStream)
		: base(baseStream, disposeStream)
	{
	}

	public int SetSize(long newSize)
	{
		base.BaseStream.SetLength(newSize);
		return 0;
	}

	public int Write(byte[] data, uint size, IntPtr processedSize)
	{
		base.BaseStream.Write(data, 0, (int)size);
		if (processedSize != IntPtr.Zero)
		{
			Marshal.WriteInt32(processedSize, (int)size);
		}
		OnBytesWritten(new IntEventArgs((int)size));
		return 0;
	}

	private void OnBytesWritten(IntEventArgs e)
	{
		this.BytesWritten?.Invoke(this, e);
	}
}
