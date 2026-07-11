using System;

namespace Ionic.Zlib;

internal sealed class Tree
{
	private static readonly int HEAP_SIZE = 2 * InternalConstants.L_CODES + 1;

	internal static readonly int[] ExtraLengthBits = new int[29]
	{
		0, 0, 0, 0, 0, 0, 0, 0, 1, 1,
		1, 1, 2, 2, 2, 2, 3, 3, 3, 3,
		4, 4, 4, 4, 5, 5, 5, 5, 0
	};

	internal static readonly int[] ExtraDistanceBits = new int[30]
	{
		0, 0, 0, 0, 1, 1, 2, 2, 3, 3,
		4, 4, 5, 5, 6, 6, 7, 7, 8, 8,
		9, 9, 10, 10, 11, 11, 12, 12, 13, 13
	};

	internal static readonly int[] extra_blbits = new int[19]
	{
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 2, 3, 7
	};

	internal static readonly sbyte[] bl_order = new sbyte[19]
	{
		16, 17, 18, 0, 8, 7, 9, 6, 10, 5,
		11, 4, 12, 3, 13, 2, 14, 1, 15
	};

	internal const int Buf_size = 16;

	private static readonly sbyte[] _dist_code = new sbyte[512]
	{
		0, 1, 2, 3, 4, 4, 5, 5, 6, 6,
		6, 6, 7, 7, 7, 7, 8, 8, 8, 8,
		8, 8, 8, 8, 9, 9, 9, 9, 9, 9,
		9, 9, 10, 10, 10, 10, 10, 10, 10, 10,
		10, 10, 10, 10, 10, 10, 10, 10, 11, 11,
		11, 11, 11, 11, 11, 11, 11, 11, 11, 11,
		11, 11, 11, 11, 12, 12, 12, 12, 12, 12,
		12, 12, 12, 12, 12, 12, 12, 12, 12, 12,
		12, 12, 12, 12, 12, 12, 12, 12, 12, 12,
		12, 12, 12, 12, 12, 12, 13, 13, 13, 13,
		13, 13, 13, 13, 13, 13, 13, 13, 13, 13,
		13, 13, 13, 13, 13, 13, 13, 13, 13, 13,
		13, 13, 13, 13, 13, 13, 13, 13, 14, 14,
		14, 14, 14, 14, 14, 14, 14, 14, 14, 14,
		14, 14, 14, 14, 14, 14, 14, 14, 14, 14,
		14, 14, 14, 14, 14, 14, 14, 14, 14, 14,
		14, 14, 14, 14, 14, 14, 14, 14, 14, 14,
		14, 14, 14, 14, 14, 14, 14, 14, 14, 14,
		14, 14, 14, 14, 14, 14, 14, 14, 14, 14,
		14, 14, 15, 15, 15, 15, 15, 15, 15, 15,
		15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
		15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
		15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
		15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
		15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
		15, 15, 15, 15, 15, 15, 0, 0, 16, 17,
		18, 18, 19, 19, 20, 20, 20, 20, 21, 21,
		21, 21, 22, 22, 22, 22, 22, 22, 22, 22,
		23, 23, 23, 23, 23, 23, 23, 23, 24, 24,
		24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
		24, 24, 24, 24, 25, 25, 25, 25, 25, 25,
		25, 25, 25, 25, 25, 25, 25, 25, 25, 25,
		26, 26, 26, 26, 26, 26, 26, 26, 26, 26,
		26, 26, 26, 26, 26, 26, 26, 26, 26, 26,
		26, 26, 26, 26, 26, 26, 26, 26, 26, 26,
		26, 26, 27, 27, 27, 27, 27, 27, 27, 27,
		27, 27, 27, 27, 27, 27, 27, 27, 27, 27,
		27, 27, 27, 27, 27, 27, 27, 27, 27, 27,
		27, 27, 27, 27, 28, 28, 28, 28, 28, 28,
		28, 28, 28, 28, 28, 28, 28, 28, 28, 28,
		28, 28, 28, 28, 28, 28, 28, 28, 28, 28,
		28, 28, 28, 28, 28, 28, 28, 28, 28, 28,
		28, 28, 28, 28, 28, 28, 28, 28, 28, 28,
		28, 28, 28, 28, 28, 28, 28, 28, 28, 28,
		28, 28, 28, 28, 28, 28, 28, 28, 29, 29,
		29, 29, 29, 29, 29, 29, 29, 29, 29, 29,
		29, 29, 29, 29, 29, 29, 29, 29, 29, 29,
		29, 29, 29, 29, 29, 29, 29, 29, 29, 29,
		29, 29, 29, 29, 29, 29, 29, 29, 29, 29,
		29, 29, 29, 29, 29, 29, 29, 29, 29, 29,
		29, 29, 29, 29, 29, 29, 29, 29, 29, 29,
		29, 29
	};

	internal static readonly sbyte[] LengthCode = new sbyte[256]
	{
		0, 1, 2, 3, 4, 5, 6, 7, 8, 8,
		9, 9, 10, 10, 11, 11, 12, 12, 12, 12,
		13, 13, 13, 13, 14, 14, 14, 14, 15, 15,
		15, 15, 16, 16, 16, 16, 16, 16, 16, 16,
		17, 17, 17, 17, 17, 17, 17, 17, 18, 18,
		18, 18, 18, 18, 18, 18, 19, 19, 19, 19,
		19, 19, 19, 19, 20, 20, 20, 20, 20, 20,
		20, 20, 20, 20, 20, 20, 20, 20, 20, 20,
		21, 21, 21, 21, 21, 21, 21, 21, 21, 21,
		21, 21, 21, 21, 21, 21, 22, 22, 22, 22,
		22, 22, 22, 22, 22, 22, 22, 22, 22, 22,
		22, 22, 23, 23, 23, 23, 23, 23, 23, 23,
		23, 23, 23, 23, 23, 23, 23, 23, 24, 24,
		24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
		24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
		24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
		25, 25, 25, 25, 25, 25, 25, 25, 25, 25,
		25, 25, 25, 25, 25, 25, 25, 25, 25, 25,
		25, 25, 25, 25, 25, 25, 25, 25, 25, 25,
		25, 25, 26, 26, 26, 26, 26, 26, 26, 26,
		26, 26, 26, 26, 26, 26, 26, 26, 26, 26,
		26, 26, 26, 26, 26, 26, 26, 26, 26, 26,
		26, 26, 26, 26, 27, 27, 27, 27, 27, 27,
		27, 27, 27, 27, 27, 27, 27, 27, 27, 27,
		27, 27, 27, 27, 27, 27, 27, 27, 27, 27,
		27, 27, 27, 27, 27, 28
	};

	internal static readonly int[] LengthBase = new int[29]
	{
		0, 1, 2, 3, 4, 5, 6, 7, 8, 10,
		12, 14, 16, 20, 24, 28, 32, 40, 48, 56,
		64, 80, 96, 112, 128, 160, 192, 224, 0
	};

	internal static readonly int[] DistanceBase = new int[30]
	{
		0, 1, 2, 3, 4, 6, 8, 12, 16, 24,
		32, 48, 64, 96, 128, 192, 256, 384, 512, 768,
		1024, 1536, 2048, 3072, 4096, 6144, 8192, 12288, 16384, 24576
	};

	internal short[] dyn_tree;

	internal int max_code;

	internal StaticTree staticTree;

	internal static int DistanceCode(int dist)
	{
		if (dist >= 256)
		{
			return _dist_code[256 + SharedUtils.URShift(dist, 7)];
		}
		return _dist_code[dist];
	}

	internal void gen_bitlen(DeflateManager s)
	{
		short[] array = dyn_tree;
		short[] treeCodes = staticTree.treeCodes;
		int[] extraBits = staticTree.extraBits;
		int extraBase = staticTree.extraBase;
		int maxLength = staticTree.maxLength;
		int num = 0;
		for (int i = 0; i <= InternalConstants.MAX_BITS; i++)
		{
			s.bl_count[i] = 0;
		}
		array[s.heap[s.heap_max] * 2 + 1] = 0;
		int j;
		for (j = s.heap_max + 1; j < HEAP_SIZE; j++)
		{
			int num2 = s.heap[j];
			int num3 = array[array[num2 * 2 + 1] * 2 + 1] + 1;
			if (num3 > maxLength)
			{
				num3 = maxLength;
				num++;
			}
			array[num2 * 2 + 1] = (short)num3;
			if (num2 <= max_code)
			{
				s.bl_count[num3]++;
				int num4 = 0;
				if (num2 >= extraBase)
				{
					num4 = extraBits[num2 - extraBase];
				}
				short num5 = array[num2 * 2];
				s.opt_len += num5 * (num3 + num4);
				if (treeCodes != null)
				{
					s.static_len += num5 * (treeCodes[num2 * 2 + 1] + num4);
				}
			}
		}
		if (num == 0)
		{
			return;
		}
		do
		{
			int num6 = maxLength - 1;
			while (s.bl_count[num6] == 0)
			{
				num6--;
			}
			s.bl_count[num6]--;
			s.bl_count[num6 + 1] = (short)(s.bl_count[num6 + 1] + 2);
			s.bl_count[maxLength]--;
			num -= 2;
		}
		while (num > 0);
		for (int num7 = maxLength; num7 != 0; num7--)
		{
			int num8 = s.bl_count[num7];
			while (num8 != 0)
			{
				int num9 = s.heap[--j];
				if (num9 <= max_code)
				{
					if (array[num9 * 2 + 1] != num7)
					{
						s.opt_len = (int)(s.opt_len + ((long)num7 - (long)array[num9 * 2 + 1]) * array[num9 * 2]);
						array[num9 * 2 + 1] = (short)num7;
					}
					num8--;
				}
			}
		}
	}

	internal void build_tree(DeflateManager s)
	{
		short[] array = dyn_tree;
		short[] treeCodes = staticTree.treeCodes;
		int elems = staticTree.elems;
		int num = -1;
		s.heap_len = 0;
		s.heap_max = HEAP_SIZE;
		for (int i = 0; i < elems; i++)
		{
			if (array[i * 2] != 0)
			{
				num = (s.heap[++s.heap_len] = i);
				s.depth[i] = 0;
			}
			else
			{
				array[i * 2 + 1] = 0;
			}
		}
		int num2;
		while (s.heap_len < 2)
		{
			num2 = (s.heap[++s.heap_len] = ((num < 2) ? (++num) : 0));
			array[num2 * 2] = 1;
			s.depth[num2] = 0;
			s.opt_len--;
			if (treeCodes != null)
			{
				s.static_len -= treeCodes[num2 * 2 + 1];
			}
		}
		max_code = num;
		for (int num3 = s.heap_len / 2; num3 >= 1; num3--)
		{
			s.pqdownheap(array, num3);
		}
		num2 = elems;
		do
		{
			int num4 = s.heap[1];
			s.heap[1] = s.heap[s.heap_len--];
			s.pqdownheap(array, 1);
			int num5 = s.heap[1];
			s.heap[--s.heap_max] = num4;
			s.heap[--s.heap_max] = num5;
			array[num2 * 2] = (short)(array[num4 * 2] + array[num5 * 2]);
			s.depth[num2] = (sbyte)(Math.Max((byte)s.depth[num4], (byte)s.depth[num5]) + 1);
			array[num4 * 2 + 1] = (array[num5 * 2 + 1] = (short)num2);
			s.heap[1] = num2++;
			s.pqdownheap(array, 1);
		}
		while (s.heap_len >= 2);
		s.heap[--s.heap_max] = s.heap[1];
		gen_bitlen(s);
		gen_codes(array, num, s.bl_count);
	}

	internal static void gen_codes(short[] tree, int max_code, short[] bl_count)
	{
		short[] array = new short[InternalConstants.MAX_BITS + 1];
		short num = 0;
		for (int i = 1; i <= InternalConstants.MAX_BITS; i++)
		{
			num = (array[i] = (short)(num + bl_count[i - 1] << 1));
		}
		for (int j = 0; j <= max_code; j++)
		{
			int num2 = tree[j * 2 + 1];
			if (num2 != 0)
			{
				tree[j * 2] = (short)bi_reverse(array[num2]++, num2);
			}
		}
	}

	internal static int bi_reverse(int code, int len)
	{
		int num = 0;
		do
		{
			num |= code & 1;
			code >>= 1;
			num <<= 1;
		}
		while (--len > 0);
		return num >> 1;
	}
}
