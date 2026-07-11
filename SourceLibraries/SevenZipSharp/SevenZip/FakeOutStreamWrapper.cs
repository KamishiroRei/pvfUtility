using System;
using System.Runtime.InteropServices;

namespace SevenZip;

internal sealed class FakeOutStreamWrapper : ISequentialOutStream, IDisposable
{
	public event EventHandler<IntEventArgs> BytesWritten;

	public void Dispose()
	{
		GC.SuppressFinalize(this);
	}

	public int Write(byte[] data, uint size, IntPtr processedSize)
	{
		OnBytesWritten(new IntEventArgs((int)size));
		if (processedSize != IntPtr.Zero)
		{
			Marshal.WriteInt32(processedSize, (int)size);
		}
		return 0;
	}

	private void OnBytesWritten(IntEventArgs e)
	{
		this.BytesWritten?.Invoke(this, e);
	}
}
