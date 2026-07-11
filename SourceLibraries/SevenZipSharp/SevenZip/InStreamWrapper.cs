using System;
using System.IO;

namespace SevenZip;

internal sealed class InStreamWrapper : StreamWrapper, ISequentialInStream, IInStream
{
	public event EventHandler<IntEventArgs> BytesRead;

	public InStreamWrapper(Stream baseStream, bool disposeStream)
		: base(baseStream, disposeStream)
	{
	}

	public int Read(byte[] data, uint size)
	{
		int num = 0;
		if (base.BaseStream != null)
		{
			num = base.BaseStream.Read(data, 0, (int)size);
			if (num > 0)
			{
				OnBytesRead(new IntEventArgs(num));
			}
		}
		return num;
	}

	private void OnBytesRead(IntEventArgs e)
	{
		this.BytesRead?.Invoke(this, e);
	}
}
