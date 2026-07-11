using System.Collections.Generic;
using System.IO;

namespace SevenZip;

internal sealed class InMultiStreamWrapper : MultiStreamWrapper, ISequentialInStream, IInStream
{
	public InMultiStreamWrapper(string fileName, bool dispose)
		: base(dispose)
	{
		string text = fileName.Substring(0, fileName.Length - 4);
		int num = 0;
		while (File.Exists(fileName))
		{
			Streams.Add(new FileStream(fileName, FileMode.Open));
			long length = Streams[num].Length;
			StreamOffsets.Add(num++, new KeyValuePair<long, long>(StreamLength, StreamLength + length));
			StreamLength += length;
			fileName = text + MultiStreamWrapper.VolumeNumber(num + 1);
		}
	}

	public int Read(byte[] data, uint size)
	{
		int num = (int)size;
		int num2 = Streams[CurrentStream].Read(data, 0, num);
		num -= num2;
		Position += num2;
		while (num2 < (int)size)
		{
			if (CurrentStream == Streams.Count - 1)
			{
				return num2;
			}
			CurrentStream++;
			Streams[CurrentStream].Seek(0L, SeekOrigin.Begin);
			int num3 = Streams[CurrentStream].Read(data, num2, num);
			num2 += num3;
			num -= num3;
			Position += num3;
		}
		return num2;
	}
}
