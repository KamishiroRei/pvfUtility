using System;
using System.Globalization;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace Swordfish.NET.Collections.Auxiliary;

[Serializable]
[ComVisible(false)]
public struct BigRationalOld : IComparable, IComparable<BigRationalOld>, IDeserializationCallback, IEquatable<BigRationalOld>, ISerializable
{
	[StructLayout(LayoutKind.Explicit)]
	internal struct DoubleUlong
	{
		[FieldOffset(0)]
		public double dbl;

		[FieldOffset(0)]
		public ulong uu;
	}

	[StructLayout(LayoutKind.Explicit)]
	internal struct DecimalUInt32
	{
		[FieldOffset(0)]
		public decimal dec;

		[FieldOffset(0)]
		public int flags;
	}

	private BigInteger m_numerator;

	private BigInteger m_denominator;

	private static readonly BigRationalOld s_brZero = new BigRationalOld(BigInteger.Zero);

	private static readonly BigRationalOld s_brOne = new BigRationalOld(BigInteger.One);

	private static readonly BigRationalOld s_brMinusOne = new BigRationalOld(BigInteger.MinusOne);

	private const int DoubleMaxScale = 308;

	private static readonly BigInteger s_bnDoublePrecision = BigInteger.Pow(10, 308);

	private static readonly BigInteger s_bnDoubleMaxValue = (BigInteger)double.MaxValue;

	private static readonly BigInteger s_bnDoubleMinValue = (BigInteger)double.MinValue;

	private const int DecimalScaleMask = 16711680;

	private const int DecimalSignMask = int.MinValue;

	private const int DecimalMaxScale = 28;

	private static readonly BigInteger s_bnDecimalPrecision = BigInteger.Pow(10, 28);

	private static readonly BigInteger s_bnDecimalMaxValue = (BigInteger)decimal.MaxValue;

	private static readonly BigInteger s_bnDecimalMinValue = (BigInteger)decimal.MinValue;

	private const string c_solidus = "/";

	public static BigRationalOld Zero => s_brZero;

	public static BigRationalOld One => s_brOne;

	public static BigRationalOld MinusOne => s_brMinusOne;

	public int Sign => m_numerator.Sign;

	public BigInteger Numerator => m_numerator;

	public BigInteger Denominator => m_denominator;

	public BigInteger GetWholePart()
	{
		return BigInteger.Divide(m_numerator, m_denominator);
	}

	public BigRationalOld GetFractionPart()
	{
		return new BigRationalOld(BigInteger.Remainder(m_numerator, m_denominator), m_denominator);
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (!(obj is BigRationalOld))
		{
			return false;
		}
		return Equals((BigRationalOld)obj);
	}

	public override int GetHashCode()
	{
		return (m_numerator / Denominator).GetHashCode();
	}

	int IComparable.CompareTo(object obj)
	{
		if (obj == null)
		{
			return 1;
		}
		if (!(obj is BigRationalOld))
		{
			throw new ArgumentException("Argument must be of type BigRational", "obj");
		}
		return Compare(this, (BigRationalOld)obj);
	}

	public int CompareTo(BigRationalOld other)
	{
		return Compare(this, other);
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(m_numerator.ToString("R", CultureInfo.InvariantCulture));
		stringBuilder.Append("/");
		stringBuilder.Append(Denominator.ToString("R", CultureInfo.InvariantCulture));
		return stringBuilder.ToString();
	}

	public bool Equals(BigRationalOld other)
	{
		if (Denominator == other.Denominator)
		{
			return m_numerator == other.m_numerator;
		}
		return m_numerator * other.Denominator == Denominator * other.m_numerator;
	}

	public BigRationalOld(BigInteger numerator)
	{
		m_numerator = numerator;
		m_denominator = BigInteger.One;
	}

	public BigRationalOld(double value)
	{
		if (double.IsNaN(value))
		{
			throw new ArgumentException("Argument is not a number", "value");
		}
		if (double.IsInfinity(value))
		{
			throw new ArgumentException("Argument is infinity", "value");
		}
		SplitDoubleIntoParts(value, out var sign, out var exp, out var man, out var _);
		if (man == 0L)
		{
			this = Zero;
			return;
		}
		m_numerator = man;
		m_denominator = 1048576;
		if (exp > 0)
		{
			m_numerator = BigInteger.Pow(m_numerator, exp);
		}
		else if (exp < 0)
		{
			m_denominator = BigInteger.Pow(m_denominator, -exp);
		}
		if (sign < 0)
		{
			m_numerator = BigInteger.Negate(m_numerator);
		}
		Simplify();
	}

	public BigRationalOld(decimal value)
	{
		int[] bits = decimal.GetBits(value);
		if (bits == null || bits.Length != 4 || (bits[3] & 0x7F00FFFF) != 0 || (bits[3] & 0xFF0000) > 1835008)
		{
			throw new ArgumentException("invalid Decimal", "value");
		}
		if (value == 0m)
		{
			this = Zero;
			return;
		}
		ulong value2 = ((ulong)(uint)bits[2] << 32) | (uint)bits[1];
		m_numerator = (new BigInteger(value2) << 32) | (uint)bits[0];
		if ((bits[3] & int.MinValue) != 0)
		{
			m_numerator = BigInteger.Negate(m_numerator);
		}
		int exponent = (bits[3] & 0xFF0000) >> 16;
		m_denominator = BigInteger.Pow(10, exponent);
		Simplify();
	}

	public BigRationalOld(BigInteger numerator, BigInteger denominator)
	{
		if (denominator.Sign == 0)
		{
			throw new DivideByZeroException();
		}
		if (numerator.Sign == 0)
		{
			m_numerator = BigInteger.Zero;
			m_denominator = BigInteger.One;
		}
		else if (denominator.Sign < 0)
		{
			m_numerator = BigInteger.Negate(numerator);
			m_denominator = BigInteger.Negate(denominator);
		}
		else
		{
			m_numerator = numerator;
			m_denominator = denominator;
		}
		Simplify();
	}

	public BigRationalOld(BigInteger whole, BigInteger numerator, BigInteger denominator)
	{
		if (denominator.Sign == 0)
		{
			throw new DivideByZeroException();
		}
		if (numerator.Sign == 0 && whole.Sign == 0)
		{
			m_numerator = BigInteger.Zero;
			m_denominator = BigInteger.One;
		}
		else if (denominator.Sign < 0)
		{
			m_denominator = BigInteger.Negate(denominator);
			m_numerator = BigInteger.Negate(whole) * m_denominator + BigInteger.Negate(numerator);
		}
		else
		{
			m_denominator = denominator;
			m_numerator = whole * denominator + numerator;
		}
		Simplify();
	}

	public static BigRationalOld Abs(BigRationalOld r)
	{
		if (r.m_numerator.Sign >= 0)
		{
			return r;
		}
		return new BigRationalOld(BigInteger.Abs(r.m_numerator), r.Denominator);
	}

	public static BigRationalOld Negate(BigRationalOld r)
	{
		return new BigRationalOld(BigInteger.Negate(r.m_numerator), r.Denominator);
	}

	public static BigRationalOld Invert(BigRationalOld r)
	{
		return new BigRationalOld(r.Denominator, r.m_numerator);
	}

	public static BigRationalOld Add(BigRationalOld x, BigRationalOld y)
	{
		return x + y;
	}

	public static BigRationalOld Subtract(BigRationalOld x, BigRationalOld y)
	{
		return x - y;
	}

	public static BigRationalOld Multiply(BigRationalOld x, BigRationalOld y)
	{
		return x * y;
	}

	public static BigRationalOld Divide(BigRationalOld dividend, BigRationalOld divisor)
	{
		return dividend / divisor;
	}

	public static BigRationalOld Remainder(BigRationalOld dividend, BigRationalOld divisor)
	{
		return dividend % divisor;
	}

	public static BigRationalOld DivRem(BigRationalOld dividend, BigRationalOld divisor, out BigRationalOld remainder)
	{
		BigInteger bigInteger = dividend.m_numerator * divisor.Denominator;
		BigInteger bigInteger2 = dividend.Denominator * divisor.m_numerator;
		BigInteger denominator = dividend.Denominator * divisor.Denominator;
		remainder = new BigRationalOld(bigInteger % bigInteger2, denominator);
		return new BigRationalOld(bigInteger, bigInteger2);
	}

	public static BigRationalOld Pow(BigRationalOld baseValue, BigInteger exponent)
	{
		if (exponent.Sign == 0)
		{
			return One;
		}
		if (exponent.Sign < 0)
		{
			if (baseValue == Zero)
			{
				throw new ArgumentException("cannot raise zero to a negative power", "baseValue");
			}
			baseValue = Invert(baseValue);
			exponent = BigInteger.Negate(exponent);
		}
		BigRationalOld result = baseValue;
		while (exponent > BigInteger.One)
		{
			result *= baseValue;
			--exponent;
		}
		return result;
	}

	public static BigInteger LeastCommonDenominator(BigRationalOld x, BigRationalOld y)
	{
		return x.Denominator * y.Denominator / BigInteger.GreatestCommonDivisor(x.Denominator, y.Denominator);
	}

	public static int Compare(BigRationalOld r1, BigRationalOld r2)
	{
		return BigInteger.Compare(r1.m_numerator * r2.Denominator, r2.m_numerator * r1.Denominator);
	}

	public static bool operator ==(BigRationalOld x, BigRationalOld y)
	{
		return Compare(x, y) == 0;
	}

	public static bool operator !=(BigRationalOld x, BigRationalOld y)
	{
		return Compare(x, y) != 0;
	}

	public static bool operator <(BigRationalOld x, BigRationalOld y)
	{
		return Compare(x, y) < 0;
	}

	public static bool operator <=(BigRationalOld x, BigRationalOld y)
	{
		return Compare(x, y) <= 0;
	}

	public static bool operator >(BigRationalOld x, BigRationalOld y)
	{
		return Compare(x, y) > 0;
	}

	public static bool operator >=(BigRationalOld x, BigRationalOld y)
	{
		return Compare(x, y) >= 0;
	}

	public static BigRationalOld operator +(BigRationalOld r)
	{
		return r;
	}

	public static BigRationalOld operator -(BigRationalOld r)
	{
		return new BigRationalOld(-r.m_numerator, r.Denominator);
	}

	public static BigRationalOld operator ++(BigRationalOld r)
	{
		return r + One;
	}

	public static BigRationalOld operator --(BigRationalOld r)
	{
		return r - One;
	}

	public static BigRationalOld operator +(BigRationalOld r1, BigRationalOld r2)
	{
		return new BigRationalOld(r1.m_numerator * r2.Denominator + r1.Denominator * r2.m_numerator, r1.Denominator * r2.Denominator);
	}

	public static BigRationalOld operator -(BigRationalOld r1, BigRationalOld r2)
	{
		return new BigRationalOld(r1.m_numerator * r2.Denominator - r1.Denominator * r2.m_numerator, r1.Denominator * r2.Denominator);
	}

	public static BigRationalOld operator *(BigRationalOld r1, BigRationalOld r2)
	{
		return new BigRationalOld(r1.m_numerator * r2.m_numerator, r1.Denominator * r2.Denominator);
	}

	public static BigRationalOld operator /(BigRationalOld r1, BigRationalOld r2)
	{
		return new BigRationalOld(r1.m_numerator * r2.Denominator, r1.Denominator * r2.m_numerator);
	}

	public static BigRationalOld operator %(BigRationalOld r1, BigRationalOld r2)
	{
		return new BigRationalOld(r1.m_numerator * r2.Denominator % (r1.Denominator * r2.m_numerator), r1.Denominator * r2.Denominator);
	}

	[CLSCompliant(false)]
	public static explicit operator sbyte(BigRationalOld value)
	{
		return (sbyte)BigInteger.Divide(value.m_numerator, value.m_denominator);
	}

	[CLSCompliant(false)]
	public static explicit operator ushort(BigRationalOld value)
	{
		return (ushort)BigInteger.Divide(value.m_numerator, value.m_denominator);
	}

	[CLSCompliant(false)]
	public static explicit operator uint(BigRationalOld value)
	{
		return (uint)BigInteger.Divide(value.m_numerator, value.m_denominator);
	}

	[CLSCompliant(false)]
	public static explicit operator ulong(BigRationalOld value)
	{
		return (ulong)BigInteger.Divide(value.m_numerator, value.m_denominator);
	}

	public static explicit operator byte(BigRationalOld value)
	{
		return (byte)BigInteger.Divide(value.m_numerator, value.m_denominator);
	}

	public static explicit operator short(BigRationalOld value)
	{
		return (short)BigInteger.Divide(value.m_numerator, value.m_denominator);
	}

	public static explicit operator int(BigRationalOld value)
	{
		return (int)BigInteger.Divide(value.m_numerator, value.m_denominator);
	}

	public static explicit operator long(BigRationalOld value)
	{
		return (long)BigInteger.Divide(value.m_numerator, value.m_denominator);
	}

	public static explicit operator BigInteger(BigRationalOld value)
	{
		return BigInteger.Divide(value.m_numerator, value.m_denominator);
	}

	public static explicit operator float(BigRationalOld value)
	{
		return (float)(double)value;
	}

	public static explicit operator double(BigRationalOld value)
	{
		if (SafeCastToDouble(value.m_numerator) && SafeCastToDouble(value.m_denominator))
		{
			return (double)value.m_numerator / (double)value.m_denominator;
		}
		BigInteger bigInteger = value.m_numerator * s_bnDoublePrecision / value.m_denominator;
		if (bigInteger.IsZero)
		{
			if (value.Sign >= 0)
			{
				return 0.0;
			}
			return BitConverter.Int64BitsToDouble(long.MinValue);
		}
		double num = 0.0;
		bool flag = false;
		for (int num2 = 308; num2 > 0; num2--)
		{
			if (!flag)
			{
				if (SafeCastToDouble(bigInteger))
				{
					num = (double)bigInteger;
					flag = true;
				}
				else
				{
					bigInteger /= (BigInteger)10;
				}
			}
			num /= 10.0;
		}
		if (!flag)
		{
			if (value.Sign >= 0)
			{
				return double.PositiveInfinity;
			}
			return double.NegativeInfinity;
		}
		return num;
	}

	public static explicit operator decimal(BigRationalOld value)
	{
		if (SafeCastToDecimal(value.m_numerator) && SafeCastToDecimal(value.m_denominator))
		{
			return (decimal)value.m_numerator / (decimal)value.m_denominator;
		}
		BigInteger bigInteger = value.m_numerator * s_bnDecimalPrecision / value.m_denominator;
		if (bigInteger.IsZero)
		{
			return 0m;
		}
		int num = 28;
		while (num >= 0)
		{
			if (!SafeCastToDecimal(bigInteger))
			{
				bigInteger /= (BigInteger)10;
				num--;
				continue;
			}
			DecimalUInt32 decimalUInt = default(DecimalUInt32);
			decimalUInt.dec = (decimal)bigInteger;
			decimalUInt.flags = (decimalUInt.flags & -16711681) | (num << 16);
			return decimalUInt.dec;
		}
		throw new OverflowException("Value was either too large or too small for a Decimal.");
	}

	[CLSCompliant(false)]
	public static implicit operator BigRationalOld(sbyte value)
	{
		return new BigRationalOld((BigInteger)value);
	}

	[CLSCompliant(false)]
	public static implicit operator BigRationalOld(ushort value)
	{
		return new BigRationalOld((BigInteger)value);
	}

	[CLSCompliant(false)]
	public static implicit operator BigRationalOld(uint value)
	{
		return new BigRationalOld((BigInteger)value);
	}

	[CLSCompliant(false)]
	public static implicit operator BigRationalOld(ulong value)
	{
		return new BigRationalOld((BigInteger)value);
	}

	public static implicit operator BigRationalOld(byte value)
	{
		return new BigRationalOld((BigInteger)value);
	}

	public static implicit operator BigRationalOld(short value)
	{
		return new BigRationalOld((BigInteger)value);
	}

	public static implicit operator BigRationalOld(int value)
	{
		return new BigRationalOld((BigInteger)value);
	}

	public static implicit operator BigRationalOld(long value)
	{
		return new BigRationalOld((BigInteger)value);
	}

	public static implicit operator BigRationalOld(BigInteger value)
	{
		return new BigRationalOld(value);
	}

	public static implicit operator BigRationalOld(float value)
	{
		return new BigRationalOld(value);
	}

	public static implicit operator BigRationalOld(double value)
	{
		return new BigRationalOld(value);
	}

	public static implicit operator BigRationalOld(decimal value)
	{
		return new BigRationalOld(value);
	}

	void IDeserializationCallback.OnDeserialization(object sender)
	{
		try
		{
			if (m_denominator.Sign == 0 || m_numerator.Sign == 0)
			{
				m_numerator = BigInteger.Zero;
				m_denominator = BigInteger.One;
			}
			else if (m_denominator.Sign < 0)
			{
				m_numerator = BigInteger.Negate(m_numerator);
				m_denominator = BigInteger.Negate(m_denominator);
			}
			Simplify();
		}
		catch (ArgumentException innerException)
		{
			throw new SerializationException("invalid serialization data", innerException);
		}
	}

	[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
	void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
	{
		if (info == null)
		{
			throw new ArgumentNullException("info");
		}
		info.AddValue("Numerator", m_numerator);
		info.AddValue("Denominator", m_denominator);
	}

	private BigRationalOld(SerializationInfo info, StreamingContext context)
	{
		if (info == null)
		{
			throw new ArgumentNullException("info");
		}
		m_numerator = (BigInteger)info.GetValue("Numerator", typeof(BigInteger));
		m_denominator = (BigInteger)info.GetValue("Denominator", typeof(BigInteger));
	}

	private void Simplify()
	{
		if (m_numerator == BigInteger.Zero)
		{
			m_denominator = BigInteger.One;
		}
		BigInteger bigInteger = BigInteger.GreatestCommonDivisor(m_numerator, m_denominator);
		if (bigInteger > BigInteger.One)
		{
			m_numerator /= bigInteger;
			m_denominator = Denominator / bigInteger;
		}
	}

	private static bool SafeCastToDouble(BigInteger value)
	{
		if (s_bnDoubleMinValue <= value)
		{
			return value <= s_bnDoubleMaxValue;
		}
		return false;
	}

	private static bool SafeCastToDecimal(BigInteger value)
	{
		if (s_bnDecimalMinValue <= value)
		{
			return value <= s_bnDecimalMaxValue;
		}
		return false;
	}

	private static void SplitDoubleIntoParts(double dbl, out int sign, out int exp, out ulong man, out bool isFinite)
	{
		DoubleUlong doubleUlong = default(DoubleUlong);
		doubleUlong.uu = 0uL;
		doubleUlong.dbl = dbl;
		sign = 1 - ((int)(doubleUlong.uu >> 62) & 2);
		man = doubleUlong.uu & 0xFFFFFFFFFFFFFL;
		exp = (int)(doubleUlong.uu >> 52) & 0x7FF;
		if (exp == 0)
		{
			isFinite = true;
			if (man != 0L)
			{
				exp = -1074;
			}
		}
		else if (exp == 2047)
		{
			isFinite = false;
			exp = int.MaxValue;
		}
		else
		{
			isFinite = true;
			man |= 4503599627370496uL;
			exp -= 1075;
		}
	}

	private static double GetDoubleFromParts(int sign, int exp, ulong man)
	{
		DoubleUlong doubleUlong = default(DoubleUlong);
		doubleUlong.dbl = 0.0;
		if (man == 0L)
		{
			doubleUlong.uu = 0uL;
		}
		else
		{
			int num = CbitHighZero(man) - 11;
			man = ((num >= 0) ? (man << num) : (man >> -num));
			exp += 1075;
			if (exp >= 2047)
			{
				doubleUlong.uu = 9218868437227405312uL;
			}
			else if (exp <= 0)
			{
				exp--;
				if (exp < -52)
				{
					doubleUlong.uu = 0uL;
				}
				else
				{
					doubleUlong.uu = man >> -exp;
				}
			}
			else
			{
				doubleUlong.uu = (man & 0xFFFFFFFFFFFFFL) | (ulong)((long)exp << 52);
			}
		}
		if (sign < 0)
		{
			doubleUlong.uu |= 9223372036854775808uL;
		}
		return doubleUlong.dbl;
	}

	private static int CbitHighZero(ulong uu)
	{
		if ((uu & 0xFFFFFFFF00000000uL) == 0L)
		{
			return 32 + CbitHighZero((uint)uu);
		}
		return CbitHighZero((uint)(uu >> 32));
	}

	private static int CbitHighZero(uint u)
	{
		if (u == 0)
		{
			return 32;
		}
		int num = 0;
		if ((u & 0xFFFF0000u) == 0)
		{
			num += 16;
			u <<= 16;
		}
		if ((u & 0xFF000000u) == 0)
		{
			num += 8;
			u <<= 8;
		}
		if ((u & 0xF0000000u) == 0)
		{
			num += 4;
			u <<= 4;
		}
		if ((u & 0xC0000000u) == 0)
		{
			num += 2;
			u <<= 2;
		}
		if ((u & 0x80000000u) == 0)
		{
			num++;
		}
		return num;
	}

}
