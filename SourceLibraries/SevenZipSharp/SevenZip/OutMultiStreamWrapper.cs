using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace SevenZip;

internal sealed class OutMultiStreamWrapper : MultiStreamWrapper, ISequentialOutStream, IOutStream
{
	private readonly string _archiveName;

	private readonly long _volumeSize;

	private long _overallLength;

	public OutMultiStreamWrapper(string archiveName, long volumeSize)
		: base(dispose: true)
	{
		_archiveName = archiveName;
		_volumeSize = volumeSize;
		CurrentStream = -1;
		NewVolumeStream();
	}

	public int SetSize(long newSize)
	{
		return 0;
	}

	public int Write(byte[] data, uint size, IntPtr processedSize)
	{
		int num = 0;
		int val = (int)size;
		Position += size;
		_overallLength = Math.Max(Position + 1, _overallLength);
		while (size > _volumeSize - Streams[CurrentStream].Position)
		{
			int num2 = (int)(_volumeSize - Streams[CurrentStream].Position);
			Streams[CurrentStream].Write(data, num, num2);
			size -= (uint)num2;
			num += num2;
			NewVolumeStream();
		}
		Streams[CurrentStream].Write(data, num, (int)size);
		if (processedSize != IntPtr.Zero)
		{
			Marshal.WriteInt32(processedSize, val);
		}
		return 0;
	}

	public override void Dispose()
	{
		int num = Streams.Count - 1;
		Streams[num].SetLength((num > 0) ? Streams[num].Position : _overallLength);
		base.Dispose();
	}

	private void NewVolumeStream()
	{
		CurrentStream++;
		Streams.Add(File.Create(_archiveName + MultiStreamWrapper.VolumeNumber(CurrentStream + 1)));
		Streams[CurrentStream].SetLength(_volumeSize);
		StreamOffsets.Add(CurrentStream, new KeyValuePair<long, long>(0L, _volumeSize - 1));
	}
}
