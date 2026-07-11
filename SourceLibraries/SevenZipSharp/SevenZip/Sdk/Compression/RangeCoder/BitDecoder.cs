namespace SevenZip.Sdk.Compression.RangeCoder;

internal struct BitDecoder
{
	public const uint kBitModelTotal = 2048u;

	public const int kNumBitModelTotalBits = 11;

	private const int kNumMoveBits = 5;

	private uint Prob;

	public void Init()
	{
		Prob = 1024u;
	}

	public uint Decode(Decoder rangeDecoder)
	{
		uint num = (rangeDecoder.Range >> 11) * Prob;
		if (rangeDecoder.Code < num)
		{
			rangeDecoder.Range = num;
			Prob += 2048 - Prob >> 5;
			if (rangeDecoder.Range < 16777216)
			{
				rangeDecoder.Code = (rangeDecoder.Code << 8) | (byte)rangeDecoder.Stream.ReadByte();
				rangeDecoder.Range <<= 8;
			}
			return 0u;
		}
		rangeDecoder.Range -= num;
		rangeDecoder.Code -= num;
		Prob -= Prob >> 5;
		if (rangeDecoder.Range < 16777216)
		{
			rangeDecoder.Code = (rangeDecoder.Code << 8) | (byte)rangeDecoder.Stream.ReadByte();
			rangeDecoder.Range <<= 8;
		}
		return 1u;
	}
}
