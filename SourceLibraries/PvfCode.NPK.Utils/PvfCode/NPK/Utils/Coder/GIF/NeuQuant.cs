using System;

namespace PvfCode.NPK.Utils.Coder.GIF;

public class NeuQuant
{
	protected static readonly int netsize = 256;

	protected static readonly int prime1 = 499;

	protected static readonly int prime2 = 491;

	protected static readonly int prime3 = 487;

	protected static readonly int prime4 = 503;

	protected static readonly int minpicturebytes = 3 * prime4;

	protected static readonly int maxnetpos = netsize - 1;

	protected static readonly int netbiasshift = 4;

	protected static readonly int ncycles = 100;

	protected static readonly int intbiasshift = 16;

	protected static readonly int intbias = 1 << intbiasshift;

	protected static readonly int gammashift = 10;

	protected static readonly int gamma = 1 << gammashift;

	protected static readonly int betashift = 10;

	protected static readonly int beta = intbias >> betashift;

	protected static readonly int betagamma = intbias << gammashift - betashift;

	protected static readonly int initrad = netsize >> 3;

	protected static readonly int radiusbiasshift = 6;

	protected static readonly int radiusbias = 1 << radiusbiasshift;

	protected static readonly int initradius = initrad * radiusbias;

	protected static readonly int radiusdec = 30;

	protected static readonly int alphabiasshift = 10;

	protected static readonly int initalpha = 1 << alphabiasshift;

	protected static readonly int radbiasshift = 8;

	protected static readonly int radbias = 1 << radbiasshift;

	protected static readonly int alpharadbshift = alphabiasshift + radbiasshift;

	protected static readonly int alpharadbias = 1 << alpharadbshift;

	protected int alphadec;

	protected int[] bias = new int[netsize];

	protected int[] freq = new int[netsize];

	protected int lengthcount;

	protected int[] netindex = new int[256];

	protected int[][] network;

	protected int[] radpower = new int[initrad];

	protected int samplefac;

	protected byte[] thepicture;

	public NeuQuant(byte[] thepic, int len, int sample)
	{
		thepicture = thepic;
		lengthcount = len;
		samplefac = sample;
		network = new int[netsize][];
		for (int i = 0; i < netsize; i++)
		{
			network[i] = new int[4];
			int[] array = network[i];
			array[0] = (array[1] = (array[2] = (i << netbiasshift + 8) / netsize));
			freq[i] = intbias / netsize;
			bias[i] = 0;
		}
	}

	public byte[] ColorMap()
	{
		byte[] array = new byte[3 * netsize];
		int[] array2 = new int[netsize];
		for (int i = 0; i < netsize; i++)
		{
			array2[network[i][3]] = i;
		}
		int num = 0;
		for (int j = 0; j < netsize; j++)
		{
			int num2 = array2[j];
			array[num++] = (byte)network[num2][0];
			array[num++] = (byte)network[num2][1];
			array[num++] = (byte)network[num2][2];
		}
		return array;
	}

	public void Inxbuild()
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < netsize; i++)
		{
			int[] array = network[i];
			int num3 = i;
			int num4 = array[1];
			int[] array2;
			for (int j = i + 1; j < netsize; j++)
			{
				array2 = network[j];
				if (array2[1] < num4)
				{
					num3 = j;
					num4 = array2[1];
				}
			}
			array2 = network[num3];
			if (i != num3)
			{
				int j = array2[0];
				array2[0] = array[0];
				array[0] = j;
				j = array2[1];
				array2[1] = array[1];
				array[1] = j;
				j = array2[2];
				array2[2] = array[2];
				array[2] = j;
				j = array2[3];
				array2[3] = array[3];
				array[3] = j;
			}
			if (num4 != num)
			{
				netindex[num] = num2 + i >> 1;
				for (int j = num + 1; j < num4; j++)
				{
					netindex[j] = i;
				}
				num = num4;
				num2 = i;
			}
		}
		netindex[num] = num2 + maxnetpos >> 1;
		for (int j = num + 1; j < 256; j++)
		{
			netindex[j] = maxnetpos;
		}
	}

	public void Learn()
	{
		if (lengthcount < minpicturebytes)
		{
			samplefac = 1;
		}
		alphadec = 30 + (samplefac - 1) / 3;
		byte[] array = thepicture;
		int num = 0;
		int num2 = lengthcount;
		int num3 = lengthcount / (3 * samplefac);
		int num4 = num3 / ncycles;
		int num5 = initalpha;
		int num6 = initradius;
		int num7 = num6 >> radiusbiasshift;
		if (num7 <= 1)
		{
			num7 = 0;
		}
		int i;
		for (i = 0; i < num7; i++)
		{
			radpower[i] = num5 * ((num7 * num7 - i * i) * radbias / (num7 * num7));
		}
		int num8 = ((lengthcount < minpicturebytes) ? 3 : ((lengthcount % prime1 != 0) ? (3 * prime1) : ((lengthcount % prime2 != 0) ? (3 * prime2) : ((lengthcount % prime3 == 0) ? (3 * prime4) : (3 * prime3)))));
		i = 0;
		while (i < num3)
		{
			int b = (array[num] & 0xFF) << netbiasshift;
			int g = (array[num + 1] & 0xFF) << netbiasshift;
			int r = (array[num + 2] & 0xFF) << netbiasshift;
			int i2 = Contest(b, g, r);
			Altersingle(num5, i2, b, g, r);
			if (num7 != 0)
			{
				Alterneigh(num7, i2, b, g, r);
			}
			num += num8;
			if (num >= num2)
			{
				num -= lengthcount;
			}
			i++;
			if (num4 == 0)
			{
				num4 = 1;
			}
			if (i % num4 == 0)
			{
				num5 -= num5 / alphadec;
				num6 -= num6 / radiusdec;
				num7 = num6 >> radiusbiasshift;
				if (num7 <= 1)
				{
					num7 = 0;
				}
				for (i2 = 0; i2 < num7; i2++)
				{
					radpower[i2] = num5 * ((num7 * num7 - i2 * i2) * radbias / (num7 * num7));
				}
			}
		}
	}

	public int Map(int b, int g, int r)
	{
		int num = 1000;
		int result = -1;
		int num2 = netindex[g];
		int num3 = num2 - 1;
		while (num2 < netsize || num3 >= 0)
		{
			int[] array;
			int num5;
			int num4;
			if (num2 < netsize)
			{
				array = network[num2];
				num4 = array[1] - g;
				if (num4 >= num)
				{
					num2 = netsize;
				}
				else
				{
					num2++;
					if (num4 < 0)
					{
						num4 = -num4;
					}
					num5 = array[0] - b;
					if (num5 < 0)
					{
						num5 = -num5;
					}
					num4 += num5;
					if (num4 < num)
					{
						num5 = array[2] - r;
						if (num5 < 0)
						{
							num5 = -num5;
						}
						num4 += num5;
						if (num4 < num)
						{
							num = num4;
							result = array[3];
						}
					}
				}
			}
			if (num3 < 0)
			{
				continue;
			}
			array = network[num3];
			num4 = g - array[1];
			if (num4 >= num)
			{
				num3 = -1;
				continue;
			}
			num3--;
			if (num4 < 0)
			{
				num4 = -num4;
			}
			num5 = array[0] - b;
			if (num5 < 0)
			{
				num5 = -num5;
			}
			num4 += num5;
			if (num4 < num)
			{
				num5 = array[2] - r;
				if (num5 < 0)
				{
					num5 = -num5;
				}
				num4 += num5;
				if (num4 < num)
				{
					num = num4;
					result = array[3];
				}
			}
		}
		return result;
	}

	public byte[] Process()
	{
		Learn();
		Unbiasnet();
		Inxbuild();
		return ColorMap();
	}

	public void Unbiasnet()
	{
		for (int i = 0; i < netsize; i++)
		{
			network[i][0] >>= netbiasshift;
			network[i][1] >>= netbiasshift;
			network[i][2] >>= netbiasshift;
			network[i][3] = i;
		}
	}

	protected void Alterneigh(int rad, int i, int b, int g, int r)
	{
		int num = i - rad;
		if (num < -1)
		{
			num = -1;
		}
		int num2 = i + rad;
		if (num2 > netsize)
		{
			num2 = netsize;
		}
		int num3 = i + 1;
		int num4 = i - 1;
		int num5 = 1;
		while (num3 < num2 || num4 > num)
		{
			int num6 = radpower[num5++];
			if (num3 < num2)
			{
				int[] array = network[num3++];
				try
				{
					array[0] -= num6 * (array[0] - b) / alpharadbias;
					array[1] -= num6 * (array[1] - g) / alpharadbias;
					array[2] -= num6 * (array[2] - r) / alpharadbias;
				}
				catch (Exception)
				{
				}
			}
			if (num4 > num)
			{
				int[] array = network[num4--];
				try
				{
					array[0] -= num6 * (array[0] - b) / alpharadbias;
					array[1] -= num6 * (array[1] - g) / alpharadbias;
					array[2] -= num6 * (array[2] - r) / alpharadbias;
				}
				catch (Exception)
				{
				}
			}
		}
	}

	protected void Altersingle(int alpha, int i, int b, int g, int r)
	{
		int[] array = network[i];
		array[0] -= alpha * (array[0] - b) / initalpha;
		array[1] -= alpha * (array[1] - g) / initalpha;
		array[2] -= alpha * (array[2] - r) / initalpha;
	}

	protected int Contest(int b, int g, int r)
	{
		int num = int.MaxValue;
		int num2 = num;
		int num3 = -1;
		int result = num3;
		for (int i = 0; i < netsize; i++)
		{
			int[] obj = network[i];
			int num4 = obj[0] - b;
			if (num4 < 0)
			{
				num4 = -num4;
			}
			int num5 = obj[1] - g;
			if (num5 < 0)
			{
				num5 = -num5;
			}
			num4 += num5;
			num5 = obj[2] - r;
			if (num5 < 0)
			{
				num5 = -num5;
			}
			num4 += num5;
			if (num4 < num)
			{
				num = num4;
				num3 = i;
			}
			int num6 = num4 - (bias[i] >> intbiasshift - netbiasshift);
			if (num6 < num2)
			{
				num2 = num6;
				result = i;
			}
			int num7 = freq[i] >> betashift;
			freq[i] -= num7;
			bias[i] += num7 << gammashift;
		}
		freq[num3] += beta;
		bias[num3] -= betagamma;
		return result;
	}
}
