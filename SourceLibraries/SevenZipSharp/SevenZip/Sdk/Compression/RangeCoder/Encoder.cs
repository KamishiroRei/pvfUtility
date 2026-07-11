using System.IO;

namespace SevenZip.Sdk.Compression.RangeCoder;

internal class Encoder
{
	public const uint kTopValue = 16777216u;

	private byte _cache;

	private uint _cacheSize;

	public ulong Low;

	public uint Range;

	private long StartPosition;

	private Stream Stream;

	public void SetStream(Stream stream)
	{
		Stream = stream;
	}

	public void ReleaseStream()
	{
		Stream = null;
	}

	public void Init()
	{
		StartPosition = Stream.Position;
		Low = 0uL;
		Range = uint.MaxValue;
		_cacheSize = 1u;
		_cache = 0;
	}

	public void FlushData()
	{
		for (int i = 0; i < 5; i++)
		{
			ShiftLow();
		}
	}

	public void FlushStream()
	{
		Stream.Flush();
	}

	public void ShiftLow()
	{
		if ((uint)Low < 4278190080u || (int)(Low >> 32) == 1)
		{
			byte b = _cache;
			do
			{
				Stream.WriteByte((byte)(b + (Low >> 32)));
				b = byte.MaxValue;
			}
			while (--_cacheSize != 0);
			_cache = (byte)((uint)Low >> 24);
		}
		_cacheSize++;
		Low = (uint)((int)Low << 8);
	}

	public void EncodeDirectBits(uint v, int numTotalBits)
	{
		for (int num = numTotalBits - 1; num >= 0; num--)
		{
			Range >>= 1;
			if (((v >> num) & 1) == 1)
			{
				Low += Range;
			}
			if (Range < 16777216)
			{
				Range <<= 8;
				ShiftLow();
			}
		}
	}

	public long GetProcessedSizeAdd()
	{
		return _cacheSize + Stream.Position - StartPosition + 4;
	}
}
