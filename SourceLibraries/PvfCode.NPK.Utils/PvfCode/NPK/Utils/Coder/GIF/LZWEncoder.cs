using System;
using System.IO;

namespace PvfCode.NPK.Utils.Coder.GIF;

public class LZWEncoder
{
	private static readonly int EOF = -1;

	private static readonly int BITS = 12;

	private static readonly int HSIZE = 5003;

	private readonly byte[] accum = new byte[256];

	private readonly int[] codetab = new int[HSIZE];

	private readonly int hsize = HSIZE;

	private readonly int[] htab = new int[HSIZE];

	private readonly int imgH;

	private readonly int imgW;

	private readonly int initCodeSize;

	private readonly int[] masks = new int[17]
	{
		0, 1, 3, 7, 15, 31, 63, 127, 255, 511,
		1023, 2047, 4095, 8191, 16383, 32767, 65535
	};

	private readonly int maxbits = BITS;

	private readonly int maxmaxcode = 1 << BITS;

	private readonly byte[] pixAry;

	private int a_count;

	private bool clear_flg;

	private int ClearCode;

	private int cur_accum;

	private int cur_bits;

	private int curPixel;

	private int EOFCode;

	private int free_ent;

	private int g_init_bits;

	private int maxcode;

	private int n_bits;

	private int remaining;

	public LZWEncoder(int width, int height, byte[] pixels, int color_depth)
	{
		imgW = width;
		imgH = height;
		pixAry = pixels;
		initCodeSize = Math.Max(2, color_depth);
	}

	private void Add(byte c, Stream outs)
	{
		accum[a_count++] = c;
		if (a_count > 254)
		{
			Flush(outs);
		}
	}

	private void ClearTable(Stream outs)
	{
		ResetCodeTable(hsize);
		free_ent = ClearCode + 2;
		clear_flg = true;
		Output(ClearCode, outs);
	}

	private void ResetCodeTable(int hsize)
	{
		for (int i = 0; i < hsize; i++)
		{
			htab[i] = -1;
		}
	}

	private void Compress(int init_bits, Stream outs)
	{
		g_init_bits = init_bits;
		clear_flg = false;
		n_bits = g_init_bits;
		maxcode = MaxCode(n_bits);
		ClearCode = 1 << init_bits - 1;
		EOFCode = ClearCode + 1;
		free_ent = ClearCode + 2;
		a_count = 0;
		int num = NextPixel();
		int num2 = 0;
		for (int num3 = hsize; num3 < 65536; num3 *= 2)
		{
			num2++;
		}
		num2 = 8 - num2;
		int num4 = hsize;
		ResetCodeTable(num4);
		Output(ClearCode, outs);
		int num5;
		while ((num5 = NextPixel()) != EOF)
		{
			int num3 = (num5 << maxbits) + num;
			int num6 = (num5 << num2) ^ num;
			if (htab[num6] == num3)
			{
				num = codetab[num6];
				continue;
			}
			if (htab[num6] >= 0)
			{
				int num7 = num4 - num6;
				if (num6 == 0)
				{
					num7 = 1;
				}
				while (true)
				{
					if ((num6 -= num7) < 0)
					{
						num6 += num4;
					}
					if (htab[num6] == num3)
					{
						break;
					}
					if (htab[num6] >= 0)
					{
						continue;
					}
					goto IL_0121;
				}
				num = codetab[num6];
				continue;
			}
			goto IL_0121;
			IL_0121:
			Output(num, outs);
			num = num5;
			if (free_ent < maxmaxcode)
			{
				codetab[num6] = free_ent++;
				htab[num6] = num3;
			}
			else
			{
				ClearTable(outs);
			}
		}
		Output(num, outs);
		Output(EOFCode, outs);
	}

	public void Encode(Stream os)
	{
		os.WriteByte(Convert.ToByte(initCodeSize));
		remaining = imgW * imgH;
		curPixel = 0;
		Compress(initCodeSize + 1, os);
		os.WriteByte(0);
	}

	private void Flush(Stream outs)
	{
		if (a_count > 0)
		{
			outs.WriteByte(Convert.ToByte(a_count));
			outs.Write(accum, 0, a_count);
			a_count = 0;
		}
	}

	private int MaxCode(int n_bits)
	{
		return (1 << n_bits) - 1;
	}

	private int NextPixel()
	{
		int upperBound = pixAry.GetUpperBound(0);
		if (curPixel > upperBound)
		{
			return EOF;
		}
		return pixAry[curPixel++] & 0xFF;
	}

	private void Output(int code, Stream outs)
	{
		cur_accum &= masks[cur_bits];
		if (cur_bits > 0)
		{
			cur_accum |= code << cur_bits;
		}
		else
		{
			cur_accum = code;
		}
		cur_bits += n_bits;
		while (cur_bits >= 8)
		{
			Add((byte)(cur_accum & 0xFF), outs);
			cur_accum >>= 8;
			cur_bits -= 8;
		}
		if (free_ent > maxcode || clear_flg)
		{
			if (clear_flg)
			{
				maxcode = MaxCode(n_bits = g_init_bits);
				clear_flg = false;
			}
			else
			{
				n_bits++;
				if (n_bits == maxbits)
				{
					maxcode = maxmaxcode;
				}
				else
				{
					maxcode = MaxCode(n_bits);
				}
			}
		}
		if (code == EOFCode)
		{
			while (cur_bits > 0)
			{
				Add((byte)(cur_accum & 0xFF), outs);
				cur_accum >>= 8;
				cur_bits -= 8;
			}
			Flush(outs);
		}
	}
}
