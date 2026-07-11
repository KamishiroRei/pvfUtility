using System;

namespace Ionic.Zlib;

internal sealed class InflateBlocks
{
	private enum InflateBlockMode
	{
		TYPE,
		LENS,
		STORED,
		TABLE,
		BTREE,
		DTREE,
		CODES,
		DRY,
		DONE,
		BAD
	}

	private const int MANY = 1440;

	internal static readonly int[] border = new int[19]
	{
		16, 17, 18, 0, 8, 7, 9, 6, 10, 5,
		11, 4, 12, 3, 13, 2, 14, 1, 15
	};

	private InflateBlockMode mode;

	internal int left;

	internal int table;

	internal int index;

	internal int[] blens;

	internal int[] bb = new int[1];

	internal int[] tb = new int[1];

	internal InflateCodes codes = new InflateCodes();

	internal int last;

	internal ZlibCodec _codec;

	internal int bitk;

	internal int bitb;

	internal int[] hufts;

	internal byte[] window;

	internal int end;

	internal int readAt;

	internal int writeAt;

	internal object checkfn;

	internal uint check;

	internal InfTree inftree = new InfTree();

	internal InflateBlocks(ZlibCodec codec, object checkfn, int w)
	{
		_codec = codec;
		hufts = new int[4320];
		window = new byte[w];
		end = w;
		this.checkfn = checkfn;
		mode = InflateBlockMode.TYPE;
		Reset();
	}

	internal uint Reset()
	{
		uint result = check;
		mode = InflateBlockMode.TYPE;
		bitk = 0;
		bitb = 0;
		readAt = (writeAt = 0);
		if (checkfn != null)
		{
			_codec._Adler32 = (check = Adler.Adler32(0u, null, 0, 0));
		}
		return result;
	}

	internal int Process(int r)
	{
		int num = _codec.NextIn;
		int num2 = _codec.AvailableBytesIn;
		int num3 = bitb;
		int i = bitk;
		int num4 = writeAt;
		int num5 = ((num4 < readAt) ? (readAt - num4 - 1) : (end - num4));
		while (true)
		{
			switch (mode)
			{
			case InflateBlockMode.TYPE:
			{
				for (; i < 3; i += 8)
				{
					if (num2 != 0)
					{
						r = 0;
						num2--;
						num3 |= (_codec.InputBuffer[num++] & 0xFF) << i;
						continue;
					}
					bitb = num3;
					bitk = i;
					_codec.AvailableBytesIn = num2;
					_codec.TotalBytesIn += num - _codec.NextIn;
					_codec.NextIn = num;
					writeAt = num4;
					return Flush(r);
				}
				int num8 = num3 & 7;
				last = num8 & 1;
				switch ((uint)(num8 >>> 1))
				{
				case 0u:
					num3 >>= 3;
					i -= 3;
					num8 = i & 7;
					num3 >>= num8;
					i -= num8;
					mode = InflateBlockMode.LENS;
					break;
				case 1u:
				{
					int[] array = new int[1];
					int[] array2 = new int[1];
					int[][] array3 = new int[1][];
					int[][] array4 = new int[1][];
					InfTree.inflate_trees_fixed(array, array2, array3, array4, _codec);
					codes.Init(array[0], array2[0], array3[0], 0, array4[0], 0);
					num3 >>= 3;
					i -= 3;
					mode = InflateBlockMode.CODES;
					break;
				}
				case 2u:
					num3 >>= 3;
					i -= 3;
					mode = InflateBlockMode.TABLE;
					break;
				case 3u:
					num3 >>= 3;
					i -= 3;
					mode = InflateBlockMode.BAD;
					_codec.Message = "invalid block type";
					r = -3;
					bitb = num3;
					bitk = i;
					_codec.AvailableBytesIn = num2;
					_codec.TotalBytesIn += num - _codec.NextIn;
					_codec.NextIn = num;
					writeAt = num4;
					return Flush(r);
				}
				break;
			}
			case InflateBlockMode.LENS:
				for (; i < 32; i += 8)
				{
					if (num2 != 0)
					{
						r = 0;
						num2--;
						num3 |= (_codec.InputBuffer[num++] & 0xFF) << i;
						continue;
					}
					bitb = num3;
					bitk = i;
					_codec.AvailableBytesIn = num2;
					_codec.TotalBytesIn += num - _codec.NextIn;
					_codec.NextIn = num;
					writeAt = num4;
					return Flush(r);
				}
				if (((~num3 >> 16) & 0xFFFF) != (num3 & 0xFFFF))
				{
					mode = InflateBlockMode.BAD;
					_codec.Message = "invalid stored block lengths";
					r = -3;
					bitb = num3;
					bitk = i;
					_codec.AvailableBytesIn = num2;
					_codec.TotalBytesIn += num - _codec.NextIn;
					_codec.NextIn = num;
					writeAt = num4;
					return Flush(r);
				}
				left = num3 & 0xFFFF;
				num3 = (i = 0);
				mode = ((left != 0) ? InflateBlockMode.STORED : ((last != 0) ? InflateBlockMode.DRY : InflateBlockMode.TYPE));
				break;
			case InflateBlockMode.STORED:
			{
				if (num2 == 0)
				{
					bitb = num3;
					bitk = i;
					_codec.AvailableBytesIn = num2;
					_codec.TotalBytesIn += num - _codec.NextIn;
					_codec.NextIn = num;
					writeAt = num4;
					return Flush(r);
				}
				if (num5 == 0)
				{
					if (num4 == end && readAt != 0)
					{
						num4 = 0;
						num5 = ((num4 < readAt) ? (readAt - num4 - 1) : (end - num4));
					}
					if (num5 == 0)
					{
						writeAt = num4;
						r = Flush(r);
						num4 = writeAt;
						num5 = ((num4 < readAt) ? (readAt - num4 - 1) : (end - num4));
						if (num4 == end && readAt != 0)
						{
							num4 = 0;
							num5 = ((num4 < readAt) ? (readAt - num4 - 1) : (end - num4));
						}
						if (num5 == 0)
						{
							bitb = num3;
							bitk = i;
							_codec.AvailableBytesIn = num2;
							_codec.TotalBytesIn += num - _codec.NextIn;
							_codec.NextIn = num;
							writeAt = num4;
							return Flush(r);
						}
					}
				}
				r = 0;
				int num7 = left;
				if (num7 > num2)
				{
					num7 = num2;
				}
				if (num7 > num5)
				{
					num7 = num5;
				}
				Array.Copy(_codec.InputBuffer, num, window, num4, num7);
				num += num7;
				num2 -= num7;
				num4 += num7;
				num5 -= num7;
				if ((left -= num7) == 0)
				{
					mode = ((last != 0) ? InflateBlockMode.DRY : InflateBlockMode.TYPE);
				}
				break;
			}
			case InflateBlockMode.TABLE:
			{
				for (; i < 14; i += 8)
				{
					if (num2 != 0)
					{
						r = 0;
						num2--;
						num3 |= (_codec.InputBuffer[num++] & 0xFF) << i;
						continue;
					}
					bitb = num3;
					bitk = i;
					_codec.AvailableBytesIn = num2;
					_codec.TotalBytesIn += num - _codec.NextIn;
					_codec.NextIn = num;
					writeAt = num4;
					return Flush(r);
				}
				int num6 = (table = num3 & 0x3FFF);
				if ((num6 & 0x1F) > 29 || ((num6 >> 5) & 0x1F) > 29)
				{
					mode = InflateBlockMode.BAD;
					_codec.Message = "too many length or distance symbols";
					r = -3;
					bitb = num3;
					bitk = i;
					_codec.AvailableBytesIn = num2;
					_codec.TotalBytesIn += num - _codec.NextIn;
					_codec.NextIn = num;
					writeAt = num4;
					return Flush(r);
				}
				num6 = 258 + (num6 & 0x1F) + ((num6 >> 5) & 0x1F);
				if (blens == null || blens.Length < num6)
				{
					blens = new int[num6];
				}
				else
				{
					Array.Clear(blens, 0, num6);
				}
				num3 >>= 14;
				i -= 14;
				index = 0;
				mode = InflateBlockMode.BTREE;
				goto case InflateBlockMode.BTREE;
			}
			case InflateBlockMode.BTREE:
			{
				while (index < 4 + (table >> 10))
				{
					for (; i < 3; i += 8)
					{
						if (num2 != 0)
						{
							r = 0;
							num2--;
							num3 |= (_codec.InputBuffer[num++] & 0xFF) << i;
							continue;
						}
						bitb = num3;
						bitk = i;
						_codec.AvailableBytesIn = num2;
						_codec.TotalBytesIn += num - _codec.NextIn;
						_codec.NextIn = num;
						writeAt = num4;
						return Flush(r);
					}
					blens[border[index++]] = num3 & 7;
					num3 >>= 3;
					i -= 3;
				}
				while (index < 19)
				{
					blens[border[index++]] = 0;
				}
				bb[0] = 7;
				int num13 = inftree.inflate_trees_bits(blens, bb, tb, hufts, _codec);
				if (num13 != 0)
				{
					r = num13;
					if (r == -3)
					{
						blens = null;
						mode = InflateBlockMode.BAD;
					}
					bitb = num3;
					bitk = i;
					_codec.AvailableBytesIn = num2;
					_codec.TotalBytesIn += num - _codec.NextIn;
					_codec.NextIn = num;
					writeAt = num4;
					return Flush(r);
				}
				index = 0;
				mode = InflateBlockMode.DTREE;
				goto case InflateBlockMode.DTREE;
			}
			case InflateBlockMode.DTREE:
			{
				int num9;
				while (true)
				{
					num9 = table;
					if (index >= 258 + (num9 & 0x1F) + ((num9 >> 5) & 0x1F))
					{
						break;
					}
					for (num9 = bb[0]; i < num9; i += 8)
					{
						if (num2 != 0)
						{
							r = 0;
							num2--;
							num3 |= (_codec.InputBuffer[num++] & 0xFF) << i;
							continue;
						}
						bitb = num3;
						bitk = i;
						_codec.AvailableBytesIn = num2;
						_codec.TotalBytesIn += num - _codec.NextIn;
						_codec.NextIn = num;
						writeAt = num4;
						return Flush(r);
					}
					num9 = hufts[(tb[0] + (num3 & InternalInflateConstants.InflateMask[num9])) * 3 + 1];
					int num10 = hufts[(tb[0] + (num3 & InternalInflateConstants.InflateMask[num9])) * 3 + 2];
					if (num10 < 16)
					{
						num3 >>= num9;
						i -= num9;
						blens[index++] = num10;
						continue;
					}
					int num11 = ((num10 == 18) ? 7 : (num10 - 14));
					int num12 = ((num10 == 18) ? 11 : 3);
					for (; i < num9 + num11; i += 8)
					{
						if (num2 != 0)
						{
							r = 0;
							num2--;
							num3 |= (_codec.InputBuffer[num++] & 0xFF) << i;
							continue;
						}
						bitb = num3;
						bitk = i;
						_codec.AvailableBytesIn = num2;
						_codec.TotalBytesIn += num - _codec.NextIn;
						_codec.NextIn = num;
						writeAt = num4;
						return Flush(r);
					}
					num3 >>= num9;
					i -= num9;
					num12 += num3 & InternalInflateConstants.InflateMask[num11];
					num3 >>= num11;
					i -= num11;
					num11 = index;
					num9 = table;
					if (num11 + num12 > 258 + (num9 & 0x1F) + ((num9 >> 5) & 0x1F) || (num10 == 16 && num11 < 1))
					{
						blens = null;
						mode = InflateBlockMode.BAD;
						_codec.Message = "invalid bit length repeat";
						r = -3;
						bitb = num3;
						bitk = i;
						_codec.AvailableBytesIn = num2;
						_codec.TotalBytesIn += num - _codec.NextIn;
						_codec.NextIn = num;
						writeAt = num4;
						return Flush(r);
					}
					num10 = ((num10 == 16) ? blens[num11 - 1] : 0);
					do
					{
						blens[num11++] = num10;
					}
					while (--num12 != 0);
					index = num11;
				}
				tb[0] = -1;
				int[] array5 = new int[1] { 9 };
				int[] array6 = new int[1] { 6 };
				int[] array7 = new int[1];
				int[] array8 = new int[1];
				num9 = table;
				num9 = inftree.inflate_trees_dynamic(257 + (num9 & 0x1F), 1 + ((num9 >> 5) & 0x1F), blens, array5, array6, array7, array8, hufts, _codec);
				if (num9 != 0)
				{
					if (num9 == -3)
					{
						blens = null;
						mode = InflateBlockMode.BAD;
					}
					r = num9;
					bitb = num3;
					bitk = i;
					_codec.AvailableBytesIn = num2;
					_codec.TotalBytesIn += num - _codec.NextIn;
					_codec.NextIn = num;
					writeAt = num4;
					return Flush(r);
				}
				codes.Init(array5[0], array6[0], hufts, array7[0], hufts, array8[0]);
				mode = InflateBlockMode.CODES;
				goto case InflateBlockMode.CODES;
			}
			case InflateBlockMode.CODES:
				bitb = num3;
				bitk = i;
				_codec.AvailableBytesIn = num2;
				_codec.TotalBytesIn += num - _codec.NextIn;
				_codec.NextIn = num;
				writeAt = num4;
				r = codes.Process(this, r);
				if (r != 1)
				{
					return Flush(r);
				}
				r = 0;
				num = _codec.NextIn;
				num2 = _codec.AvailableBytesIn;
				num3 = bitb;
				i = bitk;
				num4 = writeAt;
				num5 = ((num4 < readAt) ? (readAt - num4 - 1) : (end - num4));
				if (last == 0)
				{
					mode = InflateBlockMode.TYPE;
					break;
				}
				mode = InflateBlockMode.DRY;
				goto case InflateBlockMode.DRY;
			case InflateBlockMode.DRY:
				writeAt = num4;
				r = Flush(r);
				num4 = writeAt;
				if (num4 >= readAt)
				{
					_ = end - num4;
				}
				else
				{
					_ = readAt - num4 - 1;
				}
				if (readAt != writeAt)
				{
					bitb = num3;
					bitk = i;
					_codec.AvailableBytesIn = num2;
					_codec.TotalBytesIn += num - _codec.NextIn;
					_codec.NextIn = num;
					writeAt = num4;
					return Flush(r);
				}
				mode = InflateBlockMode.DONE;
				goto case InflateBlockMode.DONE;
			case InflateBlockMode.DONE:
				r = 1;
				bitb = num3;
				bitk = i;
				_codec.AvailableBytesIn = num2;
				_codec.TotalBytesIn += num - _codec.NextIn;
				_codec.NextIn = num;
				writeAt = num4;
				return Flush(r);
			case InflateBlockMode.BAD:
				r = -3;
				bitb = num3;
				bitk = i;
				_codec.AvailableBytesIn = num2;
				_codec.TotalBytesIn += num - _codec.NextIn;
				_codec.NextIn = num;
				writeAt = num4;
				return Flush(r);
			default:
				r = -2;
				bitb = num3;
				bitk = i;
				_codec.AvailableBytesIn = num2;
				_codec.TotalBytesIn += num - _codec.NextIn;
				_codec.NextIn = num;
				writeAt = num4;
				return Flush(r);
			}
		}
	}

	internal void Free()
	{
		Reset();
		window = null;
		hufts = null;
	}

	internal void SetDictionary(byte[] d, int start, int n)
	{
		Array.Copy(d, start, window, 0, n);
		readAt = (writeAt = n);
	}

	internal int SyncPoint()
	{
		return (mode == InflateBlockMode.LENS) ? 1 : 0;
	}

	internal int Flush(int r)
	{
		for (int i = 0; i < 2; i++)
		{
			int num = ((i != 0) ? (writeAt - readAt) : (((readAt <= writeAt) ? writeAt : end) - readAt));
			if (num == 0)
			{
				if (r == -5)
				{
					r = 0;
				}
				return r;
			}
			if (num > _codec.AvailableBytesOut)
			{
				num = _codec.AvailableBytesOut;
			}
			if (num != 0 && r == -5)
			{
				r = 0;
			}
			_codec.AvailableBytesOut -= num;
			_codec.TotalBytesOut += num;
			if (checkfn != null)
			{
				_codec._Adler32 = (check = Adler.Adler32(check, window, readAt, num));
			}
			Array.Copy(window, readAt, _codec.OutputBuffer, _codec.NextOut, num);
			_codec.NextOut += num;
			readAt += num;
			if (readAt == end && i == 0)
			{
				readAt = 0;
				if (writeAt == end)
				{
					writeAt = 0;
				}
			}
			else
			{
				i++;
			}
		}
		return r;
	}
}
