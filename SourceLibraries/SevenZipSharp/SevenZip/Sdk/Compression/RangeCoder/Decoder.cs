using System.IO;

namespace SevenZip.Sdk.Compression.RangeCoder;

internal class Decoder
{
	public const uint kTopValue = 16777216u;

	public uint Code;

	public uint Range;

	public Stream Stream;

	public void Init(Stream stream)
	{
		Stream = stream;
		Code = 0u;
		Range = uint.MaxValue;
		for (int i = 0; i < 5; i++)
		{
			Code = (Code << 8) | (byte)Stream.ReadByte();
		}
	}

	public void ReleaseStream()
	{
		Stream = null;
	}

	public uint DecodeDirectBits(int numTotalBits)
	{
		uint num = Range;
		uint num2 = Code;
		uint num3 = 0u;
		for (int num4 = numTotalBits; num4 > 0; num4--)
		{
			num >>= 1;
			uint num5 = num2 - num >> 31;
			num2 -= num & (num5 - 1);
			num3 = (num3 << 1) | (1 - num5);
			if (num < 16777216)
			{
				num2 = (num2 << 8) | (byte)Stream.ReadByte();
				num <<= 8;
			}
		}
		Range = num;
		Code = num2;
		return num3;
	}
}
