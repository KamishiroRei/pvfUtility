using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace PvfCode.ViewModels.Game;

public class BigInteger
{
	private static int DAN2pw1pwE;

	public static readonly int[] primesBelow2000;

	private uint[] A7G2UZpiBV;

	public int dataLength;

	public BigInteger()
	{
		A7G2UZpiBV = new uint[255];
		dataLength = 255;
	}

	public BigInteger(long value)
	{
		A7G2UZpiBV = new uint[DAN2pw1pwE];
		long num = value;
		dataLength = 0;
		while (value != 0L && dataLength < DAN2pw1pwE)
		{
			A7G2UZpiBV[dataLength] = (uint)(value & 0xFFFFFFFFu);
			value >>= 32;
			dataLength++;
		}
		if (num > 0)
		{
			if (value != 0L || (A7G2UZpiBV[DAN2pw1pwE - 1] & 0x80000000u) != 0)
			{
				throw new ArithmeticException("Positive overflow in constructor.");
			}
		}
		else if (num < 0 && (value != -1 || (A7G2UZpiBV[dataLength - 1] & 0x80000000u) == 0))
		{
			throw new ArithmeticException("Negative underflow in constructor.");
		}
		if (dataLength == 0)
		{
			dataLength = 1;
		}
	}

	public BigInteger(ulong value)
	{
		A7G2UZpiBV = new uint[DAN2pw1pwE];
		dataLength = 0;
		while (value != 0L && dataLength < DAN2pw1pwE)
		{
			A7G2UZpiBV[dataLength] = (uint)(value & 0xFFFFFFFFu);
			value >>= 32;
			dataLength++;
		}
		if (value != 0L || (A7G2UZpiBV[DAN2pw1pwE - 1] & 0x80000000u) != 0)
		{
			throw new ArithmeticException("Positive overflow in constructor.");
		}
		if (dataLength == 0)
		{
			dataLength = 1;
		}
	}

	public BigInteger(BigInteger bi)
	{
		A7G2UZpiBV = new uint[DAN2pw1pwE];
		dataLength = bi.dataLength;
		for (int i = 0; i < dataLength; i++)
		{
			A7G2UZpiBV[i] = bi.A7G2UZpiBV[i];
		}
	}

	public BigInteger(string value, int radix)
	{
		BigInteger bigInteger = new BigInteger(1L);
		BigInteger bigInteger2 = new BigInteger();
		value = value.ToUpper().Trim();
		int num = 0;
		if (value[0] == '-')
		{
			num = 1;
		}
		for (int num2 = value.Length - 1; num2 >= num; num2--)
		{
			int num3 = value[num2];
			num3 = ((num3 >= 48 && num3 <= 57) ? (num3 - 48) : ((num3 < 65 || num3 > 90) ? 9999999 : (num3 - 65 + 10)));
			if (num3 >= radix)
			{
				throw new ArithmeticException("Invalid string in constructor.");
			}
			if (value[0] == '-')
			{
				num3 = -num3;
			}
			bigInteger2 += bigInteger * num3;
			if (num2 - 1 >= num)
			{
				bigInteger *= (BigInteger)radix;
			}
		}
		if (value[0] == '-')
		{
			if ((bigInteger2.A7G2UZpiBV[DAN2pw1pwE - 1] & 0x80000000u) == 0)
			{
				throw new ArithmeticException("Negative underflow in constructor.");
			}
		}
		else if ((bigInteger2.A7G2UZpiBV[DAN2pw1pwE - 1] & 0x80000000u) != 0)
		{
			throw new ArithmeticException("Positive overflow in constructor.");
		}
		A7G2UZpiBV = new uint[DAN2pw1pwE];
		for (int i = 0; i < bigInteger2.dataLength; i++)
		{
			A7G2UZpiBV[i] = bigInteger2.A7G2UZpiBV[i];
		}
		dataLength = bigInteger2.dataLength;
	}

	public BigInteger(IList<byte> inData, int length = -1, int offset = 0)
	{
		int num = ((length == -1) ? (inData.Count - offset) : length);
		dataLength = num >> 2;
		int num2 = num & 3;
		if (num2 != 0)
		{
			dataLength++;
		}
		if (dataLength > DAN2pw1pwE || num > inData.Count - offset)
		{
			throw new ArithmeticException("Byte overflow in constructor.");
		}
		A7G2UZpiBV = new uint[DAN2pw1pwE];
		int num3 = num - 1;
		int num4 = 0;
		while (num3 >= 3)
		{
			A7G2UZpiBV[num4] = (uint)((inData[offset + num3 - 3] << 24) + (inData[offset + num3 - 2] << 16) + (inData[offset + num3 - 1] << 8) + inData[offset + num3]);
			num3 -= 4;
			num4++;
		}
		switch (num2)
		{
		case 1:
			A7G2UZpiBV[dataLength - 1] = inData[offset];
			break;
		case 2:
			A7G2UZpiBV[dataLength - 1] = (uint)((inData[offset] << 8) + inData[offset + 1]);
			break;
		case 3:
			A7G2UZpiBV[dataLength - 1] = (uint)((inData[offset] << 16) + (inData[offset + 1] << 8) + inData[offset + 2]);
			break;
		}
		if (dataLength == 0)
		{
			dataLength = 1;
		}
		while (dataLength > 1 && A7G2UZpiBV[dataLength - 1] == 0)
		{
			dataLength--;
		}
	}

	public BigInteger(uint[] inData)
	{
		dataLength = inData.Length;
		if (dataLength > DAN2pw1pwE)
		{
			throw new ArithmeticException("Byte overflow in constructor.");
		}
		A7G2UZpiBV = new uint[DAN2pw1pwE];
		int num = dataLength - 1;
		int num2 = 0;
		while (num >= 0)
		{
			A7G2UZpiBV[num2] = inData[num];
			num--;
			num2++;
		}
		while (dataLength > 1 && A7G2UZpiBV[dataLength - 1] == 0)
		{
			dataLength--;
		}
	}

	public static implicit operator BigInteger(long value)
	{
		return new BigInteger(value);
	}

	public static implicit operator BigInteger(ulong value)
	{
		return new BigInteger(value);
	}

	public static implicit operator BigInteger(int value)
	{
		return new BigInteger(value);
	}

	public static implicit operator BigInteger(uint value)
	{
		return new BigInteger((ulong)value);
	}

	public static BigInteger operator +(BigInteger bi1, BigInteger bi2)
	{
		BigInteger bigInteger = new BigInteger
		{
			dataLength = ((bi1.dataLength > bi2.dataLength) ? bi1.dataLength : bi2.dataLength)
		};
		long num = 0L;
		for (int i = 0; i < bigInteger.dataLength; i++)
		{
			long num2 = (long)bi1.A7G2UZpiBV[i] + (long)bi2.A7G2UZpiBV[i] + num;
			num = num2 >> 32;
			bigInteger.A7G2UZpiBV[i] = (uint)(num2 & 0xFFFFFFFFu);
		}
		if (num != 0L && bigInteger.dataLength < DAN2pw1pwE)
		{
			bigInteger.A7G2UZpiBV[bigInteger.dataLength] = (uint)num;
			bigInteger.dataLength++;
		}
		while (bigInteger.dataLength > 1 && bigInteger.A7G2UZpiBV[bigInteger.dataLength - 1] == 0)
		{
			bigInteger.dataLength--;
		}
		int num3 = DAN2pw1pwE - 1;
		if ((bi1.A7G2UZpiBV[num3] & 0x80000000u) == (bi2.A7G2UZpiBV[num3] & 0x80000000u) && (bigInteger.A7G2UZpiBV[num3] & 0x80000000u) != (bi1.A7G2UZpiBV[num3] & 0x80000000u))
		{
			throw new ArithmeticException();
		}
		return bigInteger;
	}

	public static BigInteger operator ++(BigInteger bi1)
	{
		BigInteger bigInteger = new BigInteger(bi1);
		long num = 1L;
		int num2 = 0;
		while (num != 0L && num2 < DAN2pw1pwE)
		{
			long num3 = bigInteger.A7G2UZpiBV[num2];
			num3++;
			bigInteger.A7G2UZpiBV[num2] = (uint)(num3 & 0xFFFFFFFFu);
			num = num3 >> 32;
			num2++;
		}
		if (num2 > bigInteger.dataLength)
		{
			bigInteger.dataLength = num2;
		}
		else
		{
			while (bigInteger.dataLength > 1 && bigInteger.A7G2UZpiBV[bigInteger.dataLength - 1] == 0)
			{
				bigInteger.dataLength--;
			}
		}
		int num4 = DAN2pw1pwE - 1;
		if ((bi1.A7G2UZpiBV[num4] & 0x80000000u) == 0 && (bigInteger.A7G2UZpiBV[num4] & 0x80000000u) != (bi1.A7G2UZpiBV[num4] & 0x80000000u))
		{
			throw new ArithmeticException("Overflow in ++.");
		}
		return bigInteger;
	}

	public static BigInteger operator -(BigInteger bi1, BigInteger bi2)
	{
		BigInteger bigInteger = new BigInteger
		{
			dataLength = ((bi1.dataLength > bi2.dataLength) ? bi1.dataLength : bi2.dataLength)
		};
		long num = 0L;
		for (int i = 0; i < bigInteger.dataLength; i++)
		{
			long num2 = (long)bi1.A7G2UZpiBV[i] - (long)bi2.A7G2UZpiBV[i] - num;
			bigInteger.A7G2UZpiBV[i] = (uint)(num2 & 0xFFFFFFFFu);
			num = ((num2 >= 0) ? 0 : 1);
		}
		if (num != 0L)
		{
			for (int j = bigInteger.dataLength; j < DAN2pw1pwE; j++)
			{
				bigInteger.A7G2UZpiBV[j] = uint.MaxValue;
			}
			bigInteger.dataLength = DAN2pw1pwE;
		}
		while (bigInteger.dataLength > 1 && bigInteger.A7G2UZpiBV[bigInteger.dataLength - 1] == 0)
		{
			bigInteger.dataLength--;
		}
		int num3 = DAN2pw1pwE - 1;
		if ((bi1.A7G2UZpiBV[num3] & 0x80000000u) != (bi2.A7G2UZpiBV[num3] & 0x80000000u) && (bigInteger.A7G2UZpiBV[num3] & 0x80000000u) != (bi1.A7G2UZpiBV[num3] & 0x80000000u))
		{
			throw new ArithmeticException();
		}
		return bigInteger;
	}

	public static BigInteger operator --(BigInteger bi1)
	{
		BigInteger bigInteger = new BigInteger(bi1);
		bool flag = true;
		int num = 0;
		while (flag && num < DAN2pw1pwE)
		{
			long num2 = bigInteger.A7G2UZpiBV[num];
			num2--;
			bigInteger.A7G2UZpiBV[num] = (uint)(num2 & 0xFFFFFFFFu);
			if (num2 >= 0)
			{
				flag = false;
			}
			num++;
		}
		if (num > bigInteger.dataLength)
		{
			bigInteger.dataLength = num;
		}
		while (bigInteger.dataLength > 1 && bigInteger.A7G2UZpiBV[bigInteger.dataLength - 1] == 0)
		{
			bigInteger.dataLength--;
		}
		int num3 = DAN2pw1pwE - 1;
		if ((bi1.A7G2UZpiBV[num3] & 0x80000000u) != 0 && (bigInteger.A7G2UZpiBV[num3] & 0x80000000u) != (bi1.A7G2UZpiBV[num3] & 0x80000000u))
		{
			throw new ArithmeticException("Underflow in --.");
		}
		return bigInteger;
	}

	public static BigInteger operator *(BigInteger bi1, BigInteger bi2)
	{
		int num = DAN2pw1pwE - 1;
		bool flag = false;
		bool flag2 = false;
		try
		{
			if ((bi1.A7G2UZpiBV[num] & 0x80000000u) != 0)
			{
				flag = true;
				bi1 = -bi1;
			}
			if ((bi2.A7G2UZpiBV[num] & 0x80000000u) != 0)
			{
				flag2 = true;
				bi2 = -bi2;
			}
		}
		catch (Exception)
		{
		}
		BigInteger bigInteger = new BigInteger();
		try
		{
			for (int i = 0; i < bi1.dataLength; i++)
			{
				if (bi1.A7G2UZpiBV[i] != 0)
				{
					ulong num2 = 0uL;
					int num3 = 0;
					int num4 = i;
					while (num3 < bi2.dataLength)
					{
						ulong num5 = (ulong)((long)bi1.A7G2UZpiBV[i] * (long)bi2.A7G2UZpiBV[num3] + bigInteger.A7G2UZpiBV[num4]) + num2;
						bigInteger.A7G2UZpiBV[num4] = (uint)(num5 & 0xFFFFFFFFu);
						num2 = num5 >> 32;
						num3++;
						num4++;
					}
					if (num2 != 0L)
					{
						bigInteger.A7G2UZpiBV[i + bi2.dataLength] = (uint)num2;
					}
				}
			}
		}
		catch (Exception)
		{
			throw new ArithmeticException("Multiplication overflow.");
		}
		bigInteger.dataLength = bi1.dataLength + bi2.dataLength;
		if (bigInteger.dataLength > DAN2pw1pwE)
		{
			bigInteger.dataLength = DAN2pw1pwE;
		}
		while (bigInteger.dataLength > 1 && bigInteger.A7G2UZpiBV[bigInteger.dataLength - 1] == 0)
		{
			bigInteger.dataLength--;
		}
		if ((bigInteger.A7G2UZpiBV[num] & 0x80000000u) != 0)
		{
			if (flag != flag2 && bigInteger.A7G2UZpiBV[num] == 2147483648u)
			{
				if (bigInteger.dataLength == 1)
				{
					return bigInteger;
				}
				bool flag3 = true;
				for (int j = 0; j < bigInteger.dataLength - 1 && flag3; j++)
				{
					if (bigInteger.A7G2UZpiBV[j] != 0)
					{
						flag3 = false;
					}
				}
				if (flag3)
				{
					return bigInteger;
				}
			}
			throw new ArithmeticException("Multiplication overflow.");
		}
		if (flag != flag2)
		{
			return -bigInteger;
		}
		return bigInteger;
	}

	public static BigInteger operator <<(BigInteger bi1, int shiftVal)
	{
		BigInteger bigInteger = new BigInteger(bi1);
		bigInteger.dataLength = J5I2P8k3J0(bigInteger.A7G2UZpiBV, shiftVal);
		return bigInteger;
	}

	private static int J5I2P8k3J0(uint[] P_0, int P_1)
	{
		int num = 32;
		int num2 = P_0.Length;
		while (num2 > 1 && P_0[num2 - 1] == 0)
		{
			num2--;
		}
		for (int num3 = P_1; num3 > 0; num3 -= num)
		{
			if (num3 < num)
			{
				num = num3;
			}
			ulong num4 = 0uL;
			for (int i = 0; i < num2; i++)
			{
				ulong num5 = (ulong)P_0[i] << num;
				num5 |= num4;
				P_0[i] = (uint)(num5 & 0xFFFFFFFFu);
				num4 = num5 >> 32;
			}
			if (num4 != 0L && num2 + 1 <= P_0.Length)
			{
				P_0[num2] = (uint)num4;
				num2++;
			}
		}
		return num2;
	}

	public static BigInteger operator >>(BigInteger bi1, int shiftVal)
	{
		BigInteger bigInteger = new BigInteger(bi1);
		bigInteger.dataLength = Cv52ZocIqN(bigInteger.A7G2UZpiBV, shiftVal);
		if ((bi1.A7G2UZpiBV[DAN2pw1pwE - 1] & 0x80000000u) != 0)
		{
			for (int num = DAN2pw1pwE - 1; num >= bigInteger.dataLength; num--)
			{
				bigInteger.A7G2UZpiBV[num] = uint.MaxValue;
			}
			uint num2 = 2147483648u;
			for (int i = 0; i < 32; i++)
			{
				if ((bigInteger.A7G2UZpiBV[bigInteger.dataLength - 1] & num2) != 0)
				{
					break;
				}
				bigInteger.A7G2UZpiBV[bigInteger.dataLength - 1] |= num2;
				num2 >>= 1;
			}
			bigInteger.dataLength = DAN2pw1pwE;
		}
		return bigInteger;
	}

	private static int Cv52ZocIqN(uint[] P_0, int P_1)
	{
		int num = 32;
		int num2 = 0;
		int num3 = P_0.Length;
		while (num3 > 1 && P_0[num3 - 1] == 0)
		{
			num3--;
		}
		for (int num4 = P_1; num4 > 0; num4 -= num)
		{
			if (num4 < num)
			{
				num = num4;
				num2 = 32 - num;
			}
			ulong num5 = 0uL;
			for (int num6 = num3 - 1; num6 >= 0; num6--)
			{
				ulong num7 = (ulong)P_0[num6] >> num;
				num7 |= num5;
				num5 = ((ulong)P_0[num6] << num2) & 0xFFFFFFFFu;
				P_0[num6] = (uint)num7;
			}
		}
		while (num3 > 1 && P_0[num3 - 1] == 0)
		{
			num3--;
		}
		return num3;
	}

	public static BigInteger operator ~(BigInteger bi1)
	{
		BigInteger bigInteger = new BigInteger(bi1);
		for (int i = 0; i < DAN2pw1pwE; i++)
		{
			bigInteger.A7G2UZpiBV[i] = ~bi1.A7G2UZpiBV[i];
		}
		bigInteger.dataLength = DAN2pw1pwE;
		while (bigInteger.dataLength > 1 && bigInteger.A7G2UZpiBV[bigInteger.dataLength - 1] == 0)
		{
			bigInteger.dataLength--;
		}
		return bigInteger;
	}

	public static BigInteger operator -(BigInteger bi1)
	{
		if (bi1.dataLength == 1 && bi1.A7G2UZpiBV[0] == 0)
		{
			return new BigInteger();
		}
		BigInteger bigInteger = new BigInteger(bi1);
		for (int i = 0; i < DAN2pw1pwE; i++)
		{
			bigInteger.A7G2UZpiBV[i] = ~bi1.A7G2UZpiBV[i];
		}
		long num = 1L;
		int num2 = 0;
		while (num != 0L && num2 < DAN2pw1pwE)
		{
			long num3 = bigInteger.A7G2UZpiBV[num2];
			num3++;
			bigInteger.A7G2UZpiBV[num2] = (uint)(num3 & 0xFFFFFFFFu);
			num = num3 >> 32;
			num2++;
		}
		if ((bi1.A7G2UZpiBV[DAN2pw1pwE - 1] & 0x80000000u) == (bigInteger.A7G2UZpiBV[DAN2pw1pwE - 1] & 0x80000000u))
		{
			throw new ArithmeticException("Overflow in negation.\n");
		}
		bigInteger.dataLength = DAN2pw1pwE;
		while (bigInteger.dataLength > 1 && bigInteger.A7G2UZpiBV[bigInteger.dataLength - 1] == 0)
		{
			bigInteger.dataLength--;
		}
		return bigInteger;
	}

	public static bool operator ==(BigInteger bi1, BigInteger bi2)
	{
		return bi1.Equals(bi2);
	}

	public static bool operator !=(BigInteger bi1, BigInteger bi2)
	{
		return !bi1.Equals(bi2);
	}

	public override bool Equals(object? o)
	{
		BigInteger bigInteger = (BigInteger)o;
		if (dataLength != bigInteger.dataLength)
		{
			return false;
		}
		for (int i = 0; i < dataLength; i++)
		{
			if (A7G2UZpiBV[i] != bigInteger.A7G2UZpiBV[i])
			{
				return false;
			}
		}
		return true;
	}

	public override int GetHashCode()
	{
		return ToString().GetHashCode();
	}

	public static bool operator >(BigInteger bi1, BigInteger bi2)
	{
		int num = DAN2pw1pwE - 1;
		if ((bi1.A7G2UZpiBV[num] & 0x80000000u) != 0 && (bi2.A7G2UZpiBV[num] & 0x80000000u) == 0)
		{
			return false;
		}
		if ((bi1.A7G2UZpiBV[num] & 0x80000000u) == 0 && (bi2.A7G2UZpiBV[num] & 0x80000000u) != 0)
		{
			return true;
		}
		num = ((bi1.dataLength > bi2.dataLength) ? bi1.dataLength : bi2.dataLength) - 1;
		while (num >= 0 && bi1.A7G2UZpiBV[num] == bi2.A7G2UZpiBV[num])
		{
			num--;
		}
		if (num >= 0)
		{
			if (bi1.A7G2UZpiBV[num] > bi2.A7G2UZpiBV[num])
			{
				return true;
			}
			return false;
		}
		return false;
	}

	public static bool operator <(BigInteger bi1, BigInteger bi2)
	{
		int num = DAN2pw1pwE - 1;
		if ((bi1.A7G2UZpiBV[num] & 0x80000000u) != 0 && (bi2.A7G2UZpiBV[num] & 0x80000000u) == 0)
		{
			return true;
		}
		if ((bi1.A7G2UZpiBV[num] & 0x80000000u) == 0 && (bi2.A7G2UZpiBV[num] & 0x80000000u) != 0)
		{
			return false;
		}
		num = ((bi1.dataLength > bi2.dataLength) ? bi1.dataLength : bi2.dataLength) - 1;
		while (num >= 0 && bi1.A7G2UZpiBV[num] == bi2.A7G2UZpiBV[num])
		{
			num--;
		}
		if (num >= 0)
		{
			if (bi1.A7G2UZpiBV[num] < bi2.A7G2UZpiBV[num])
			{
				return true;
			}
			return false;
		}
		return false;
	}

	public static bool operator >=(BigInteger bi1, BigInteger bi2)
	{
		if (!(bi1 == bi2))
		{
			return bi1 > bi2;
		}
		return true;
	}

	public static bool operator <=(BigInteger bi1, BigInteger bi2)
	{
		if (!(bi1 == bi2))
		{
			return bi1 < bi2;
		}
		return true;
	}

	private static void mCM2JmMTo8(BigInteger P_0, BigInteger P_1, BigInteger P_2, BigInteger P_3)
	{
		uint[] array = new uint[DAN2pw1pwE];
		int num = P_0.dataLength + 1;
		uint[] array2 = new uint[num];
		uint num2 = 2147483648u;
		uint num3 = P_1.A7G2UZpiBV[P_1.dataLength - 1];
		int num4 = 0;
		int num5 = 0;
		while (num2 != 0 && (num3 & num2) == 0)
		{
			num4++;
			num2 >>= 1;
		}
		for (int i = 0; i < P_0.dataLength; i++)
		{
			array2[i] = P_0.A7G2UZpiBV[i];
		}
		J5I2P8k3J0(array2, num4);
		P_1 <<= num4;
		int num6 = num - P_1.dataLength;
		int num7 = num - 1;
		ulong num8 = P_1.A7G2UZpiBV[P_1.dataLength - 1];
		ulong num9 = P_1.A7G2UZpiBV[P_1.dataLength - 2];
		int num10 = P_1.dataLength + 1;
		uint[] array3 = new uint[num10];
		while (num6 > 0)
		{
			ulong num11 = ((ulong)array2[num7] << 32) + array2[num7 - 1];
			ulong num12 = num11 / num8;
			ulong num13 = num11 % num8;
			bool flag = false;
			while (!flag)
			{
				flag = true;
				if (num12 == 4294967296L || num12 * num9 > (num13 << 32) + array2[num7 - 2])
				{
					num12--;
					num13 += num8;
					if (num13 < 4294967296L)
					{
						flag = false;
					}
				}
			}
			for (int j = 0; j < num10; j++)
			{
				array3[j] = array2[num7 - j];
			}
			BigInteger bigInteger = new BigInteger(array3);
			BigInteger bigInteger2;
			for (bigInteger2 = P_1 * (long)num12; bigInteger2 > bigInteger; bigInteger2 -= P_1)
			{
				num12--;
			}
			BigInteger bigInteger3 = bigInteger - bigInteger2;
			for (int k = 0; k < num10; k++)
			{
				array2[num7 - k] = bigInteger3.A7G2UZpiBV[P_1.dataLength - k];
			}
			array[num5++] = (uint)num12;
			num7--;
			num6--;
		}
		P_2.dataLength = num5;
		int l = 0;
		int num14 = P_2.dataLength - 1;
		while (num14 >= 0)
		{
			P_2.A7G2UZpiBV[l] = array[num14];
			num14--;
			l++;
		}
		for (; l < DAN2pw1pwE; l++)
		{
			P_2.A7G2UZpiBV[l] = 0u;
		}
		while (P_2.dataLength > 1 && P_2.A7G2UZpiBV[P_2.dataLength - 1] == 0)
		{
			P_2.dataLength--;
		}
		if (P_2.dataLength == 0)
		{
			P_2.dataLength = 1;
		}
		P_3.dataLength = Cv52ZocIqN(array2, num4);
		for (l = 0; l < P_3.dataLength; l++)
		{
			P_3.A7G2UZpiBV[l] = array2[l];
		}
		for (; l < DAN2pw1pwE; l++)
		{
			P_3.A7G2UZpiBV[l] = 0u;
		}
	}

	private static void KgQ2k5kBEa(BigInteger P_0, BigInteger P_1, BigInteger P_2, BigInteger P_3)
	{
		uint[] array = new uint[DAN2pw1pwE];
		int num = 0;
		for (int i = 0; i < DAN2pw1pwE; i++)
		{
			P_3.A7G2UZpiBV[i] = P_0.A7G2UZpiBV[i];
		}
		P_3.dataLength = P_0.dataLength;
		while (P_3.dataLength > 1 && P_3.A7G2UZpiBV[P_3.dataLength - 1] == 0)
		{
			P_3.dataLength--;
		}
		ulong num2 = P_1.A7G2UZpiBV[0];
		int num3 = P_3.dataLength - 1;
		ulong num4 = P_3.A7G2UZpiBV[num3];
		if (num4 >= num2)
		{
			ulong num5 = num4 / num2;
			array[num++] = (uint)num5;
			P_3.A7G2UZpiBV[num3] = (uint)(num4 % num2);
		}
		num3--;
		while (num3 >= 0)
		{
			num4 = ((ulong)P_3.A7G2UZpiBV[num3 + 1] << 32) + P_3.A7G2UZpiBV[num3];
			ulong num6 = num4 / num2;
			array[num++] = (uint)num6;
			P_3.A7G2UZpiBV[num3 + 1] = 0u;
			P_3.A7G2UZpiBV[num3--] = (uint)(num4 % num2);
		}
		P_2.dataLength = num;
		int j = 0;
		int num7 = P_2.dataLength - 1;
		while (num7 >= 0)
		{
			P_2.A7G2UZpiBV[j] = array[num7];
			num7--;
			j++;
		}
		for (; j < DAN2pw1pwE; j++)
		{
			P_2.A7G2UZpiBV[j] = 0u;
		}
		while (P_2.dataLength > 1 && P_2.A7G2UZpiBV[P_2.dataLength - 1] == 0)
		{
			P_2.dataLength--;
		}
		if (P_2.dataLength == 0)
		{
			P_2.dataLength = 1;
		}
		while (P_3.dataLength > 1 && P_3.A7G2UZpiBV[P_3.dataLength - 1] == 0)
		{
			P_3.dataLength--;
		}
	}

	public static BigInteger operator /(BigInteger bi1, BigInteger bi2)
	{
		BigInteger bigInteger = new BigInteger();
		BigInteger bigInteger2 = new BigInteger();
		int num = DAN2pw1pwE - 1;
		bool flag = false;
		bool flag2 = false;
		if ((bi1.A7G2UZpiBV[num] & 0x80000000u) != 0)
		{
			bi1 = -bi1;
			flag2 = true;
		}
		if ((bi2.A7G2UZpiBV[num] & 0x80000000u) != 0)
		{
			bi2 = -bi2;
			flag = true;
		}
		if (bi1 < bi2)
		{
			return bigInteger;
		}
		if (bi2.dataLength == 1)
		{
			KgQ2k5kBEa(bi1, bi2, bigInteger, bigInteger2);
		}
		else
		{
			mCM2JmMTo8(bi1, bi2, bigInteger, bigInteger2);
		}
		if (flag2 != flag)
		{
			return -bigInteger;
		}
		return bigInteger;
	}

	public static BigInteger operator %(BigInteger bi1, BigInteger bi2)
	{
		BigInteger bigInteger = new BigInteger();
		BigInteger bigInteger2 = new BigInteger(bi1);
		int num = DAN2pw1pwE - 1;
		bool flag = false;
		if ((bi1.A7G2UZpiBV[num] & 0x80000000u) != 0)
		{
			bi1 = -bi1;
			flag = true;
		}
		if ((bi2.A7G2UZpiBV[num] & 0x80000000u) != 0)
		{
			bi2 = -bi2;
		}
		if (bi1 < bi2)
		{
			return bigInteger2;
		}
		if (bi2.dataLength == 1)
		{
			KgQ2k5kBEa(bi1, bi2, bigInteger, bigInteger2);
		}
		else
		{
			mCM2JmMTo8(bi1, bi2, bigInteger, bigInteger2);
		}
		if (flag)
		{
			return -bigInteger2;
		}
		return bigInteger2;
	}

	public static BigInteger operator &(BigInteger bi1, BigInteger bi2)
	{
		BigInteger bigInteger = new BigInteger();
		int num = ((bi1.dataLength > bi2.dataLength) ? bi1.dataLength : bi2.dataLength);
		for (int i = 0; i < num; i++)
		{
			uint num2 = bi1.A7G2UZpiBV[i] & bi2.A7G2UZpiBV[i];
			bigInteger.A7G2UZpiBV[i] = num2;
		}
		bigInteger.dataLength = DAN2pw1pwE;
		while (bigInteger.dataLength > 1 && bigInteger.A7G2UZpiBV[bigInteger.dataLength - 1] == 0)
		{
			bigInteger.dataLength--;
		}
		return bigInteger;
	}

	public static BigInteger operator |(BigInteger bi1, BigInteger bi2)
	{
		BigInteger bigInteger = new BigInteger();
		int num = ((bi1.dataLength > bi2.dataLength) ? bi1.dataLength : bi2.dataLength);
		for (int i = 0; i < num; i++)
		{
			uint num2 = bi1.A7G2UZpiBV[i] | bi2.A7G2UZpiBV[i];
			bigInteger.A7G2UZpiBV[i] = num2;
		}
		bigInteger.dataLength = DAN2pw1pwE;
		while (bigInteger.dataLength > 1 && bigInteger.A7G2UZpiBV[bigInteger.dataLength - 1] == 0)
		{
			bigInteger.dataLength--;
		}
		return bigInteger;
	}

	public static BigInteger operator ^(BigInteger bi1, BigInteger bi2)
	{
		BigInteger bigInteger = new BigInteger();
		int num = ((bi1.dataLength > bi2.dataLength) ? bi1.dataLength : bi2.dataLength);
		for (int i = 0; i < num; i++)
		{
			uint num2 = bi1.A7G2UZpiBV[i] ^ bi2.A7G2UZpiBV[i];
			bigInteger.A7G2UZpiBV[i] = num2;
		}
		bigInteger.dataLength = DAN2pw1pwE;
		while (bigInteger.dataLength > 1 && bigInteger.A7G2UZpiBV[bigInteger.dataLength - 1] == 0)
		{
			bigInteger.dataLength--;
		}
		return bigInteger;
	}

	public BigInteger max(BigInteger bi)
	{
		if (this > bi)
		{
			return new BigInteger(this);
		}
		return new BigInteger(bi);
	}

	public BigInteger min(BigInteger bi)
	{
		if (this < bi)
		{
			return new BigInteger(this);
		}
		return new BigInteger(bi);
	}

	public BigInteger abs()
	{
		if ((A7G2UZpiBV[DAN2pw1pwE - 1] & 0x80000000u) != 0)
		{
			return -this;
		}
		return new BigInteger(this);
	}

	public override string ToString()
	{
		return ToString(10);
	}

	public string ToString(int radix)
	{
		if (radix < 2 || radix > 36)
		{
			throw new ArgumentException("Radix must be >= 2 and <= 36");
		}
		string text = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		string text2 = "";
		BigInteger bigInteger = this;
		bool flag = false;
		if ((bigInteger.A7G2UZpiBV[DAN2pw1pwE - 1] & 0x80000000u) != 0)
		{
			flag = true;
			try
			{
				bigInteger = -bigInteger;
			}
			catch (Exception)
			{
			}
		}
		BigInteger bigInteger2 = new BigInteger();
		BigInteger bigInteger3 = new BigInteger();
		BigInteger bigInteger4 = new BigInteger(radix);
		if (bigInteger.dataLength == 1 && bigInteger.A7G2UZpiBV[0] == 0)
		{
			text2 = "0";
		}
		else
		{
			while (bigInteger.dataLength > 1 || (bigInteger.dataLength == 1 && bigInteger.A7G2UZpiBV[0] != 0))
			{
				KgQ2k5kBEa(bigInteger, bigInteger4, bigInteger2, bigInteger3);
				text2 = ((bigInteger3.A7G2UZpiBV[0] >= 10) ? (text[(int)(bigInteger3.A7G2UZpiBV[0] - 10)] + text2) : (bigInteger3.A7G2UZpiBV[0] + text2));
				bigInteger = bigInteger2;
			}
			if (flag)
			{
				text2 = "-" + text2;
			}
		}
		return text2;
	}

	public string ToHexString()
	{
		string text = A7G2UZpiBV[dataLength - 1].ToString("X");
		for (int num = dataLength - 2; num >= 0; num--)
		{
			text += A7G2UZpiBV[num].ToString("X8");
		}
		return text;
	}

	public BigInteger modPow(BigInteger exp, BigInteger n)
	{
		if ((exp.A7G2UZpiBV[DAN2pw1pwE - 1] & 0x80000000u) != 0)
		{
			throw new ArithmeticException("Positive exponents only.");
		}
		BigInteger bigInteger = 1;
		bool flag = false;
		BigInteger bigInteger2;
		if ((A7G2UZpiBV[DAN2pw1pwE - 1] & 0x80000000u) != 0)
		{
			bigInteger2 = -this % n;
			flag = true;
		}
		else
		{
			bigInteger2 = this % n;
		}
		if ((n.A7G2UZpiBV[DAN2pw1pwE - 1] & 0x80000000u) != 0)
		{
			n = -n;
		}
		BigInteger bigInteger3 = new BigInteger();
		int num = n.dataLength << 1;
		bigInteger3.A7G2UZpiBV[num] = 1u;
		bigInteger3.dataLength = num + 1;
		bigInteger3 /= n;
		int num2 = exp.bitCount();
		int num3 = 0;
		for (int i = 0; i < exp.dataLength; i++)
		{
			uint num4 = 1u;
			for (int j = 0; j < 32; j++)
			{
				if ((exp.A7G2UZpiBV[i] & num4) != 0)
				{
					bigInteger = W9o20KSE25(bigInteger * bigInteger2, n, bigInteger3);
				}
				num4 <<= 1;
				bigInteger2 = W9o20KSE25(bigInteger2 * bigInteger2, n, bigInteger3);
				if (bigInteger2.dataLength == 1 && bigInteger2.A7G2UZpiBV[0] == 1)
				{
					if (flag && (exp.A7G2UZpiBV[0] & 1) != 0)
					{
						return -bigInteger;
					}
					return bigInteger;
				}
				num3++;
				if (num3 == num2)
				{
					break;
				}
			}
		}
		if (flag && (exp.A7G2UZpiBV[0] & 1) != 0)
		{
			return -bigInteger;
		}
		return bigInteger;
	}

	private BigInteger W9o20KSE25(BigInteger P_0, BigInteger P_1, BigInteger P_2)
	{
		int num = P_1.dataLength;
		int num2 = num + 1;
		int num3 = num - 1;
		BigInteger bigInteger = new BigInteger();
		int num4 = num3;
		int num5 = 0;
		while (num4 < P_0.dataLength)
		{
			bigInteger.A7G2UZpiBV[num5] = P_0.A7G2UZpiBV[num4];
			num4++;
			num5++;
		}
		bigInteger.dataLength = P_0.dataLength - num3;
		if (bigInteger.dataLength <= 0)
		{
			bigInteger.dataLength = 1;
		}
		BigInteger bigInteger2 = bigInteger * P_2;
		BigInteger bigInteger3 = new BigInteger();
		int num6 = num2;
		int num7 = 0;
		while (num6 < bigInteger2.dataLength)
		{
			bigInteger3.A7G2UZpiBV[num7] = bigInteger2.A7G2UZpiBV[num6];
			num6++;
			num7++;
		}
		bigInteger3.dataLength = bigInteger2.dataLength - num2;
		if (bigInteger3.dataLength <= 0)
		{
			bigInteger3.dataLength = 1;
		}
		BigInteger bigInteger4 = new BigInteger();
		int num8 = ((P_0.dataLength > num2) ? num2 : P_0.dataLength);
		for (int i = 0; i < num8; i++)
		{
			bigInteger4.A7G2UZpiBV[i] = P_0.A7G2UZpiBV[i];
		}
		bigInteger4.dataLength = num8;
		BigInteger bigInteger5 = new BigInteger();
		for (int j = 0; j < bigInteger3.dataLength; j++)
		{
			if (bigInteger3.A7G2UZpiBV[j] != 0)
			{
				ulong num9 = 0uL;
				int num10 = j;
				int num11 = 0;
				while (num11 < P_1.dataLength && num10 < num2)
				{
					ulong num12 = (ulong)((long)bigInteger3.A7G2UZpiBV[j] * (long)P_1.A7G2UZpiBV[num11] + bigInteger5.A7G2UZpiBV[num10]) + num9;
					bigInteger5.A7G2UZpiBV[num10] = (uint)(num12 & 0xFFFFFFFFu);
					num9 = num12 >> 32;
					num11++;
					num10++;
				}
				if (num10 < num2)
				{
					bigInteger5.A7G2UZpiBV[num10] = (uint)num9;
				}
			}
		}
		bigInteger5.dataLength = num2;
		while (bigInteger5.dataLength > 1 && bigInteger5.A7G2UZpiBV[bigInteger5.dataLength - 1] == 0)
		{
			bigInteger5.dataLength--;
		}
		bigInteger4 -= bigInteger5;
		if ((bigInteger4.A7G2UZpiBV[DAN2pw1pwE - 1] & 0x80000000u) != 0)
		{
			BigInteger bigInteger6 = new BigInteger();
			bigInteger6.A7G2UZpiBV[num2] = 1u;
			bigInteger6.dataLength = num2 + 1;
			bigInteger4 += bigInteger6;
		}
		for (; bigInteger4 >= P_1; bigInteger4 -= P_1)
		{
		}
		return bigInteger4;
	}

	public BigInteger gcd(BigInteger bi)
	{
		BigInteger bigInteger = (((A7G2UZpiBV[DAN2pw1pwE - 1] & 0x80000000u) == 0) ? this : (-this));
		BigInteger bigInteger2 = (((bi.A7G2UZpiBV[DAN2pw1pwE - 1] & 0x80000000u) == 0) ? bi : (-bi));
		BigInteger bigInteger3 = bigInteger2;
		while (bigInteger.dataLength > 1 || (bigInteger.dataLength == 1 && bigInteger.A7G2UZpiBV[0] != 0))
		{
			bigInteger3 = bigInteger;
			bigInteger = bigInteger2 % bigInteger;
			bigInteger2 = bigInteger3;
		}
		return bigInteger3;
	}

	public void genRandomBits(int bits, Random rand)
	{
		int num = bits >> 5;
		int num2 = bits & 0x1F;
		if (num2 != 0)
		{
			num++;
		}
		if (num > DAN2pw1pwE || bits <= 0)
		{
			throw new ArithmeticException("Number of required bits is not valid.");
		}
		byte[] array = new byte[num * 4];
		rand.NextBytes(array);
		for (int i = 0; i < num; i++)
		{
			A7G2UZpiBV[i] = BitConverter.ToUInt32(array, i * 4);
		}
		for (int j = num; j < DAN2pw1pwE; j++)
		{
			A7G2UZpiBV[j] = 0u;
		}
		if (num2 != 0)
		{
			uint num3;
			if (bits != 1)
			{
				num3 = (uint)(1 << num2 - 1);
				A7G2UZpiBV[num - 1] |= num3;
			}
			num3 = uint.MaxValue >> 32 - num2;
			A7G2UZpiBV[num - 1] &= num3;
		}
		else
		{
			A7G2UZpiBV[num - 1] |= 2147483648u;
		}
		dataLength = num;
		if (dataLength == 0)
		{
			dataLength = 1;
		}
	}

	public void genRandomBits(int bits, RandomNumberGenerator rng)
	{
		int num = bits >> 5;
		int num2 = bits & 0x1F;
		if (num2 != 0)
		{
			num++;
		}
		if (num > DAN2pw1pwE || bits <= 0)
		{
			throw new ArithmeticException("Number of required bits is not valid.");
		}
		byte[] array = new byte[num * 4];
		rng.GetBytes(array);
		for (int i = 0; i < num; i++)
		{
			A7G2UZpiBV[i] = BitConverter.ToUInt32(array, i * 4);
		}
		for (int j = num; j < DAN2pw1pwE; j++)
		{
			A7G2UZpiBV[j] = 0u;
		}
		if (num2 != 0)
		{
			uint num3;
			if (bits != 1)
			{
				num3 = (uint)(1 << num2 - 1);
				A7G2UZpiBV[num - 1] |= num3;
			}
			num3 = uint.MaxValue >> 32 - num2;
			A7G2UZpiBV[num - 1] &= num3;
		}
		else
		{
			A7G2UZpiBV[num - 1] |= 2147483648u;
		}
		dataLength = num;
		if (dataLength == 0)
		{
			dataLength = 1;
		}
	}

	public int bitCount()
	{
		while (dataLength > 1 && A7G2UZpiBV[dataLength - 1] == 0)
		{
			dataLength--;
		}
		uint num = A7G2UZpiBV[dataLength - 1];
		uint num2 = 2147483648u;
		int num3 = 32;
		while (num3 > 0 && (num & num2) == 0)
		{
			num3--;
			num2 >>= 1;
		}
		num3 += dataLength - 1 << 5;
		if (num3 != 0)
		{
			return num3;
		}
		return 1;
	}

	public bool FermatLittleTest(int confidence)
	{
		BigInteger bigInteger = (((A7G2UZpiBV[DAN2pw1pwE - 1] & 0x80000000u) == 0) ? this : (-this));
		if (bigInteger.dataLength == 1)
		{
			if (bigInteger.A7G2UZpiBV[0] == 0 || bigInteger.A7G2UZpiBV[0] == 1)
			{
				return false;
			}
			if (bigInteger.A7G2UZpiBV[0] == 2 || bigInteger.A7G2UZpiBV[0] == 3)
			{
				return true;
			}
		}
		if ((bigInteger.A7G2UZpiBV[0] & 1) == 0)
		{
			return false;
		}
		int num = bigInteger.bitCount();
		BigInteger bigInteger2 = new BigInteger();
		BigInteger exp = bigInteger - new BigInteger(1L);
		Random random = new Random();
		for (int i = 0; i < confidence; i++)
		{
			bool flag = false;
			while (!flag)
			{
				int num2;
				for (num2 = 0; num2 < 2; num2 = (int)(random.NextDouble() * (double)num))
				{
				}
				bigInteger2.genRandomBits(num2, random);
				int num3 = bigInteger2.dataLength;
				if (num3 > 1 || (num3 == 1 && bigInteger2.A7G2UZpiBV[0] != 1))
				{
					flag = true;
				}
			}
			BigInteger bigInteger3 = bigInteger2.gcd(bigInteger);
			if (bigInteger3.dataLength == 1 && bigInteger3.A7G2UZpiBV[0] != 1)
			{
				return false;
			}
			BigInteger bigInteger4 = bigInteger2.modPow(exp, bigInteger);
			int num4 = bigInteger4.dataLength;
			if (num4 > 1 || (num4 == 1 && bigInteger4.A7G2UZpiBV[0] != 1))
			{
				return false;
			}
		}
		return true;
	}

	public bool RabinMillerTest(int confidence)
	{
		BigInteger bigInteger = (((A7G2UZpiBV[DAN2pw1pwE - 1] & 0x80000000u) == 0) ? this : (-this));
		if (bigInteger.dataLength == 1)
		{
			if (bigInteger.A7G2UZpiBV[0] == 0 || bigInteger.A7G2UZpiBV[0] == 1)
			{
				return false;
			}
			if (bigInteger.A7G2UZpiBV[0] == 2 || bigInteger.A7G2UZpiBV[0] == 3)
			{
				return true;
			}
		}
		if ((bigInteger.A7G2UZpiBV[0] & 1) == 0)
		{
			return false;
		}
		BigInteger bigInteger2 = bigInteger - new BigInteger(1L);
		int num = 0;
		for (int i = 0; i < bigInteger2.dataLength; i++)
		{
			uint num2 = 1u;
			for (int j = 0; j < 32; j++)
			{
				if ((bigInteger2.A7G2UZpiBV[i] & num2) != 0)
				{
					i = bigInteger2.dataLength;
					break;
				}
				num2 <<= 1;
				num++;
			}
		}
		BigInteger exp = bigInteger2 >> num;
		int num3 = bigInteger.bitCount();
		BigInteger bigInteger3 = new BigInteger();
		Random random = new Random();
		for (int k = 0; k < confidence; k++)
		{
			bool flag = false;
			while (!flag)
			{
				int num4;
				for (num4 = 0; num4 < 2; num4 = (int)(random.NextDouble() * (double)num3))
				{
				}
				bigInteger3.genRandomBits(num4, random);
				int num5 = bigInteger3.dataLength;
				if (num5 > 1 || (num5 == 1 && bigInteger3.A7G2UZpiBV[0] != 1))
				{
					flag = true;
				}
			}
			BigInteger bigInteger4 = bigInteger3.gcd(bigInteger);
			if (bigInteger4.dataLength == 1 && bigInteger4.A7G2UZpiBV[0] != 1)
			{
				return false;
			}
			BigInteger bigInteger5 = bigInteger3.modPow(exp, bigInteger);
			bool flag2 = false;
			if (bigInteger5.dataLength == 1 && bigInteger5.A7G2UZpiBV[0] == 1)
			{
				flag2 = true;
			}
			int num6 = 0;
			while (!flag2 && num6 < num)
			{
				if (bigInteger5 == bigInteger2)
				{
					flag2 = true;
					break;
				}
				bigInteger5 = bigInteger5 * bigInteger5 % bigInteger;
				num6++;
			}
			if (!flag2)
			{
				return false;
			}
		}
		return true;
	}

	public bool SolovayStrassenTest(int confidence)
	{
		BigInteger bigInteger = (((A7G2UZpiBV[DAN2pw1pwE - 1] & 0x80000000u) == 0) ? this : (-this));
		if (bigInteger.dataLength == 1)
		{
			if (bigInteger.A7G2UZpiBV[0] == 0 || bigInteger.A7G2UZpiBV[0] == 1)
			{
				return false;
			}
			if (bigInteger.A7G2UZpiBV[0] == 2 || bigInteger.A7G2UZpiBV[0] == 3)
			{
				return true;
			}
		}
		if ((bigInteger.A7G2UZpiBV[0] & 1) == 0)
		{
			return false;
		}
		int num = bigInteger.bitCount();
		BigInteger bigInteger2 = new BigInteger();
		BigInteger bigInteger3 = bigInteger - 1;
		BigInteger exp = bigInteger3 >> 1;
		Random random = new Random();
		for (int i = 0; i < confidence; i++)
		{
			bool flag = false;
			while (!flag)
			{
				int num2;
				for (num2 = 0; num2 < 2; num2 = (int)(random.NextDouble() * (double)num))
				{
				}
				bigInteger2.genRandomBits(num2, random);
				int num3 = bigInteger2.dataLength;
				if (num3 > 1 || (num3 == 1 && bigInteger2.A7G2UZpiBV[0] != 1))
				{
					flag = true;
				}
			}
			BigInteger bigInteger4 = bigInteger2.gcd(bigInteger);
			if (bigInteger4.dataLength == 1 && bigInteger4.A7G2UZpiBV[0] != 1)
			{
				return false;
			}
			BigInteger bigInteger5 = bigInteger2.modPow(exp, bigInteger);
			if (bigInteger5 == bigInteger3)
			{
				bigInteger5 = -1;
			}
			BigInteger bigInteger6 = Jacobi(bigInteger2, bigInteger);
			if (bigInteger5 != bigInteger6)
			{
				return false;
			}
		}
		return true;
	}

	public bool LucasStrongTest()
	{
		BigInteger bigInteger = (((A7G2UZpiBV[DAN2pw1pwE - 1] & 0x80000000u) == 0) ? this : (-this));
		if (bigInteger.dataLength == 1)
		{
			if (bigInteger.A7G2UZpiBV[0] == 0 || bigInteger.A7G2UZpiBV[0] == 1)
			{
				return false;
			}
			if (bigInteger.A7G2UZpiBV[0] == 2 || bigInteger.A7G2UZpiBV[0] == 3)
			{
				return true;
			}
		}
		if ((bigInteger.A7G2UZpiBV[0] & 1) == 0)
		{
			return false;
		}
		return IpQ279rAj4(bigInteger);
	}

	private bool IpQ279rAj4(BigInteger P_0)
	{
		long num = 5L;
		long num2 = -1L;
		long num3 = 0L;
		for (bool flag = false; !flag; num3++)
		{
			switch (Jacobi(num, P_0))
			{
			case -1:
				flag = true;
				continue;
			case 0:
				if (Math.Abs(num) < P_0)
				{
					return false;
				}
				break;
			}
			if (num3 == 20)
			{
				BigInteger bigInteger = P_0.sqrt();
				if (bigInteger * bigInteger == P_0)
				{
					return false;
				}
			}
			num = (Math.Abs(num) + 2) * num2;
			num2 = -num2;
		}
		long num4 = 1 - num >> 2;
		BigInteger bigInteger2 = P_0 + 1;
		int num5 = 0;
		for (int i = 0; i < bigInteger2.dataLength; i++)
		{
			uint num6 = 1u;
			for (int j = 0; j < 32; j++)
			{
				if ((bigInteger2.A7G2UZpiBV[i] & num6) != 0)
				{
					i = bigInteger2.dataLength;
					break;
				}
				num6 <<= 1;
				num5++;
			}
		}
		BigInteger bigInteger3 = bigInteger2 >> num5;
		BigInteger bigInteger4 = new BigInteger();
		int num7 = P_0.dataLength << 1;
		bigInteger4.A7G2UZpiBV[num7] = 1u;
		bigInteger4.dataLength = num7 + 1;
		bigInteger4 /= P_0;
		BigInteger[] array = IWy2X71rmZ(1, num4, bigInteger3, P_0, bigInteger4, 0);
		bool flag2 = false;
		if ((array[0].dataLength == 1 && array[0].A7G2UZpiBV[0] == 0) || (array[1].dataLength == 1 && array[1].A7G2UZpiBV[0] == 0))
		{
			flag2 = true;
		}
		for (int k = 1; k < num5; k++)
		{
			if (!flag2)
			{
				array[1] = P_0.W9o20KSE25(array[1] * array[1], P_0, bigInteger4);
				array[1] = (array[1] - (array[2] << 1)) % P_0;
				if (array[1].dataLength == 1 && array[1].A7G2UZpiBV[0] == 0)
				{
					flag2 = true;
				}
			}
			array[2] = P_0.W9o20KSE25(array[2] * array[2], P_0, bigInteger4);
		}
		if (flag2)
		{
			BigInteger bigInteger5 = P_0.gcd(num4);
			if (bigInteger5.dataLength == 1 && bigInteger5.A7G2UZpiBV[0] == 1)
			{
				if ((array[2].A7G2UZpiBV[DAN2pw1pwE - 1] & 0x80000000u) != 0)
				{
					BigInteger[] array2 = array;
					array2[2] += P_0;
				}
				BigInteger bigInteger6 = num4 * Jacobi(num4, P_0) % P_0;
				if ((bigInteger6.A7G2UZpiBV[DAN2pw1pwE - 1] & 0x80000000u) != 0)
				{
					bigInteger6 += P_0;
				}
				if (array[2] != bigInteger6)
				{
					flag2 = false;
				}
			}
		}
		return flag2;
	}

	public bool isProbablePrime(int confidence)
	{
		BigInteger bigInteger = (((A7G2UZpiBV[DAN2pw1pwE - 1] & 0x80000000u) == 0) ? this : (-this));
		for (int i = 0; i < primesBelow2000.Length; i++)
		{
			BigInteger bigInteger2 = primesBelow2000[i];
			if (bigInteger2 >= bigInteger)
			{
				break;
			}
			if ((bigInteger % bigInteger2).IntValue() == 0)
			{
				return false;
			}
		}
		if (bigInteger.RabinMillerTest(confidence))
		{
			return true;
		}
		return false;
	}

	public bool isProbablePrime()
	{
		BigInteger bigInteger = (((A7G2UZpiBV[DAN2pw1pwE - 1] & 0x80000000u) == 0) ? this : (-this));
		if (bigInteger.dataLength == 1)
		{
			if (bigInteger.A7G2UZpiBV[0] == 0 || bigInteger.A7G2UZpiBV[0] == 1)
			{
				return false;
			}
			if (bigInteger.A7G2UZpiBV[0] == 2 || bigInteger.A7G2UZpiBV[0] == 3)
			{
				return true;
			}
		}
		if ((bigInteger.A7G2UZpiBV[0] & 1) == 0)
		{
			return false;
		}
		for (int i = 0; i < primesBelow2000.Length; i++)
		{
			BigInteger bigInteger2 = primesBelow2000[i];
			if (bigInteger2 >= bigInteger)
			{
				break;
			}
			if ((bigInteger % bigInteger2).IntValue() == 0)
			{
				return false;
			}
		}
		BigInteger bigInteger3 = bigInteger - new BigInteger(1L);
		int num = 0;
		for (int j = 0; j < bigInteger3.dataLength; j++)
		{
			uint num2 = 1u;
			for (int k = 0; k < 32; k++)
			{
				if ((bigInteger3.A7G2UZpiBV[j] & num2) != 0)
				{
					j = bigInteger3.dataLength;
					break;
				}
				num2 <<= 1;
				num++;
			}
		}
		BigInteger exp = bigInteger3 >> num;
		bigInteger.bitCount();
		BigInteger bigInteger4 = ((BigInteger)2).modPow(exp, bigInteger);
		bool flag = false;
		if (bigInteger4.dataLength == 1 && bigInteger4.A7G2UZpiBV[0] == 1)
		{
			flag = true;
		}
		int num3 = 0;
		while (!flag && num3 < num)
		{
			if (bigInteger4 == bigInteger3)
			{
				flag = true;
				break;
			}
			bigInteger4 = bigInteger4 * bigInteger4 % bigInteger;
			num3++;
		}
		if (flag)
		{
			flag = IpQ279rAj4(bigInteger);
		}
		return flag;
	}

	public int IntValue()
	{
		return (int)A7G2UZpiBV[0];
	}

	public long LongValue()
	{
		long num = A7G2UZpiBV[0];
		try
		{
			num |= (long)((ulong)A7G2UZpiBV[1] << 32);
		}
		catch (Exception)
		{
			if ((A7G2UZpiBV[0] & 0x80000000u) != 0)
			{
				num = (int)A7G2UZpiBV[0];
			}
		}
		return num;
	}

	public static int Jacobi(BigInteger a, BigInteger b)
	{
		if ((b.A7G2UZpiBV[0] & 1) == 0)
		{
			throw new ArgumentException("Jacobi defined only for odd integers.");
		}
		if (a >= b)
		{
			a %= b;
		}
		if (a.dataLength == 1 && a.A7G2UZpiBV[0] == 0)
		{
			return 0;
		}
		if (a.dataLength == 1 && a.A7G2UZpiBV[0] == 1)
		{
			return 1;
		}
		if (a < 0)
		{
			if (((b - 1).A7G2UZpiBV[0] & 2) == 0)
			{
				return Jacobi(-a, b);
			}
			return -Jacobi(-a, b);
		}
		int num = 0;
		for (int i = 0; i < a.dataLength; i++)
		{
			uint num2 = 1u;
			for (int j = 0; j < 32; j++)
			{
				if ((a.A7G2UZpiBV[i] & num2) != 0)
				{
					i = a.dataLength;
					break;
				}
				num2 <<= 1;
				num++;
			}
		}
		BigInteger bigInteger = a >> num;
		int num3 = 1;
		if ((num & 1) != 0 && ((b.A7G2UZpiBV[0] & 7) == 3 || (b.A7G2UZpiBV[0] & 7) == 5))
		{
			num3 = -1;
		}
		if ((b.A7G2UZpiBV[0] & 3) == 3 && (bigInteger.A7G2UZpiBV[0] & 3) == 3)
		{
			num3 = -num3;
		}
		if (bigInteger.dataLength == 1 && bigInteger.A7G2UZpiBV[0] == 1)
		{
			return num3;
		}
		return num3 * Jacobi(b % bigInteger, bigInteger);
	}

	public static BigInteger genPseudoPrime(int bits, int confidence, Random rand)
	{
		BigInteger bigInteger = new BigInteger();
		bool flag = false;
		while (!flag)
		{
			bigInteger.genRandomBits(bits, rand);
			bigInteger.A7G2UZpiBV[0] |= 1u;
			flag = bigInteger.isProbablePrime(confidence);
		}
		return bigInteger;
	}

	public static BigInteger genPseudoPrime(int bits, int confidence, RandomNumberGenerator rand)
	{
		BigInteger bigInteger = new BigInteger();
		bool flag = false;
		while (!flag)
		{
			bigInteger.genRandomBits(bits, rand);
			bigInteger.A7G2UZpiBV[0] |= 1u;
			flag = bigInteger.isProbablePrime(confidence);
		}
		return bigInteger;
	}

	public BigInteger genCoPrime(int bits, Random rand)
	{
		bool flag = false;
		BigInteger bigInteger = new BigInteger();
		while (!flag)
		{
			bigInteger.genRandomBits(bits, rand);
			BigInteger bigInteger2 = bigInteger.gcd(this);
			if (bigInteger2.dataLength == 1 && bigInteger2.A7G2UZpiBV[0] == 1)
			{
				flag = true;
			}
		}
		return bigInteger;
	}

	public BigInteger genCoPrime(int bits, RandomNumberGenerator rand)
	{
		bool flag = false;
		BigInteger bigInteger = new BigInteger();
		while (!flag)
		{
			bigInteger.genRandomBits(bits, rand);
			BigInteger bigInteger2 = bigInteger.gcd(this);
			if (bigInteger2.dataLength == 1 && bigInteger2.A7G2UZpiBV[0] == 1)
			{
				flag = true;
			}
		}
		return bigInteger;
	}

	public BigInteger modInverse(BigInteger modulus)
	{
		BigInteger[] array = new BigInteger[2] { 0, 1 };
		BigInteger[] array2 = new BigInteger[2];
		BigInteger[] array3 = new BigInteger[2] { 0, 0 };
		int num = 0;
		BigInteger bigInteger = modulus;
		BigInteger bigInteger2 = this;
		while (bigInteger2.dataLength > 1 || (bigInteger2.dataLength == 1 && bigInteger2.A7G2UZpiBV[0] != 0))
		{
			BigInteger bigInteger3 = new BigInteger();
			BigInteger bigInteger4 = new BigInteger();
			if (num > 1)
			{
				BigInteger bigInteger5 = (array[0] - array[1] * array2[0]) % modulus;
				array[0] = array[1];
				array[1] = bigInteger5;
			}
			if (bigInteger2.dataLength == 1)
			{
				KgQ2k5kBEa(bigInteger, bigInteger2, bigInteger3, bigInteger4);
			}
			else
			{
				mCM2JmMTo8(bigInteger, bigInteger2, bigInteger3, bigInteger4);
			}
			array2[0] = array2[1];
			array3[0] = array3[1];
			array2[1] = bigInteger3;
			array3[1] = bigInteger4;
			bigInteger = bigInteger2;
			bigInteger2 = bigInteger4;
			num++;
		}
		if (array3[0].dataLength > 1 || (array3[0].dataLength == 1 && array3[0].A7G2UZpiBV[0] != 1))
		{
			throw new ArithmeticException("No inverse!");
		}
		BigInteger bigInteger6 = (array[0] - array[1] * array2[0]) % modulus;
		if ((bigInteger6.A7G2UZpiBV[DAN2pw1pwE - 1] & 0x80000000u) != 0)
		{
			bigInteger6 += modulus;
		}
		return bigInteger6;
	}

	public byte[] getBytes()
	{
		int num = bitCount();
		int num2 = num >> 3;
		if ((num & 7) != 0)
		{
			num2++;
		}
		byte[] array = new byte[num2];
		int num3 = 0;
		uint num4 = A7G2UZpiBV[dataLength - 1];
		uint num5;
		if ((num5 = (num4 >> 24) & 0xFF) != 0)
		{
			array[num3++] = (byte)num5;
		}
		if ((num5 = (num4 >> 16) & 0xFF) != 0)
		{
			array[num3++] = (byte)num5;
		}
		else if (num3 > 0)
		{
			num3++;
		}
		if ((num5 = (num4 >> 8) & 0xFF) != 0)
		{
			array[num3++] = (byte)num5;
		}
		else if (num3 > 0)
		{
			num3++;
		}
		if ((num5 = num4 & 0xFF) != 0)
		{
			array[num3++] = (byte)num5;
		}
		else if (num3 > 0)
		{
			num3++;
		}
		int num6 = dataLength - 2;
		while (num6 >= 0)
		{
			num4 = A7G2UZpiBV[num6];
			array[num3 + 3] = (byte)(num4 & 0xFF);
			num4 >>= 8;
			array[num3 + 2] = (byte)(num4 & 0xFF);
			num4 >>= 8;
			array[num3 + 1] = (byte)(num4 & 0xFF);
			num4 >>= 8;
			array[num3] = (byte)(num4 & 0xFF);
			num6--;
			num3 += 4;
		}
		return array;
	}

	public void setBit(uint bitNum)
	{
		uint num = bitNum >> 5;
		byte b = (byte)(bitNum & 0x1F);
		uint num2 = (uint)(1 << (int)b);
		A7G2UZpiBV[num] |= num2;
		if (num >= dataLength)
		{
			dataLength = (int)(num + 1);
		}
	}

	public void unsetBit(uint bitNum)
	{
		uint num = bitNum >> 5;
		if (num < dataLength)
		{
			byte b = (byte)(bitNum & 0x1F);
			uint num2 = (uint)(1 << (int)b);
			uint num3 = 0xFFFFFFFFu ^ num2;
			A7G2UZpiBV[num] &= num3;
			if (dataLength > 1 && A7G2UZpiBV[dataLength - 1] == 0)
			{
				dataLength--;
			}
		}
	}

	public BigInteger sqrt()
	{
		uint num = (uint)bitCount();
		num = (((num & 1) == 0) ? (num >> 1) : ((num >> 1) + 1));
		uint num2 = num >> 5;
		byte b = (byte)(num & 0x1F);
		BigInteger bigInteger = new BigInteger();
		uint num3;
		if (b == 0)
		{
			num3 = 2147483648u;
		}
		else
		{
			num3 = (uint)(1 << (int)b);
			num2++;
		}
		bigInteger.dataLength = (int)num2;
		for (int num4 = (int)(num2 - 1); num4 >= 0; num4--)
		{
			while (num3 != 0)
			{
				bigInteger.A7G2UZpiBV[num4] ^= num3;
				if (bigInteger * bigInteger > this)
				{
					bigInteger.A7G2UZpiBV[num4] ^= num3;
				}
				num3 >>= 1;
			}
			num3 = 2147483648u;
		}
		return bigInteger;
	}

	public static BigInteger[] LucasSequence(BigInteger P, BigInteger Q, BigInteger k, BigInteger n)
	{
		if (k.dataLength == 1 && k.A7G2UZpiBV[0] == 0)
		{
			return new BigInteger[3]
			{
				0,
				2 % n,
				1 % n
			};
		}
		BigInteger bigInteger = new BigInteger();
		int num = n.dataLength << 1;
		bigInteger.A7G2UZpiBV[num] = 1u;
		bigInteger.dataLength = num + 1;
		bigInteger /= n;
		int num2 = 0;
		for (int i = 0; i < k.dataLength; i++)
		{
			uint num3 = 1u;
			for (int j = 0; j < 32; j++)
			{
				if ((k.A7G2UZpiBV[i] & num3) != 0)
				{
					i = k.dataLength;
					break;
				}
				num3 <<= 1;
				num2++;
			}
		}
		BigInteger bigInteger2 = k >> num2;
		return IWy2X71rmZ(P, Q, bigInteger2, n, bigInteger, num2);
	}

	private static BigInteger[] IWy2X71rmZ(BigInteger P_0, BigInteger P_1, BigInteger P_2, BigInteger P_3, BigInteger P_4, int P_5)
	{
		BigInteger[] array = new BigInteger[3];
		if ((P_2.A7G2UZpiBV[0] & 1) == 0)
		{
			throw new ArgumentException("Argument k must be odd.");
		}
		int num = P_2.bitCount();
		uint num2 = (uint)(1 << (num & 0x1F) - 1);
		BigInteger bigInteger = 2 % P_3;
		BigInteger bigInteger2 = 1 % P_3;
		BigInteger bigInteger3 = P_0 % P_3;
		BigInteger bigInteger4 = bigInteger2;
		bool flag = true;
		for (int num3 = P_2.dataLength - 1; num3 >= 0; num3--)
		{
			while (num2 != 0 && (num3 != 0 || num2 != 1))
			{
				if ((P_2.A7G2UZpiBV[num3] & num2) != 0)
				{
					bigInteger4 = bigInteger4 * bigInteger3 % P_3;
					bigInteger = (bigInteger * bigInteger3 - P_0 * bigInteger2) % P_3;
					bigInteger3 = P_3.W9o20KSE25(bigInteger3 * bigInteger3, P_3, P_4);
					bigInteger3 = (bigInteger3 - (bigInteger2 * P_1 << 1)) % P_3;
					if (flag)
					{
						flag = false;
					}
					else
					{
						bigInteger2 = P_3.W9o20KSE25(bigInteger2 * bigInteger2, P_3, P_4);
					}
					bigInteger2 = bigInteger2 * P_1 % P_3;
				}
				else
				{
					bigInteger4 = (bigInteger4 * bigInteger - bigInteger2) % P_3;
					bigInteger3 = (bigInteger * bigInteger3 - P_0 * bigInteger2) % P_3;
					bigInteger = P_3.W9o20KSE25(bigInteger * bigInteger, P_3, P_4);
					bigInteger = (bigInteger - (bigInteger2 << 1)) % P_3;
					if (flag)
					{
						bigInteger2 = P_1 % P_3;
						flag = false;
					}
					else
					{
						bigInteger2 = P_3.W9o20KSE25(bigInteger2 * bigInteger2, P_3, P_4);
					}
				}
				num2 >>= 1;
			}
			num2 = 2147483648u;
		}
		bigInteger4 = (bigInteger4 * bigInteger - bigInteger2) % P_3;
		bigInteger = (bigInteger * bigInteger3 - P_0 * bigInteger2) % P_3;
		if (flag)
		{
			flag = false;
		}
		else
		{
			bigInteger2 = P_3.W9o20KSE25(bigInteger2 * bigInteger2, P_3, P_4);
		}
		bigInteger2 = bigInteger2 * P_1 % P_3;
		for (int i = 0; i < P_5; i++)
		{
			bigInteger4 = bigInteger4 * bigInteger % P_3;
			bigInteger = (bigInteger * bigInteger - (bigInteger2 << 1)) % P_3;
			if (flag)
			{
				bigInteger2 = P_1 % P_3;
				flag = false;
			}
			else
			{
				bigInteger2 = P_3.W9o20KSE25(bigInteger2 * bigInteger2, P_3, P_4);
			}
		}
		array[0] = bigInteger4;
		array[1] = bigInteger;
		array[2] = bigInteger2;
		return array;
	}

	static BigInteger()
	{
		DAN2pw1pwE = 255;
		primesBelow2000 = new int[303]
		{
			2, 3, 5, 7, 11, 13, 17, 19, 23, 29,
			31, 37, 41, 43, 47, 53, 59, 61, 67, 71,
			73, 79, 83, 89, 97, 101, 103, 107, 109, 113,
			127, 131, 137, 139, 149, 151, 157, 163, 167, 173,
			179, 181, 191, 193, 197, 199, 211, 223, 227, 229,
			233, 239, 241, 251, 257, 263, 269, 271, 277, 281,
			283, 293, 307, 311, 313, 317, 331, 337, 347, 349,
			353, 359, 367, 373, 379, 383, 389, 397, 401, 409,
			419, 421, 431, 433, 439, 443, 449, 457, 461, 463,
			467, 479, 487, 491, 499, 503, 509, 521, 523, 541,
			547, 557, 563, 569, 571, 577, 587, 593, 599, 601,
			607, 613, 617, 619, 631, 641, 643, 647, 653, 659,
			661, 673, 677, 683, 691, 701, 709, 719, 727, 733,
			739, 743, 751, 757, 761, 769, 773, 787, 797, 809,
			811, 821, 823, 827, 829, 839, 853, 857, 859, 863,
			877, 881, 883, 887, 907, 911, 919, 929, 937, 941,
			947, 953, 967, 971, 977, 983, 991, 997, 1009, 1013,
			1019, 1021, 1031, 1033, 1039, 1049, 1051, 1061, 1063, 1069,
			1087, 1091, 1093, 1097, 1103, 1109, 1117, 1123, 1129, 1151,
			1153, 1163, 1171, 1181, 1187, 1193, 1201, 1213, 1217, 1223,
			1229, 1231, 1237, 1249, 1259, 1277, 1279, 1283, 1289, 1291,
			1297, 1301, 1303, 1307, 1319, 1321, 1327, 1361, 1367, 1373,
			1381, 1399, 1409, 1423, 1427, 1429, 1433, 1439, 1447, 1451,
			1453, 1459, 1471, 1481, 1483, 1487, 1489, 1493, 1499, 1511,
			1523, 1531, 1543, 1549, 1553, 1559, 1567, 1571, 1579, 1583,
			1597, 1601, 1607, 1609, 1613, 1619, 1621, 1627, 1637, 1657,
			1663, 1667, 1669, 1693, 1697, 1699, 1709, 1721, 1723, 1733,
			1741, 1747, 1753, 1759, 1777, 1783, 1787, 1789, 1801, 1811,
			1823, 1831, 1847, 1861, 1867, 1871, 1873, 1877, 1879, 1889,
			1901, 1907, 1913, 1931, 1933, 1949, 1951, 1973, 1979, 1987,
			1993, 1997, 1999
		};
	}
}
