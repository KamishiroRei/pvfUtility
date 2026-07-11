using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;

namespace SevenZip;

internal class MultiStreamWrapper : DisposeVariableWrapper, IDisposable
{
	protected readonly Dictionary<int, KeyValuePair<long, long>> StreamOffsets = new Dictionary<int, KeyValuePair<long, long>>();

	protected readonly List<Stream> Streams = new List<Stream>();

	protected int CurrentStream;

	protected long Position;

	protected long StreamLength;

	public long Length => StreamLength;

	protected MultiStreamWrapper(bool dispose)
		: base(dispose)
	{
	}

	public virtual void Dispose()
	{
		if (base.DisposeStream)
		{
			foreach (Stream stream in Streams)
			{
				try
				{
					stream.Dispose();
				}
				catch (ObjectDisposedException)
				{
				}
			}
			Streams.Clear();
		}
		GC.SuppressFinalize(this);
	}

	protected static string VolumeNumber(int num)
	{
		if (num < 10)
		{
			return ".00" + num.ToString(CultureInfo.InvariantCulture);
		}
		if (num > 9 && num < 100)
		{
			return ".0" + num.ToString(CultureInfo.InvariantCulture);
		}
		if (num > 99 && num < 1000)
		{
			return "." + num.ToString(CultureInfo.InvariantCulture);
		}
		return string.Empty;
	}

	private int StreamNumberByOffset(long offset)
	{
		foreach (int key in StreamOffsets.Keys)
		{
			if (StreamOffsets[key].Key <= offset && StreamOffsets[key].Value >= offset)
			{
				return key;
			}
		}
		return -1;
	}

	public void Seek(long offset, SeekOrigin seekOrigin, IntPtr newPosition)
	{
		long num = seekOrigin switch
		{
			SeekOrigin.Begin => offset, 
			SeekOrigin.Current => Position + offset, 
			SeekOrigin.End => Length + offset, 
			_ => throw new ArgumentOutOfRangeException("seekOrigin"), 
		};
		CurrentStream = StreamNumberByOffset(num);
		long num2 = Streams[CurrentStream].Seek(num - StreamOffsets[CurrentStream].Key, SeekOrigin.Begin);
		Position = StreamOffsets[CurrentStream].Key + num2;
		if (newPosition != IntPtr.Zero)
		{
			Marshal.WriteInt64(newPosition, Position);
		}
	}
}
